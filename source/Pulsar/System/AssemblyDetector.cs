using System;
using System.Reflection;
using System.Collections.Generic;

namespace Pulsar.System
{
    /// <summary>
    /// Provides mechanismes to search for loaded assemblies
    /// </summary>
    public sealed class AssemblyDetector
    {
        #region Fields

        private static readonly string[] SystemsName =
        {
            "mscorlib",
            "System"
        };
        private static readonly string[] SystemsStartWith =
        {
            "System.",
            "Microsoft."
        };

        private readonly List<string> _excludeName = new List<string>();
        private readonly List<string> _excludeStartWith = new List<string>();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the list of loaded assemblies
        /// </summary>
        /// <returns>Returns a list of assemblies that are currently loaded</returns>
        public List<Assembly> GetAssemblies()
        {
            List<Assembly> result = new List<Assembly>();
#if WINDOWS
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                Assembly assembly = assemblies[i];
                if(!CanInclude(assembly.GetName().Name))
                    continue;

                LoadAssemblyAndReferences(assembly, result);
            }
#endif
            return result;
        }

        /// <summary>
        /// Loads an assembly and recursively load all its referenced assembly
        /// </summary>
        /// <param name="assembly">Assembly to load</param>
        /// <param name="loadedAssemblies">List of already loaded assemblies</param>
        private void LoadAssemblyAndReferences(Assembly assembly, List<Assembly> loadedAssemblies)
        {
            if (!loadedAssemblies.Contains(assembly))
                loadedAssemblies.Add(assembly);
#if WINDOWS
            AssemblyName[] refAssemblies = assembly.GetReferencedAssemblies();
            for (int i = 0; i < refAssemblies.Length; i++)
            {
                if (!CanInclude(refAssemblies[i].Name)) 
                    continue;

                try
                {
                    Assembly refAssembly = Assembly.Load(refAssemblies[i].Name);
                    if (loadedAssemblies.Contains(refAssembly)) 
                        continue;

                    LoadAssemblyAndReferences(refAssembly, loadedAssemblies);
                }
                catch (Exception)
                {
                }
            }
#endif
        }

        /// <summary>
        /// Checks if an assembly is a system assembly
        /// </summary>
        /// <param name="name">Name of the assembly</param>
        /// <returns>Returns true if the assembly is system otherwise false</returns>
        private bool IsSystem(string name)
        {
            for (int i = 0; i < SystemsName.Length; i++)
            {
                if (string.Equals(SystemsName[i], name, StringComparison.Ordinal))
                    return true;
            }

            for (int i = 0; i < SystemsStartWith.Length; i++)
            {
                if (name.StartsWith(SystemsStartWith[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if an assembly should be excluded from the search
        /// </summary>
        /// <param name="name">Name of the assembly</param>
        /// <returns>Returns true if the assembly should be excluded otherwise false</returns>
        private bool IsExclude(string name)
        {
            for (int i = 0; i < _excludeName.Count; i++)
            {
                if (string.Equals(_excludeName[i], name, StringComparison.Ordinal))
                    return true;
            }

            for (int i = 0; i < _excludeStartWith.Count; i++)
            {
                if (name.StartsWith(_excludeStartWith[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if an assembly can be included in the search
        /// </summary>
        /// <param name="name">Name of the assembly</param>
        /// <returns>Returns true if the assembly is included in the search otherwise false</returns>
        private bool CanInclude(string name)
        {
            if (IncludeAll)
                return true;

            return !IsSystem(name) && !IsExclude(name);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value that indicates all assemblies will be include in the search
        /// </summary>
        public bool IncludeAll { get; set; }

        /// <summary>
        /// Gets the list of excluded assemblies
        /// </summary>
        public List<string> ExcludeAssembly
        {
            get { return _excludeName; }
        }

        /// <summary>
        /// Gets a list that contains the beginning of an assembly name to exclude of the search
        /// </summary>
        public List<string> ExcludeAssemblyBeginBy
        {
            get { return _excludeStartWith; }
        }

        #endregion
    }
}
