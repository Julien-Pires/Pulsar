using System;
using System.Reflection;
using System.Collections.Generic;

namespace Pulsar.System
{
    /// <summary>
    /// Provides mechanismes to search for specific types instance
    /// </summary>
    public sealed class TypeDetector
    {
        #region Fields

        private readonly AssemblyDetector _assemblyDetector = new AssemblyDetector();
        private readonly List<Type> _attributesList = new List<Type>();

        #endregion

        #region Methods

        /// <summary>
        /// Gets a list of types that match specified criteria
        /// </summary>
        /// <returns>Returns a list of types</returns>
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

        /// <summary>
        /// Checks if a type is assignable to the base type
        /// </summary>
        /// <param name="type">Type to checks</param>
        /// <returns>Returns true if it is assignable otherwise false</returns>
        private bool IsAssignable(Type type)
        {
            if (BaseType == null)
                return true;

            if (!BaseType.IsGenericTypeDefinition) 
                return BaseType.IsAssignableFrom(type);

            Type parent = type.BaseType;
            while ((parent != null) && (!parent.IsGenericType || (parent.GetGenericTypeDefinition() != BaseType)))
                parent = parent.BaseType;

            return parent != null;
        }

        /// <summary>
        /// Checks if a type is exclude from the search
        /// </summary>
        /// <param name="type">Type to checks</param>
        /// <returns>Returns true if the type is excluded otherwise false</returns>
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
                if (type.GetConstructor((BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    ,null, Type.EmptyTypes, null) == null)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a type has all specified attributes
        /// </summary>
        /// <param name="type">Type to checks</param>
        /// <returns>Returns true if the type has all attributes otherwise false</returns>
        private bool HasAttributes(Type type)
        {
            bool result = true;
            for (int i = 0; i < _attributesList.Count; i++)
                result &= type.IsDefined(_attributesList[i], false);

            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the assembly detector instance used for the search
        /// </summary>
        public AssemblyDetector AssemblyDetector
        {
            get { return _assemblyDetector; }
        }

        /// <summary>
        /// Gets or sets the base type that classes must inherit or implement
        /// </summary>
        public Type BaseType { get; set; }

        /// <summary>
        /// Gets the list of attributes that classes must have
        /// </summary>
        public List<Type> Attributes
        {
            get { return _attributesList; }
        }

        /// <summary>
        /// Gets or sets criteria to filter the search
        /// </summary>
        public TypeDetectorRule Exclude { get; set; }

        #endregion
    }
}
