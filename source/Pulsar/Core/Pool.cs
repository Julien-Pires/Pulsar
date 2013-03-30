using System;

using System.Collections.Generic;

namespace Pulsar.Core
{
    /// <summary>
    /// Pool is used to reuse existing object to avoid constant instantiation
    /// This is useful to avoid GC working at each frame
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Pool<T> where T : new()
    {
        #region Nested

        /// <summary>
        /// Represents the index of an instance in the pool
        /// </summary>
        public class Accessor
        {
            #region Constructors

            /// <summary>
            /// Constructor of Accessor class
            /// </summary>
            internal Accessor()
            {
            }

            #endregion

            #region Properties

            /// <summary>
            /// Get the index of an object in the pool
            /// </summary>
            public int Index { get; internal set; }

            #endregion
        }

        #endregion

        #region Fields

        private int capacity = 4;
        private int used;
        private T[] elements;
        private Accessor[] indices;
        private Func<T> createFunction;
        private Action<T> initializeCallback;
        private Action<T> disposeCallback;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Pool class
        /// </summary>
        /// <param name="capacity">Starting capacity of the pool</param>
        /// <param name="createFunc">Function used to create a new instance</param>
        /// <param name="initCallback">Method used to initialize new instance</param>
        /// <param name="disposeCallback">Method used to dispose instance</param>
        public Pool(int capacity, Func<T> createFunc, Action<T> initCallback, Action<T> disposeCallback)
        {
            if (capacity >= 0)
            {
                this.capacity = capacity;
            }
            this.createFunction = createFunc;
            this.initializeCallback = initCallback;
            this.disposeCallback = disposeCallback;

            this.elements = new T[this.capacity];
            for (int i = 0; i < this.capacity; i++)
            {
                T instance;
                if (this.createFunction == null)
                {
                    instance = new T();
                }
                else
                {
                    instance = this.createFunction();
                }
                this.elements[i] = instance;

                if (this.initializeCallback != null)
                {
                    this.initializeCallback(instance);
                }
            }

            this.indices = new Accessor[this.capacity];
            for (int i = 0; i < this.capacity; i++)
            {
                this.indices[i] = new Accessor() { Index = i };
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear the pool
        /// </summary>
        public void Clear()
        {
            if (this.disposeCallback != null)
            {
                for (int i = 0; i < this.capacity; i++)
                {
                    this.disposeCallback(this.elements[i]);
                }
            }
            this.elements = null;

            for (int i = 0; i < this.capacity; i++)
            {
                this.indices = null;
            }
            this.indices = null;
        }

        /// <summary>
        /// Resize the pool
        /// </summary>
        /// <param name="newCapacity">New capacity of the pool</param>
        private void Resize(int newCapacity)
        {
            int oldCapacity = this.capacity;

            T[] newElements = new T[newCapacity];
            for (int i = 0; i < newCapacity; i++)
            {
                if (i < oldCapacity)
                {
                    newElements[i] = this.elements[i];
                }
                else
                {
                    T instance;
                    if (this.createFunction == null)
                    {
                        instance = new T();
                    }
                    else
                    {
                        instance = this.createFunction();
                    }
                    newElements[i] = instance;

                    if (this.initializeCallback != null)
                    {
                        this.initializeCallback(instance);
                    }
                }
            }
            this.elements = newElements;

            Accessor[] newIndices = new Accessor[newCapacity];
            for (int i = 0; i < newCapacity; i++)
            {
                if (i < oldCapacity)
                {
                    newIndices[i] = this.indices[i];
                }
                else
                {
                    newIndices[i] = new Accessor() { Index = i };
                }
            }
            this.indices = newIndices;

            this.capacity = newCapacity;
        }

        /// <summary>
        /// Get an index to a free instance in the pool
        /// </summary>
        /// <returns>Return an Accessor object containing an index to a free instance</returns>
        public Accessor Get()
        {
            if (this.used >= this.capacity)
            {
                this.Resize(this.capacity + 20);
            }
            this.used++;

            return this.indices[this.used - 1];
        }

        /// <summary>
        /// Released an instance in the pool
        /// </summary>
        /// <param name="access">Index of the instance to release</param>
        public void Release(Accessor access)
        {
            T releasedElement = this.elements[access.Index];
            this.elements[access.Index] = this.elements[this.used - 1];
            this.elements[this.used - 1] = releasedElement;

            int oldIndex = access.Index;
            access.Index = this.used - 1;
            this.indices[access.Index - 1].Index = oldIndex;

            Accessor lastActive = this.indices[this.used - 1];
            this.indices[this.used - 1] = access;
            this.indices[oldIndex] = lastActive;

            this.used--;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the array of elements, use Get() method to have a valid index to
        /// find a free instance
        /// </summary>
        public T[] Elements
        {
            get { return this.elements; }
        }

        #endregion
    }
}
