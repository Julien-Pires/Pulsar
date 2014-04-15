using System;
using System.Collections.Generic;

namespace Pulsar
{
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

        public Pool(Func<T> create) : this(create, DefaultSize, Reset)
        {
        }

        public Pool(Func<T> create, int size) : this(create, size, Reset)
        {
        }

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

        private static void Reset(T item)
        {
        }

        #endregion

        #region Methods

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

        public void Clear()
        {
            _availableItems.Clear();
        }

        public void Reset()
        {
            while (_availableItems.Count > _startSize)
                _availableItems.Dequeue();

            for (int i = 0; i < _startSize; i++)
            {
                T item = _availableItems.Dequeue();
                _resetMethod(item);
                _availableItems.Enqueue(item);
            }
        }

        public T Get()
        {
            int count = _availableItems.Count;

            return (count > 0) ? _availableItems.Dequeue() : _createFunc();
        }

        public void Release(T item)
        {
            _resetMethod(item);
            _availableItems.Enqueue(item);
        }

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

        public int StartSize
        {
            get { return _startSize; }
            set
            {
                if(StartSize < 0)
                    throw new ArgumentException("", "value");

                _startSize = value;
            }
        }

        #endregion
    }
}
