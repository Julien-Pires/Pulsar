using System.Collections.Generic;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a collection of variable binding
    /// </summary>
    internal sealed class ShaderVariableBindingCollection : List<ShaderVariableBinding>
    {
        #region Constructors

        /// <summary>
        /// Constructor of ShaderVariableBindingCollection class
        /// </summary>
        internal ShaderVariableBindingCollection()
        {
        }

        /// <summary>
        /// Constructor of ShaderVariableBindingCollection class
        /// </summary>
        /// <param name="capacity">Initial capacity</param>
        internal ShaderVariableBindingCollection(int capacity) : base(capacity)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates all binding
        /// </summary>
        /// <param name="context">Frame context</param>
        internal void Update(FrameContext context)
        {
            for(int i = 0; i < Count; i++)
                this[i].Update(context);
        }

        /// <summary>
        /// Writes all binding
        /// </summary>
        internal void Write()
        {
            for(int i = 0; i < Count; i++)
                this[i].Write();
        }

        #endregion
    }
}
