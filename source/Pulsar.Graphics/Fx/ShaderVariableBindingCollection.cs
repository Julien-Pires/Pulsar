using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a collection of variable binding
    /// </summary>
    public sealed class ShaderVariableBindingCollection
    {
        #region Fields

        private const int DefaultCapacity = 4;

        private readonly List<ShaderVariableBinding> _bindings;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ShaderVariableBindingCollection class
        /// </summary>
        internal ShaderVariableBindingCollection() : this(DefaultCapacity)
        {
        }

        /// <summary>
        /// Constructor of ShaderVariableBindingCollection class
        /// </summary>
        /// <param name="capacity">Initial capacity</param>
        internal ShaderVariableBindingCollection(int capacity)
        {
            _bindings = new List<ShaderVariableBinding>(capacity);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Gets a binding with a specified name
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <returns>Returns a variable binding if found otherwise null</returns>
        public ShaderVariableBinding this[string name]
        {
            get
            {
                int idx = IndexOf(name);

                return (idx > -1) ? _bindings[idx] : null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a binding with a specified name
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <returns>Returns a variable binding if found otherwise null</returns>
        public ShaderVariableBinding GetBinding(string name)
        {
            int idx = IndexOf(name);

            return (idx > -1) ? _bindings[idx] : null;
        }

        /// <summary>
        /// Gets a binding with a specified name
        /// </summary>
        /// <typeparam name="T">Binding type</typeparam>
        /// <param name="name">Name of the variable</param>
        /// <returns>Returns a variable binding if found otherwise null</returns>
        public T GetBinding<T>(string name) where T : ShaderVariableBinding
        {
            int idx = IndexOf(name);

            return (idx > -1) ? _bindings[idx] as T : null;
        }

        /// <summary>
        /// Gets the index of binding with a specified name
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <returns>Returns a based zero index if found otherwise -1</returns>
        private int IndexOf(string name)
        {
            for (int i = 0; i < _bindings.Count; i++)
            {
                if (string.Equals(_bindings[i].Name, name, StringComparison.Ordinal))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Adds a binding to the collection
        /// </summary>
        /// <param name="binding">Variable binding</param>
        internal void Add(ShaderVariableBinding binding)
        {
            Debug.Assert(binding != null);

            _bindings.Add(binding);
        }

        /// <summary>
        /// Clears the collection
        /// </summary>
        internal void Clear()
        {
            _bindings.Clear();
        }

        /// <summary>
        /// Updates all binding
        /// </summary>
        /// <param name="context">Frame context</param>
        internal void Update(FrameContext context)
        {
            for (int i = 0; i < _bindings.Count; i++)
                _bindings[i].Update(context);
        }

        /// <summary>
        /// Writes all binding
        /// </summary>
        internal void Write()
        {
            for (int i = 0; i < _bindings.Count; i++)
                _bindings[i].Write();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of binding in the collection
        /// </summary>
        public int Count
        {
            get { return _bindings.Count; }
        }

        #endregion
    }
}
