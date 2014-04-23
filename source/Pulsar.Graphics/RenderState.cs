using System;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents a set of render state
    /// </summary>
    public sealed class RenderState
    {
        #region Fields

        private static readonly List<StateObject<RasterizerState>> RasterizerStates =
            new List<StateObject<RasterizerState>>();
        private static readonly List<StateObject<DepthStencilState>> DepthStencilStates =
            new List<StateObject<DepthStencilState>>();
        private static readonly List<StateObject<BlendState>> BlendStates = new List<StateObject<BlendState>>();
        private static readonly Dictionary<ulong, RenderState> RenderStates = new Dictionary<ulong, RenderState>();

        /// <summary>
        /// Default render state
        /// </summary>
        public static readonly RenderState Default;

        private readonly RenderStateId _id;
        private readonly RasterizerState _rasterizerState;
        private readonly DepthStencilState _depthStencilState;
        private readonly BlendState _blendState;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor of RenderState class
        /// </summary>
        static RenderState()
        {
            StateObject<RasterizerState> rasterizerState = RegisterState(RasterizerState.CullCounterClockwise,
                RasterizerStates);
            StateObject<DepthStencilState> depthstencilState = RegisterState(DepthStencilState.Default,
                DepthStencilStates);
            StateObject<BlendState> blendState = RegisterState(BlendState.Opaque, BlendStates);
            Default = GetRenderState(rasterizerState, depthstencilState, blendState);
        }

        /// <summary>
        /// Constructor of RenderState class
        /// </summary>
        /// <param name="rasterizerState">Rasterizer</param>
        /// <param name="depthStencilState">DepthStencil</param>
        /// <param name="blendState">BlendState</param>
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

        /// <summary>
        /// Registers a new state object
        /// </summary>
        /// <typeparam name="T">State type</typeparam>
        /// <param name="state">State object</param>
        /// <param name="list">List that keep existing state object</param>
        /// <returns>Returns a state object instance</returns>
        private static StateObject<T> RegisterState<T>(T state, List<StateObject<T>> list) where T :GraphicsResource
        {
            Debug.Assert(state != null);

            StateObject<T> result = new StateObject<T>(state);
            list.Add(result);

            return result;
        }

        /// <summary>
        /// Gets a RenderState instance for a set of specified state object
        /// </summary>
        /// <param name="rasterizerState">Rasterizer</param>
        /// <param name="depthStencilState">DepthStencil</param>
        /// <param name="blendState">Blend</param>
        /// <returns>Returns a RenderState instance</returns>
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

        /// <summary>
        /// Gets a rasterizer state object
        /// </summary>
        /// <param name="cull">Cull mode</param>
        /// <param name="fill">Fill mode</param>
        /// <returns>Returns a rasterizer state</returns>
        public static StateObject<RasterizerState> GetRasterizerState(CullMode cull, FillMode fill)
        {
            for (int i = 0; i < RasterizerStates.Count; i++)
            {
                RasterizerState current = RasterizerStates[i].State;
                if ((current.CullMode == cull) && (current.FillMode == fill))
                    return RasterizerStates[i];
            }

            RasterizerState newState = new RasterizerState
            {
                CullMode = cull,
                FillMode = fill
            };

            return RegisterState(newState, RasterizerStates);
        }

        /// <summary>
        /// Gets a depthstencil state object
        /// </summary>
        /// <param name="zWrite">Enable write to z-buffer</param>
        /// <param name="zCompare">Depth compare method</param>
        /// <param name="stencilRef">Stencil reference value</param>
        /// <param name="stencilMask">Stencil mask</param>
        /// <param name="stencilWriteMask">Stencil write mask</param>
        /// <param name="stencilCompare">Stencil compare method</param>
        /// <param name="pass">Stencil pass method</param>
        /// <param name="fail">Stencil fail method</param>
        /// <param name="depthFail">Depth fail method</param>
        /// <returns>Returns a depthstencil state</returns>
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

        /// <summary>
        /// Gets a blend state object
        /// </summary>
        /// <param name="colorBlend">Color blend function</param>
        /// <param name="alphaBlend">Alpha blend function</param>
        /// <param name="colorSrc">Color source</param>
        /// <param name="colorDst">Color destination</param>
        /// <param name="alphaSrc">Alpha source</param>
        /// <param name="alphaDst">Alpha destination</param>
        /// <returns>Returns a blend state</returns>
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

        /// <summary>
        /// Gets the render state id
        /// </summary>
        public RenderStateId Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the rasterizer state
        /// </summary>
        public RasterizerState Rasterizer
        {
            get { return _rasterizerState; }
        }

        /// <summary>
        /// Gets the depthstencil state
        /// </summary>
        public DepthStencilState DepthStencil
        {
            get { return _depthStencilState; }
        }

        /// <summary>
        /// Gets the blend state
        /// </summary>
        public BlendState Blend
        {
            get { return _blendState; }
        }

        #endregion
    }
}