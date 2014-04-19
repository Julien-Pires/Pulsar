using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    public sealed class RenderState
    {
        #region Fields

        private static readonly List<StateObject<RasterizerState>> RasterizerStates =
            new List<StateObject<RasterizerState>>();
        private static readonly List<StateObject<DepthStencilState>> DepthStencilStates =
            new List<StateObject<DepthStencilState>>();
        private static readonly List<StateObject<BlendState>> BlendStates = new List<StateObject<BlendState>>();
        private static readonly Dictionary<ulong, RenderState> RenderStates = new Dictionary<ulong, RenderState>();
        public static readonly RenderState Default;

        private readonly RenderStateId _id;
        private readonly RasterizerState _rasterizerState;
        private readonly DepthStencilState _depthStencilState;
        private readonly BlendState _blendState;

        #endregion

        #region Constructors

        static RenderState()
        {
            StateObject<RasterizerState> rasterizerState = RegisterState(RasterizerState.CullCounterClockwise,
                RasterizerStates);
            StateObject<DepthStencilState> depthstencilState = RegisterState(DepthStencilState.Default,
                DepthStencilStates);
            StateObject<BlendState> blendState = RegisterState(BlendState.Opaque, BlendStates);
            Default = GetRenderState(rasterizerState, depthstencilState, blendState);
        }

        internal RenderState(StateObject<RasterizerState> rasterizerState,
            StateObject<DepthStencilState> depthStencilState, StateObject<BlendState> blendState)
        {
            _rasterizerState = rasterizerState.State;
            _depthStencilState = depthStencilState.State;
            _blendState = blendState.State;
            _id = new RenderStateId(rasterizerState.Id, depthStencilState.Id, blendState.Id, 0);
        }

        #endregion

        #region Static methods

        private static StateObject<T> RegisterState<T>(T state, List<StateObject<T>> list) where T :GraphicsResource
        {
            Debug.Assert(state != null);

            StateObject<T> result = new StateObject<T>(state);
            list.Add(result);

            return result;
        }

        public static RenderState GetRenderState(StateObject<RasterizerState> rasterizerState, 
            StateObject<DepthStencilState> depthStencilState, StateObject<BlendState> blendState)
        {
            if(rasterizerState == null)
                throw new ArgumentNullException("rasterizerState");

            if(depthStencilState == null)
                throw new ArgumentNullException("depthStencilState");

            if(blendState == null)
                throw new ArgumentNullException("blendState");

            RenderStateId stateId = new RenderStateId(rasterizerState.Id, depthStencilState.Id, blendState.Id, 0);
            RenderState result;
            if (RenderStates.TryGetValue(stateId.Id, out result))
                return result;

            result = new RenderState(rasterizerState, depthStencilState, blendState);
            RenderStates.Add(result.Id, result);

            return result;
        }

        public static StateObject<RasterizerState> GetRasterizerState(CullMode cull, FillMode mode)
        {
            for (int i = 0; i < RasterizerStates.Count; i++)
            {
                RasterizerState current = RasterizerStates[i].State;
                if ((current.CullMode == cull) && (current.FillMode == mode))
                    return RasterizerStates[i];
            }

            RasterizerState newState = new RasterizerState
            {
                CullMode = cull,
                FillMode = mode
            };

            return RegisterState(newState, RasterizerStates);
        }

        public static StateObject<DepthStencilState> GetDepthStencilState(bool zWrite, CompareFunction zCompare,
            int stencilRef, int stencilMask, int stencilWriteMask, CompareFunction stencilCompare, StencilOperation pass,
            StencilOperation fail, StencilOperation depthFail)
        {
            for (int i = 0; i < DepthStencilStates.Count; i++)
            {
                DepthStencilState current = DepthStencilStates[i].State;
                bool depthState = (current.DepthBufferWriteEnable == zWrite) &&
                                  (current.DepthBufferFunction == zCompare);
                bool stencilMaskState = (current.ReferenceStencil == stencilRef) && (current.StencilMask == stencilMask) &&
                                        (current.StencilWriteMask == stencilWriteMask);
                bool stencFuncState = (current.StencilFunction == stencilCompare) && (current.StencilPass == pass) &&
                                      (current.StencilFail == fail) && (current.StencilDepthBufferFail == depthFail);

                if (depthState && stencilMaskState && stencFuncState)
                    return DepthStencilStates[i];
            }

            DepthStencilState newState = new DepthStencilState
            {
                DepthBufferWriteEnable = zWrite,
                DepthBufferFunction = zCompare,
                ReferenceStencil = stencilRef,
                StencilMask = stencilMask,
                StencilWriteMask = stencilWriteMask,
                StencilFunction = stencilCompare,
                StencilPass = pass,
                StencilFail = fail,
                StencilDepthBufferFail = depthFail
            };

            return RegisterState(newState, DepthStencilStates);
        }

        public static StateObject<BlendState> GetBlendState(BlendFunction colorBlend, BlendFunction alphaBlend, 
            Blend colorSrc, Blend colorDst, Blend alphaSrc, Blend alphaDst)
        {
            for (int i = 0; i < BlendStates.Count; i++)
            {
                BlendState current = BlendStates[i].State;
                bool blendFuncState = (current.ColorBlendFunction == colorBlend) &&
                                      (current.AlphaBlendFunction == alphaBlend);
                bool blendState = (current.ColorSourceBlend == colorSrc) && (current.ColorDestinationBlend == colorDst) &&
                                  (current.AlphaSourceBlend == alphaSrc) && (current.AlphaDestinationBlend == alphaDst);

                if (blendFuncState && blendState)
                    return BlendStates[i];
            }

            BlendState newState = new BlendState
            {
                ColorBlendFunction = colorBlend,
                AlphaBlendFunction = alphaBlend,
                ColorSourceBlend = colorSrc,
                ColorDestinationBlend = colorDst,
                AlphaSourceBlend = alphaSrc,
                AlphaDestinationBlend = alphaDst
            };

            return RegisterState(newState, BlendStates);
        }

        #endregion

        #region Properties

        public RenderStateId Id
        {
            get { return _id; }
        }

        public RasterizerState Rasterizer
        {
            get { return _rasterizerState; }
        }

        public DepthStencilState DepthStencil
        {
            get { return _depthStencilState; }
        }

        public BlendState Blend
        {
            get { return _blendState; }
        }

        #endregion
    }
}