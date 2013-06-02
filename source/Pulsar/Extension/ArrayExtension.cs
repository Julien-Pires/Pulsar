using System;

namespace Pulsar.Extension
{
    public static class ArrayExtension
    {
        #region Methods

        public static T[] Add<T>(this T[] source, T item)
        {
            int newSize = source.Length + 1;
            T[] dest = new T[newSize];
            source.CopyTo(dest, 0);
            dest[source.Length] = item;

            return dest;
        }

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

        #endregion
    }
}
