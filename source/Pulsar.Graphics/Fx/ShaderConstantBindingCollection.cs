using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents a collection of constants binding
    /// </summary>
    public sealed class ShaderConstantBindingCollection
    {
        #region Fields

        private const int DefaultCapacity = 4;

        private readonly List<ShaderConstantBinding> _bindings;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ShaderConstantBindingCollection class
        /// </summary>
        internal ShaderConstantBindingCollection() : this(DefaultCapacity)
        {
        }

        /// <summary>
        /// Constructor of ShaderConstantBindingCollection class
        /// </summary>
        /// <param name="capacity">Initial capacity</param>
        internal ShaderConstantBindingCollection(int capacity)
        {
            _bindings = new List<ShaderConstantBinding>(capacity);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Gets a binding with a specified name
        /// </summary>
        /// <param name="name">Name of the constant</param>
        /// <returns>Returns a constant binding if found otherwise null</returns>
        public ShaderConstantBinding this[string name]
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
        /// <param name="name">Name of the constant</param>
        /// <returns>Returns a constant binding if found otherwise null</returns>
        public ShaderConstantBinding GetBinding(string name)
        {
            int idx = IndexOf(name);

            return (idx > -1) ? _bindings[idx] : null;
        }

        /// <summary>
        /// Gets a binding with a specified name
        /// </summary>
        /// <typeparam name="T">Binding type</typeparam>
        /// <param name="name">Name of the constant</param>
        /// <returns>Returns a constant binding if found otherwise null</returns>
        public T GetBinding<T>(string name) where T : ShaderConstantBinding
        {
            int idx = IndexOf(name);

            return (idx > -1) ? _bindings[idx] as T : null;
        }

        public bool TryGetBinding(string name, out ShaderConstantBinding binding)
        {
            binding = GetBinding(name);

            return binding != null;
        }

        public bool TryGetBinding<T>(string name, out T binding) where T : ShaderConstantBinding
        {
            binding = GetBinding<T>(name);

            return binding != null;
        }

        /// <summary>
        /// Gets the index of a binding with a specified name
        /// </summary>
        /// <param name="name">Name of the constant</param>
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
        /// <param name="binding">Constant binding</param>
        internal void Add(ShaderConstantBinding binding)
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

        internal void UpdateAndWrite(FrameContext context)
        {
            for (int i = 0; i < _bindings.Count; i++)
            {
                _bindings[i].Update(context);
                _bindings[i].Write();
            }
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

        internal bool AlreadyUpdated { get; set; }

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
