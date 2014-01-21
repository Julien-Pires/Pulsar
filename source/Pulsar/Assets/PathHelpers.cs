using System;

namespace Pulsar.Assets
{
    /// <summary>
    /// Provides methods to manipulate path
    /// </summary>
    internal static class PathHelpers
    {
        #region Methods

        /// <summary>
        /// Removes a directory root from the beginning of a path
        /// </summary>
        /// <param name="path">Path to clean</param>
        /// <param name="root">Directory to remove from the path</param>
        /// <returns>Returns a cleaned path</returns>
        public static string RemoveRoot(string path, string root)
        {
            path = path.Replace(root, string.Empty);
            while (path.StartsWith("\\"))
                path = path.Substring("\\".Length);

            return path;
        }

        /// <summary>
        /// Cleans a path to remove the first directory
        /// </summary>
        /// <param name="path">Path to clean</param>
        /// <returns>Returns a cleaned path</returns>
        public static string GetDirectoryPath(string path)
        {
            path = CleanPath(path);

            while (path.StartsWith("\\"))
                path = path.Substring("\\".Length);

            int index = path.IndexOf("\\", StringComparison.Ordinal);
            if (index < 1)
                return string.Empty;

            path = path.Substring(index);
            while (path.StartsWith("\\"))
                path = path.Substring("\\".Length);

            return path;
        }

        /// <summary>
        /// Cleans a path
        /// </summary>
        /// <param name="path">Path to clean</param>
        /// <returns>Returns a cleaned path</returns>
        public static string CleanPath(string path)
        {
            path = path.Replace('/', '\\');
            path = path.Replace("\\.\\", "\\");
            while (path.StartsWith(".\\"))
                path = path.Substring(".\\".Length);

            while (path.EndsWith("\\."))
            {
                path = (path.Length > "\\.".Length) ? path.Substring(0, path.Length - "\\.".Length) : "\\";
            }

            for (int i = 1; i < path.Length; i = CollapseParentDirectory(ref path, i, "\\..\\".Length))
            {
                i = path.IndexOf("\\..\\", i, StringComparison.Ordinal);
                if (i < 0)
                    break;
            }

            if (path.EndsWith("\\.."))
            {
                int i = path.Length - "\\..".Length;
                if (i > 0)
                    CollapseParentDirectory(ref path, i, "\\..".Length);
            }

            if (path == ".")
                path = string.Empty;

            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="position"></param>
        /// <param name="removeLength"></param>
        /// <returns></returns>
        public static int CollapseParentDirectory(ref string path, int position, int removeLength)
        {
            int num = path.LastIndexOf('\\', position - 1) + 1;
            path = path.Remove(num, position - num + removeLength);

            return Math.Max(num - 1, 1);
        }

        #endregion
    }
}
