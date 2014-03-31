using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    public sealed class RenderState
    {
        #region Fields

        private static readonly List<StateObject<RasterizerState>> _rasterizerStates =
            new List<StateObject<RasterizerState>>();
        private static readonly List<StateObject<DepthStencilState>> _DepthStencilStates =
            new List<StateObject<DepthStencilState>>();
        private static readonly List<StateObject<BlendState>> _blendStates = new List<StateObject<BlendState>>();

        private readonly RenderStateId _id;
        private readonly StateObject<RasterizerState> _rasterizerState;
        private readonly StateObject<DepthStencilState> _depthStencilState;
        private readonly StateObject<BlendState> _blendState;

        #endregion

        #region Constructors

        internal RenderState(StateObject<RasterizerState> rasterizerState,
            StateObject<DepthStencilState> depthStencilState, StateObject<BlendState> blendState)
        {
            _rasterizerState = rasterizerState;
            _depthStencilState = depthStencilState;
            _blendState = blendState;
            _id = new RenderStateId(rasterizerState.Id, depthStencilState.Id, blendState.Id, 0);
        }

        #endregion

        #region Static methods

        internal static StateObject<RasterizerState> FindRasterizerState()
        {
            
        }

        internal static StateObject<DepthStencilState> FindDepthStencilState()
        {
            
        }

        internal static StateObject<BlendState> FindBlendState()
        {
            
        }

        #endregion

        #region Properties

        public RenderStateId Id
        {
            get { return _id; }
        }

        public RasterizerState RasterizerState
        {
            get { return _rasterizerState.State; }
        }

        public DepthStencilState DepthStencilState
        {
            get { return _depthStencilState.State; }
        }

        public BlendState BlendState
        {
            get { return _blendState.State; }
        }

        #endregion
    }
}