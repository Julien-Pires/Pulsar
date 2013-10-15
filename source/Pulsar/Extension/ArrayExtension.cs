using System;

namespace Pulsar.Extension
{
    /// <summary>
    /// Contains extension methods for Array
    /// </summary>
    public static class ArrayExtension
    {
        #region Methods

        /// <summary>
        /// Adds a new item in an array
        /// </summary>
        /// <typeparam name="T">Type of item contained in the array</typeparam>
        /// <param name="source">Source array</param>
        /// <param name="item">Item to add</param>
        /// <returns>Returns a new array containing all items from source and the new item</returns>
        public static T[] Add<T>(this T[] source, T item)
        {
            int newSize = source.Length + 1;
            T[] dest = new T[newSize];
            source.CopyTo(dest, 0);
            dest[source.Length] = item;

            return dest;
        }

        /// <summary>
        /// Removes an item from at a specific index
        /// </summary>
        /// <typeparam name="T">Type of item contained in the array</typeparam>
        /// <param name="source">Source array</param>
        /// <param name="index">Index from which to remove the item</param>
        /// <returns>Returns a new array containing all items from source without the one specified</returns>
        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            int newSize = source.Length - 1;
            T[] dest = new T[newSize];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < newSize)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        /// <summary>
        /// Copies elements from a source array to a destination array by removing a range of elements specified
        /// by a starting offset and a length
        /// </summary>
        /// <typeparam name="T">Type of item contained in the array</typeparam>
        /// <param name="source">Source array</param>
        /// <param name="sliceIndex">Starting offset</param>
        /// <param name="sliceLength">Number of elements to remove during the copy</param>
        /// <param name="destination">Destination array</param>
        public static void SlicedCopy<T>(T[] source, int sliceIndex, int sliceLength, T[] destination)
        {
            if (destination == null) 
                throw new ArgumentNullException("destination");

            if ((sliceIndex < 0) || (sliceIndex > (source.Length - 1))) 
                throw new ArgumentOutOfRangeException("sliceIndex");

            if (sliceLength > source.Length) 
                throw new ArgumentException("Slice length cannot be greater than source length", "sliceLength");

            if (sliceLength < 0) 
                throw new ArgumentException("Slice length cannot be negative", "sliceLength");

            int nextIndex = sliceIndex + sliceLength;
            if (nextIndex > source.Length) 
                throw new IndexOutOfRangeException("Slice end index cannot be greater than source length");

            Array.Copy(source, 0, destination, 0, sliceIndex);
            Array.Copy(source, nextIndex, destination, sliceIndex, source.Length - nextIndex);
        }

        /// <summary>
        /// Copies elements from a source array to a destination array by removing a range of elements specified
        /// by a starting offset and a length
        /// </summary>
        /// <typeparam name="T">Type of item contained in the array</typeparam>
        /// <param name="source">Source array</param>
        /// <param name="sliceIndex">Starting offset</param>
        /// <param name="sliceLength">Number of elements to remove during the copy</param>
        /// <param name="destination">Destination array</param>
        public static void SlicedCopyTo<T>(this T[] source, int sliceIndex, int sliceLength, T[] destination)
        {
            SlicedCopy(source, sliceIndex, sliceLength, destination);
        }

        /// <summary>
        /// Merges two array into a destination array
        /// </summary>
        /// <typeparam name="T">Type of item contained in the array</typeparam>
        /// <param name="first">First array</param>
        /// <param name="second">Second array</param>
        /// <param name="destination">Destination array</param>
        public static void MergeTo<T>(this T[] first, T[] second, T[] destination)
        {
            Merge(first, second, destination);
        }

        /// <summary>
        /// Merges two array into a destination array
        /// </summary>
        /// <typeparam name="T">Type of item contained in the array</typeparam>
        /// <param name="first">First array</param>
        /// <param name="second">Second array</param>
        /// <param name="destination">Destination array</param>
        public static void Merge<T>(T[] first, T[] second, T[] destination)
        {
            int firstLength = first.Length;
            int secondLength = second.Length;
            Array.Copy(first, destination, first.Length);
            Array.Copy(second, 0, destination, firstLength, secondLength);
        }

        /// <summary>
        /// Replaces a range of elements from a source array and copies the new data in another array
        /// </summary>
        /// <typeparam name="T">Type of item contained in the array</typeparam>
        /// <param name="source">Source array</param>
        /// <param name="data">New data used to replace the old ones</param>
        /// <param name="offset">Starting offset of  to replace</param>
        /// <param name="length">Number of elements to replace</param>
        /// <param name="destination">Destination array</param>
        public static void ReplaceTo<T>(this T[] source, T[] data, int offset, int length, T[] destination)
        {
            Replace(source, data, offset, length, destination);
        }

        /// <summary>
        /// Replaces a range of elements from a source array and copies the new data in another array
        /// </summary>
        /// <typeparam name="T">Type of item contained in the array</typeparam>
        /// <param name="source">Source array</param>
        /// <param name="data">New data used to replace the old ones</param>
        /// <param name="offset">Starting offset of  to replace</param>
        /// <param name="length">Number of elements to replace</param>
        /// <param name="destination">Destination array</param>
        public static void Replace<T>(T[] source, T[] data, int offset, int length, T[] destination)
        {
            if(source == null) 
                throw new ArgumentNullException("source");

            if(data == null) 
                throw new ArgumentNullException("data");

            if(destination == null) 
                throw  new ArgumentNullException("destination");

            int dataLength = source.Length;
            int sourceLength = data.Length;
            if (length > dataLength) 
                throw new ArgumentException("length cannot be superior than data length", "length");

            if (length != sourceLength) 
                throw new ArgumentException("length cannot be superior than source length", "length");

            if(dataLength > destination.Length) 
                throw new ArgumentException("destination length is inferior to data length", "destination");

            if ((offset < 0) || (offset >= dataLength)) 
                throw new ArgumentOutOfRangeException("offset");

            if ((offset + length) - 1 >= dataLength) 
                throw new Exception("length cannot go out of range");

            int lastBlockOffset = (offset + sourceLength);
            Array.Copy(source, destination, offset);
            Array.Copy(data, 0, destination, offset, sourceLength);
            Array.Copy(source, lastBlockOffset, destination, lastBlockOffset, (dataLength - lastBlockOffset));
        }

        #endregion
    }
}
