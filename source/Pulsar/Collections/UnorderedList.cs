using System;
using System.Collections;
using System.Collections.Generic;

namespace Pulsar.Collections
{
    /// <summary>
    /// Represents a strongly typed list that doesn't keep order
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    public class UnorderedList<T> : IList<T>, IList
    {
        #region Nested

        /// <summary>
        /// Enumerator for UnorderedList class
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            #region Fields

            private readonly UnorderedList<T> _list;
            private int _index;
            private T _current;

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor of Enumerator struct
            /// </summary>
            /// <param name="list">List to enumerates</param>
            internal Enumerator(UnorderedList<T> list)
            {
                _list = list;
                _index = 0;
                _current = default(T);
            }

            #endregion

            #region Methods

            /// <summary>
            /// Disposes resources
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// Resets enumerator
            /// </summary>
            public void Reset()
            {
                _current = default(T);
                _index = 0;
            }

            /// <summary>
            /// Moves to the next item
            /// </summary>
            /// <returns>Returns true if moved successfully, false if the end of the collection is reached</returns>
            public bool MoveNext()
            {
                if (_index < _list.Count)
                {
                    _current = _list[_index];
                    _index++;

                    return true;
                }

                _current = default(T);
                _index = _list.Count + 1;

                return false;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the current item
            /// </summary>
            object IEnumerator.Current
            {
                get { return Current; }
            }

            /// <summary>
            /// Gets the current item
            /// </summary>
            public T Current
            {
                get { return _current; }
            }

            #endregion
        }

        #endregion

        #region Fields

        private static readonly T[] Empty = new T[0];

        private readonly object _syncRoot = new object();
        private T[] _items;
        private int _count;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of UnorderedList class
        /// </summary>
        public UnorderedList() : this(32)
        {
        }

        /// <summary>
        /// Constructor of UnorderedList class
        /// </summary>
        /// <param name="capacity">Initial capacity</param>
        public UnorderedList(int capacity)
        {
            _items = new T[capacity];    
        }

        #endregion

        #region Operators

        /// <summary>
        /// Gets or sets the item at the specified index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Returns the item at the specified index</returns>
        public T this[int index]
        {
            get
            {
                if(index >= _count)
                    throw new ArgumentOutOfRangeException("index");

                return _items[index];
            }

            set { Set(index, value); }
        }

        /// <summary>
        /// Gets or sets the item at the specified index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Returns the item at the specified index</returns>
        object IList.this[int index]
        {
            get { return this[index]; }
            set
            {
                if(!IsValidType(value))
                    throw new ArgumentException(string.Format("Invalid item type, {0} item expected", typeof(T)));

                this[index] = (T) value;
            }
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Checks if the item is a valid type for the collection
        /// </summary>
        /// <param name="item">Item to checks</param>
        /// <returns>Return true if the type of the item is valid otherwise false</returns>
        internal static bool IsValidType(object item)
        {
            return (item is T) || ((item == null) && (default(T) == null));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets an enumerator for the collection
        /// </summary>
        /// <returns>Returns an enumerator</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Gets an enumerator for the collection
        /// </summary>
        /// <returns>Returns an enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for the collection
        /// </summary>
        /// <returns>Returns an enumerator</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Checks if the specified index is the range of the collection
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Returns true if the index is valid otherwise false</returns>
        public bool IsIndexWithinBounds(int index)
        {
            return (index >= 0) & (index < _count);
        }

        /// <summary>
        /// Checks if the collection contains a specified item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Returns true if the item is in the collection otherwise false</returns>
        bool IList.Contains(object item)
        {
            return IsValidType(item) && Contains((T) item);
        }

        /// <summary>
        /// Checks if the collection contains a specified item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Returns true if the item is in the collection otherwise false</returns>
        public bool Contains(T item)
        {
            if (item == null)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (_items[i] == null)
                        return true;
                }

                return false;
            }

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < _count; i++)
            {
                if (comparer.Equals(item, _items[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Clears the collection
        /// </summary>
        public void Clear()
        {
            if(_count > 0)
                Array.Clear(_items, 0, _count);

            _count = 0;
        }

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Returns the index of the item</returns>
        int IList.Add(object item)
        {
            if(!IsValidType(item))
                throw new ArgumentException(string.Format("Invalid item type, {0} item expected", typeof(T)));

            Add((T)item);

            return _count - 1;
        }

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="item">Item</param>
        public void Add(T item)
        {
            if (_count == _items.Length)
                Capacity = (_items.Length * 3) / 2 + 1;

            _items[_count++] = item;
        }

        /// <summary>
        /// Sets an item at a specified index
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="item">Item</param>
        public void Set(int index, T item)
        {
            if(index < 0)
                throw new ArgumentOutOfRangeException("index");

            if (index >= _items.Length)
                Capacity = ((index * 3) / 2 + 1);

            _count = Math.Max(_count, (index + 1));
            _items[index] = item;
        }

        /// <summary>
        /// Inserts an item at a specified index
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="item">Item</param>
        void IList.Insert(int index, object item)
        {
            if(!IsValidType(item))
                throw new ArgumentException(string.Format("Invalid item type, {0} item expected", typeof(T)));

            Insert(index, (T)item);
        }

        /// <summary>
        /// Inserts an item at a specified index
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="item">Item</param>
        public void Insert(int index, T item)
        {
            if(index > _count)
                throw new ArgumentOutOfRangeException("index");

            if (_count == _items.Length)
                Capacity = (_items.Length * 3) / 2 + 1;

            if(index < _count)
                Array.Copy(_items, index, _items, index + 1, (_count - index));

            _items[index] = item;
            _count++;
        }

        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="item">Item</param>
        void IList.Remove(object item)
        {
            if (IsValidType(item))
                Remove((T) item);
        }

        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Returns true if the item is removed otherwise false</returns>
        public bool Remove(T item)
        {
            int index = Array.IndexOf(_items, item, 0, _count);
            if (index == -1) 
                return false;

            RemoveAt(index);

            return true;
        }

        /// <summary>
        /// Removes an item at a specified index
        /// </summary>
        /// <param name="index">Index</param>
        public void RemoveAt(int index)
        {
            if(index < 0)
                throw new ArgumentOutOfRangeException("index");

            if(index >= _count)
                throw new ArgumentOutOfRangeException("index");

            _count--;
            _items[index] = _items[_count];
            _items[_count] = default(T);
        }

        /// <summary>
        /// Gets the index of a specified item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Returns the zero-based index if found otherwise -1</returns>
        int IList.IndexOf(object item)
        {
            return IsValidType(item) ? IndexOf((T) item) : -1;
        }

        /// <summary>
        /// Gets the index of a specified item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Returns the zero-based index if found otherwise -1</returns>
        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, 0, _count);
        }

        /// <summary>
        /// Copies the collection to an array with a starting index
        /// </summary>
        /// <param name="array">Destination array</param>
        /// <param name="index">Starting index</param>
        public void CopyTo(T[] array, int index)
        {
            Array.Copy(_items, index, array, index, _count);
        }

        /// <summary>
        /// Copies the collection to an array with a starting index
        /// </summary>
        /// <param name="array">Destination array</param>
        /// <param name="index">Starting index</param>
        void ICollection.CopyTo(Array array, int index)
        {
            if((array != null) && (array.Rank != 1))
                throw new ArgumentException("Incompatible array dimension");

            Array.Copy(_items, index, array, index, _count);
        }

        #endregion

        #region Fields

        /// <summary>
        /// Gets an object used to synchronize access to the collection
        /// </summary>
        object ICollection.SyncRoot
        {
            get { return _syncRoot; }
        }

        /// <summary>
        /// Gets a value that indicate if the collection has a fixed size
        /// </summary>
        bool IList.IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value that indicate if the collection is read only
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value that indicate if the collection is read only
        /// </summary>
        bool IList.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value that indicate if the access to the collection is synchronized
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the number of items
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Gets or set the capacity of the collection
        /// </summary>
        public int Capacity
        {
            get { return _items.Length; }
            set
            {
                if(value <= _items.Length) return;

                if (value > 0)
                {
                    T[] array = new T[value];
                    Array.Copy(_items, 0, array, 0, _items.Length);
                    _items = array;
                }
                else
                    _items = Empty;
            }
        }

        #endregion
    }
}