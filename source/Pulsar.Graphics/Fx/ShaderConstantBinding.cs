using System.Diagnostics;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents the base class for a shader constant binding
    /// </summary>
    public abstract class ShaderConstantBinding
    {
        #region Constructors

        /// <summary>
        /// Constructor of ShaderConstantBinding class
        /// </summary>
        /// <param name="definition">Constant definition</param>
        internal ShaderConstantBinding(ShaderConstantDefinition definition)
        {
            Debug.Assert(definition.Parameter != null, "Effect parameter cannot be null");

            FxParameter = definition.Parameter;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the value
        /// </summary>
        /// <param name="context">Frame context</param>
        internal abstract void Update(FrameContext context);

        /// <summary>
        /// Writes the value to the effect parameter
        /// </summary>
        internal abstract void Write();

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the underlying effect parameter
        /// </summary>
        internal EffectParameter FxParameter { get; private set; }

        /// <summary>
        /// Gets the name of the constant
        /// </summary>
        public string Name
        {
            get { return FxParameter.Name; }
        }

        internal abstract object UntypedValue { get; set; }

        #endregion
    }
}
