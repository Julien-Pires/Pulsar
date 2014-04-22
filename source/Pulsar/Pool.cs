using System;
using System.Collections.Generic;

namespace Pulsar
{
    /// <summary>
    /// Manages a pool of object
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    public sealed class Pool<T> : IDisposable where T : class
    {
        #region Fields

        private const int DefaultSize = 4;

        private bool _isDisposed;
        private Queue<T> _availableItems;
        private int _startSize;
        private readonly Func<T> _createFunc;
        private readonly Action<T> _resetMethod;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Pool class
        /// </summary>
        /// <param name="create">Method used to create item</param>
        public Pool(Func<T> create) : this(create, DefaultSize, Reset)
        {
        }

        /// <summary>
        /// Constructor of Pool class
        /// </summary>
        /// <param name="create">Method used to create item</param>
        /// <param name="size">Initial size of the pool</param>
        public Pool(Func<T> create, int size) : this(create, size, Reset)
        {
        }

        /// <summary>
        /// Constructor of Pool class
        /// </summary>
        /// <param name="create">Method used to create item</param>
        /// <param name="size">Initial size of the pool</param>
        /// <param name="reset">Method used to reset an item</param>
        public Pool(Func<T> create, int size, Action<T> reset)
        {
            StartSize = size;
            _createFunc = create;
            _resetMethod = reset;
            
            _availableItems = new Queue<T>(_startSize);
            for (int i = 0; i < _startSize; i++)
                _availableItems.Enqueue(_createFunc());
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Default reset method used if no one is provided
        /// </summary>
        /// <param name="item">Item</param>
        private static void Reset(T item)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed) 
                return;

            try
            {
                while(_availableItems.Count > 0)
                {
                    T item = _availableItems.Dequeue();
                    IDisposable disposable = item as IDisposable;
                    if(disposable != null)
                        disposable.Dispose();
                }
                _availableItems.Clear();
            }
            finally
            {
                _availableItems = null;
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Deletes all items in the pool
        /// </summary>
        public void Clear()
        {
            _availableItems.Clear();
        }

        /// <summary>
        /// Resets items in the pool
        /// </summary>
        /// <param name="full">Indicates if the pool should be reset to its initial size</param>
        public void Reset(bool full)
        {
            int count = _availableItems.Count;
            if (full)
            {
                count = _startSize;
                while (_availableItems.Count > _startSize)
                    _availableItems.Dequeue();
            }

            for (int i = 0; i < count; i++)
            {
                T item = _availableItems.Dequeue();
                _resetMethod(item);
                _availableItems.Enqueue(item);
            }
        }

        /// <summary>
        /// Gets an existing instance if one is available otherwise one will be created
        /// </summary>
        /// <returns>Returns an instance of T</returns>
        public T Get()
        {
            int count = _availableItems.Count;

            return (count > 0) ? _availableItems.Dequeue() : _createFunc();
        }

        /// <summary>
        /// Releases an item
        /// </summary>
        /// <param name="item">Item to release</param>
        public void Release(T item)
        {
            _resetMethod(item);
            _availableItems.Enqueue(item);
        }

        /// <summary>
        /// Releases a list of item
        /// </summary>
        /// <param name="items">List of items</param>
        public void Release(IList<T> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                _resetMethod(items[i]);
                _availableItems.Enqueue(items[i]);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the initial size of the pool
        /// </summary>
        public int StartSize
        {
            get { return _startSize; }
            set
            {
                if(StartSize < 0)
                    throw new ArgumentException("Invalid size, must be superior to zero", "value");

                _startSize = value;
            }
        }

        #endregion
    }
}
