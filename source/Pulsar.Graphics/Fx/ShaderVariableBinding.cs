using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents the base class for a shader variable binding
    /// </summary>
    public abstract class ShaderVariableBinding
    {
        #region Constructors

        /// <summary>
        /// Constructor of ShaderVariableBinding class
        /// </summary>
        /// <param name="definition">Variable definition</param>
        protected ShaderVariableBinding(ShaderVariableDefinition definition)
        {
            Debug.Assert(definition.Parameter != null, "Effect parameter cannot be null");

            FxParameter = definition.Parameter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the variable value
        /// </summary>
        /// <param name="context">Frame context</param>
        public abstract void Update(FrameContext context);

        /// <summary>
        /// Writes the value to the effect parameter
        /// </summary>
        public abstract void Write();

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the underlying effect parameter
        /// </summary>
        internal EffectParameter FxParameter { get; private set; }

        /// <summary>
        /// Gets the name of the variable
        /// </summary>
        public string Name
        {
            get { return FxParameter.Name; }
        }

        #endregion
    }
}
