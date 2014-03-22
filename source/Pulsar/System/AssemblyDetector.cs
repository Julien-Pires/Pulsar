using System;
using System.Reflection;
using System.Collections.Generic;

namespace Pulsar.System
{
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
                catch
                {
                }
            }
#endif
        }

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

        private bool CanInclude(string name)
        {
            if (IncludeAll)
                return true;

            return !IsSystem(name) && !IsExclude(name);
        }

        #endregion

        #region Properties

        public bool IncludeAll { get; set; }

        public List<string> ExcludeAssembly
        {
            get { return _excludeName; }
        }

        public List<string> ExcludeAssemblyBeginBy
        {
            get { return _excludeStartWith; }
        }

        #endregion
    }
}
