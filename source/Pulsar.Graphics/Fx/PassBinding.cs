using System.Diagnostics;

namespace Pulsar.Graphics.Fx
{
    internal sealed class PassBinding
    {
        #region Fields

        private readonly PassDefinition _passDefinition;

        #endregion

        #region Constructors

        internal PassBinding(PassDefinition passDefinition)
        {
            Debug.Assert(passDefinition != null);

            _passDefinition = passDefinition;
        }

        #endregion

        #region Methods

        internal void Apply()
        {
            _passDefinition.Pass.Apply();
        }

        #endregion

        #region Properties

        public ushort Id
        {
            get { return _passDefinition.Id; }
        }

        public string Name
        {
            get { return _passDefinition.Name; }
        }

        public RenderState RenderState
        {
            get { return _passDefinition.State; }
        }

        #endregion
    }
}
