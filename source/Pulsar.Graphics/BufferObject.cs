using System;

using Microsoft.Xna.Framework.Graphics;

using Pulsar.Extension;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Base class for all buffer objects
    /// This class encapsulate a vertex buffer or index buffer into wrapper to manipulate them as if
    /// they were exactly the same
    /// The size of the buffer is resized if necessary which can cause its destruction and, creates a
    /// new one with enough space to set all data
    /// </summary>
    public abstract class BufferObject : IDisposable
    {
        #region Fields

        private bool _disposed;

        #endregion

        #region Methods

        /// <summary>
        /// Disposes this buffer
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes this buffer
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            try
            {
                if (Wrapper != null)
                    Wrapper.Dispose();
            }
            finally
            {
                Wrapper = null;
                _disposed = true;
            }
        }

        /// <summary>
        /// Gets elements stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <returns>Returns an array containing all elements stored in the buffer</returns>
        public T[] GetData<T>() where T : struct
        {
            return GetData<T>(0, ElementCount);
        }

        /// <summary>
        /// Gets a range of elements stored in the buffer from a starting index
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of elements to get</param>
        /// <returns>Returns an array containing a range of elements in the buffer</returns>
        public T[] GetData<T>(int startIndex, int elementCount) where T : struct
        {
            return GetData<T>(0, startIndex, elementCount);
        }

        /// <summary>
        /// Gets a range of elements stored in the buffer from a starting offset and a starting index
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="bufferOffset">Offset in the buffer to the data</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of elements to get</param>
        /// <returns>Returns an array containing a range of elements in the buffer</returns>
        public T[] GetData<T>(int bufferOffset, int startIndex, int elementCount) where T : struct
        {
            T[] data = new T[elementCount];
            GetData(bufferOffset, data, startIndex, elementCount);

            return data;
        }

        /// <summary>
        /// Gets the elements stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="data">Array in which to store data</param>
        public void GetData<T>(T[] data) where T : struct
        {
            int elementCount = (data == null) ? 0 : data.Length;
            GetData(0, data, 0, elementCount);
        }

        /// <summary>
        /// Gets the elements stored in the buffer from a starting index
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="data">Array in which to store data</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of elements to get</param>
        public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct 
        {
            GetData(0, data, startIndex, elementCount);
        }

        /// <summary>
        /// Gets the elements stored in the buffer from a starting offset and a starting index
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="bufferOffset">Offset in the buffer to the data</param>
        /// <param name="data">Array in which to store data</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of elements to get</param>
        public void GetData<T>(int bufferOffset, T[] data, int startIndex, int elementCount) where T : struct
        {
            if (Wrapper == null) 
                return;

            Wrapper.GetData(bufferOffset, data, startIndex, elementCount);
        }

        /// <summary>
        /// Sets the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="data">Elements to add</param>
        public void SetData<T>(T[] data) where T : struct
        {
            SetData(data, 0, data.Length, SetDataOptions.None);
        }

        /// <summary>
        /// Sets the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            SetData(data, startIndex, elementCount, SetDataOptions.None);
        }

        /// <summary>
        /// Sets the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public void SetData<T>(T[] data, int startIndex, int elementCount, SetDataOptions option) where T : struct
        {
            SetData(0, data, startIndex, elementCount, option);
        }

        /// <summary>
        /// Sets the data stored in the buffer
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="bufferOffset">Offset in the buffer to the data</param>
        /// <param name="data">Elements to add</param>
        /// <param name="startIndex">Index of the first element</param>
        /// <param name="elementCount">Number of element to set</param>
        /// <param name="option">Settings option</param>
        public void SetData<T>(int bufferOffset, T[] data, int startIndex, int elementCount, SetDataOptions option)
            where T : struct
        {
            int setSize = ElementCount - bufferOffset;
            if (setSize < elementCount)
            {
                T[] oldData = new T[ElementCount];
                GetData(oldData);

                int newSize = ElementCount + (elementCount - setSize);
                CreateBuffer(newSize);
                Wrapper.SetData(0, oldData, 0, oldData.Length, SetDataOptions.None);
            }
            Wrapper.SetData(bufferOffset, data, startIndex, elementCount, option);
        }

        /// <summary>
        /// Adds elements at the end of the buffer
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="source">Elements to add</param>
        public void AddData<T>(T[] source) where T : struct
        {
            AddData(source, 0, source.Length);
        }

        /// <summary>
        /// Adds a range of elements at the end of the buffer
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="source">Elements to add</param>
        /// <param name="index">First element to add</param>
        /// <param name="elementCount">Number of elements</param>
        public void AddData<T>(T[] source, int index, int elementCount) where T : struct
        {
            SetData(ElementCount, source, index, elementCount, SetDataOptions.None);
        }

        /// <summary>
        /// Removes a range of elements from the buffer
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="offset">First element to remove</param>
        /// <param name="length">Number of elements to remove</param>
        public void RemoveData<T>(int offset, int length) where T : struct
        {
            RemoveData<T>(length, offset, false);
        }

        /// <summary>
        /// Removes a range of elements from the buffer and specifies if elements should be 
        /// only reseted or memory completely resized
        /// </summary>
        /// <typeparam name="T">Type of element stored in the buffer</typeparam>
        /// <param name="offset">First element to remove</param>
        /// <param name="length">Number of elements to remove</param>
        /// <param name="resetOnly">
        /// Indicates if elements should be reseted or completely destroyed by memory resizing 
        /// </param>
        public void RemoveData<T>(int offset, int length, bool resetOnly) where T : struct
        {
            if(length == 0) 
                return;

            if(length > ElementCount) 
                throw new ArgumentException("Length cannot be larger than element count", "length");

            if((offset < 0) || (offset >= ElementCount)) 
                throw new IndexOutOfRangeException("Offset out of range");

            if((offset + length) >= ElementCount) 
                throw new Exception("Length cannot go out of range");

            if (resetOnly)
            {
                T[] resetData = new T[length];
                for (int i = 0; i < length; i++) resetData[i] = default(T);
                SetData(offset, resetData, 0, length, SetDataOptions.None);
            }
            else
            {
                int remainingSize = ElementCount - length;
                if(remainingSize == 0) 
                    throw new Exception("Buffer cannot contains zero element");

                T[] bufferData = new T[ElementCount];
                GetData(bufferData);

                T[] newBufferData = new T[remainingSize];
                ArrayExtension.SlicedCopy(bufferData, offset, length, newBufferData);
                CreateBuffer(remainingSize);
                SetData(newBufferData);
            }
        }

        /// <summary>
        /// Creates a new hardware level buffer
        /// </summary>
        /// <param name="elementCount">Length of the new buffer</param>
        protected abstract void CreateBuffer(int elementCount);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the wrapper that encapsulate the underlaying buffer
        /// </summary>
        public abstract IBufferWrapper Wrapper { get; internal set; }

        /// <summary>
        /// Gets the number of elements in the buffer
        /// </summary>
        public int ElementCount
        {
            get { return Wrapper.ElementCount; }
        }

        #endregion
    }
}
 