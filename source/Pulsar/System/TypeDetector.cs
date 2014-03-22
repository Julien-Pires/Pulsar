using System;
using System.Reflection;
using System.Collections.Generic;

namespace Pulsar.System
{
    public sealed class TypeDetector
    {
        #region Fields

        private readonly AssemblyDetector _assemblyDetector = new AssemblyDetector();
        private readonly List<Type> _attributesList = new List<Type>();

        #endregion

        #region Methods

        public List<Type> GetTypes()
        {
            List<Assembly> assemblies = _assemblyDetector.GetAssemblies();
            List<Type> result = new List<Type>();
            for (int i = 0; i < assemblies.Count; i++)
            {
                Type[] types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; j++)
                {
                    Type currentType = types[j];
                    if (!HasAttributes(currentType) || IsExclude(currentType) || !IsAssignable(currentType))
                        continue;

                    result.Add(currentType);
                }
            }

            return result;
        }

        private bool IsAssignable(Type type)
        {
            if (BaseType == null)
                return true;

            if (BaseType.IsGenericTypeDefinition)
            {
                Type parent = type.BaseType;
                while ((parent != null) && (!parent.IsGenericType || (parent.GetGenericTypeDefinition() != BaseType)))
                    parent = parent.BaseType;

                return parent != null;
            }

            return BaseType.IsAssignableFrom(type);
        }

        private bool IsExclude(Type type)
        {
            TypeDetectorRule rule = Exclude;
            if (((rule & TypeDetectorRule.Class) == TypeDetectorRule.Class) && type.IsClass)
                return true;

            if (((rule & TypeDetectorRule.Interface) == TypeDetectorRule.Interface) && type.IsInterface)
                return true;

            if (((rule & TypeDetectorRule.ValueType) == TypeDetectorRule.ValueType) && type.IsValueType)
                return true;

            if (((rule & TypeDetectorRule.Public) == TypeDetectorRule.Public) && type.IsPublic)
                return true;

            if (((rule & TypeDetectorRule.Private) == TypeDetectorRule.Private) && type.IsNotPublic)
                return true;
#if WINDOWS
            if (((rule & TypeDetectorRule.Nested) == TypeDetectorRule.Nested) && type.IsNested)
                return true;
#endif
            if (((rule & TypeDetectorRule.Abstract) == TypeDetectorRule.Abstract) && type.IsAbstract)
                return true;

            if (((rule & TypeDetectorRule.Sealed) == TypeDetectorRule.Sealed) && type.IsSealed)
                return true;

            if (((rule & TypeDetectorRule.NoParameterLessCtor) == TypeDetectorRule.NoParameterLessCtor))
            {
                if (type.GetConstructor(Type.EmptyTypes) == null)
                    return true;
            }

            return false;
        }

        private bool HasAttributes(Type type)
        {
            bool result = true;
            for (int i = 0; i < _attributesList.Count; i++)
                result &= type.IsDefined(_attributesList[i], false);

            return result;
        }

        #endregion

        #region Properties

        public AssemblyDetector AssemblyDetector
        {
            get { return _assemblyDetector; }
        }

        public Type BaseType { get; set; }

        public List<Type> Attributes
        {
            get { return _attributesList; }
        }

        public TypeDetectorRule Exclude { get; set; }

        #endregion
    }
}
