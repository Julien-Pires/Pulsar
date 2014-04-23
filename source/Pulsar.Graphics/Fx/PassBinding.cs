using System.Diagnostics;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a binding to a pass of a shader technique
    /// </summary>
    internal sealed class PassBinding
    {
        #region Fields

        private readonly PassDefinition _passDefinition;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of PassBinding class
        /// </summary>
        /// <param name="passDefinition">Definition of the pass</param>
        internal PassBinding(PassDefinition passDefinition)
        {
            Debug.Assert(passDefinition != null);

            _passDefinition = passDefinition;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets this pass as the active pass
        /// </summary>
        internal void Apply()
        {
            _passDefinition.Pass.Apply();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the pass
        /// </summary>
        public string Name
        {
            get { return _passDefinition.Name; }
        }

        /// <summary>
        /// Gets the render state of the pass
        /// </summary>
        public RenderState RenderState
        {
            get { return _passDefinition.State; }
        }

        #endregion
    }
}
