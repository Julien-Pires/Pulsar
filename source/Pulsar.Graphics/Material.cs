using System;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Pulsar.Graphics.Fx;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents a material used to render an object
    /// </summary>
    public sealed class Material
    {
        #region Fields

        private const string DiffuseKey = "Diffuse";
        private const string DiffuseMapKey = "DiffuseMap";
        private const string SpecularKey = "Specular";
        private const string SpecularMapKey = "SpecularMap";
        private const string SpecularPowerKey = "SpecularPower";
        private const string NormalMapKey = "NormalMap";
        private const string OpacityKey = "Opacity";

        private static readonly IndexPool IndexPool = new IndexPool();
        private static readonly Type[] ReadValueType = new Type[1];
        private static readonly object[] ReadValueArgs = new object[3];
        private static readonly MethodInfo ReadValueMethod = typeof (Material).GetMethod("ReadValue",
            (BindingFlags.Static | BindingFlags.NonPublic));

        private readonly ushort _id;
        private float _opacity;
        private Color _diffuse;
        private Texture _diffuseMap;
        private Color _specular;
        private Texture _specularMap;
        private float _specularPower;
        private Texture _normalMap;
        private readonly Dictionary<Type, object> _dataMap = new Dictionary<Type, object>();
        private TechniqueBinding _currentTechnique;

        #endregion

        #region Events

        /// <summary>
        /// Occurred when the used technique change
        /// </summary>
        public event MaterialTechniqueChangedHandler TechniqueChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Material class
        /// </summary>
        /// <param name="name">Name of the material</param>
        public Material(string name)
        {
            _id = GetId();
            Name = name;

            InitializeDataMap();

            Opacity = 1.0f;
            Diffuse = Color.White;
            DiffuseMap = null;
            Specular = Color.White;
            SpecularMap = null;
            SpecularPower = 50.0f;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Gets an id
        /// </summary>
        /// <returns>Returns an id</returns>
        private static ushort GetId()
        {
            int id = IndexPool.Get();
            if(id > ushort.MaxValue)
                throw new Exception("Maximum number of material reached");

            return (ushort)id;
        }

        /// <summary>
        /// Converts a binary input to a material
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Returns a Material instance</returns>
        internal static Material Read(ContentReader input)
        {
            string name = input.ReadString();
            Material material = new Material(name);

            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string key = input.ReadString();
                Type type = Type.GetType(input.ReadString());
                if (type == null)
                    throw new Exception(string.Format("Failed to load data {0} on material {1}, no type provided", key,
                        name));

                ReadValueType[0] = type;
                MethodInfo genericRead = ReadValueMethod.MakeGenericMethod(ReadValueType);

                ReadValueArgs[0] = input;
                ReadValueArgs[1] = material;
                ReadValueArgs[2] = key;
                genericRead.Invoke(null, ReadValueArgs);
            }

            string shaderName = input.ReadString();
            string technique = input.ReadString();
            Shader shader = input.ContentManager.Load<Shader>(shaderName);
            material.BindShader(shader, technique);

            return material;
        }

        private static void ReadValue<T>(ContentReader input, Material material, string key)
        {
            bool isExternalRef = input.ReadBoolean();
            T value = isExternalRef ? input.ReadExternalReference<T>() : input.ReadRawObject<T>();
            material.SetValue(key, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds a technique to this material
        /// </summary>
        /// <param name="shader">Shader that has the technique</param>
        /// <param name="technique">Technique to use</param>
        public void BindShader(Shader shader, string technique = null)
        {
            if(shader == null)
                throw new ArgumentNullException("shader");

            if (string.IsNullOrWhiteSpace(technique))
                technique = shader.DefaultTechnique;

            _currentTechnique = new TechniqueBinding(shader, technique);
            IsTransparent = _currentTechnique.Definition.IsTransparent;
            BindConstantValue();

            OnTechniqueChanged(_currentTechnique.Definition);
        }

        /// <summary>
        /// Binds the shader constant to this material
        /// </summary>
        private void BindConstantValue()
        {
            foreach (object value in _dataMap.Values)
            {
                IDictionary map = (IDictionary)value;
                foreach (DictionaryEntry entry in map)
                    _currentTechnique.TrySetConstantValue((string)entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Called when the technique change
        /// </summary>
        /// <param name="definition">New technique definition</param>
        private void OnTechniqueChanged(TechniqueDefinition definition)
        {
            MaterialTechniqueChangedHandler handler = TechniqueChanged;
            if (handler != null)
                handler(this, definition);
        }

        /// <summary>
        /// Initialize maps of data
        /// </summary>
        private void InitializeDataMap()
        {
            _dataMap.Add(typeof(float), new Dictionary<string, float>());
            _dataMap.Add(typeof(Texture), new Dictionary<string, Texture>());
            _dataMap.Add(typeof(Color), new Dictionary<string, Color>());
        }

        /// <summary>
        /// Sets a value to this material
        /// </summary>
        /// <remarks>Can be unsafe if map for type T doesn't exist</remarks>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void UnsafeSetValue<T>(string key, T value)
        {
            Dictionary<string, T> map = (Dictionary<string, T>)_dataMap[typeof(T)];
            map[key] = value;
        }

        /// <summary>
        /// Sets a value to this material
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void SetValue<T>(string key, T value)
        {
            Dictionary<string, T> map = EnsureDictionary<T>();
            map[key] = value;
        }

        /// <summary>
        /// Gets a value
        /// </summary>
        /// <remarks>Can be unsafe if map for type T doesn't exist</remarks>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Returns the data</returns>
        public T UnsafeGetValue<T>(string key)
        {
            Dictionary<string, T> map = (Dictionary<string, T>)_dataMap[typeof(T)];

            return map[key];
        }

        /// <summary>
        /// Gets a value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Returns the data if found otherwise default value for T</returns>
        public T GetValue<T>(string key)
        {
            Dictionary<string, T> map = EnsureDictionary<T>();
            T result;

            return !map.TryGetValue(key, out result) ? default(T) : result;
        }

        /// <summary>
        /// Gets a map for a specified type
        /// </summary>
        /// <param name="type">Data type</param>
        /// <returns>Returns a map</returns>
        private IDictionary EnsureDictionary(Type type)
        {
            MethodInfo methodInf = typeof (Material).GetMethod("EnsureDictionary", 
                BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            MethodInfo ensureMethod = methodInf.MakeGenericMethod(new[] {type});
            
            return (IDictionary)ensureMethod.Invoke(this, null);
        }

        /// <summary>
        /// Gets a map for a specified type
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>Returns a map</returns>
        private Dictionary<string, T> EnsureDictionary<T>()
        {
            Type type = typeof (T);
            object value;
            if (!_dataMap.TryGetValue(type, out value))
            {
                value = new Dictionary<string, T>();
                _dataMap.Add(type, value);
            }

            Dictionary<string, T> typedDictionary = value as Dictionary<string, T>;
            Debug.Assert(typedDictionary != null);

            return typedDictionary;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the id of the material
        /// </summary>
        public ushort Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Gets the name of the material
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value that indicate if this material use transparency
        /// </summary>
        public bool IsTransparent { get; private set; }

        /// <summary>
        /// Gets the technique used by this material
        /// </summary>
        public TechniqueBinding Technique
        {
            get { return _currentTechnique; }
        }

        /// <summary>
        /// Gets or sets the opacity
        /// </summary>
        public float Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                UnsafeSetValue(OpacityKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the diffuse color
        /// </summary>
        public Color Diffuse
        {
            get { return _diffuse; }
            set
            {
                _diffuse = value;
                UnsafeSetValue(DiffuseKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the diffuse texture
        /// </summary>
        public Texture DiffuseMap
        {
            get { return _diffuseMap; }
            set
            {
                _diffuseMap = value;
                UnsafeSetValue(DiffuseMapKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the specular color
        /// </summary>
        public Color Specular
        {
            get { return _specular; }
            set
            {
                _specular = value;
                UnsafeSetValue(SpecularKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the specular texture
        /// </summary>
        public Texture SpecularMap
        {
            get { return _specularMap; }
            set
            {
                _specularMap = value;
                UnsafeSetValue(SpecularMapKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the specular power
        /// </summary>
        public float SpecularPower
        {
            get { return _specularPower; }
            set
            {
                _specularPower = value;
                UnsafeSetValue(SpecularPowerKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the normal texture
        /// </summary>
        public Texture NormalMap
        {
            get { return _normalMap; }
            set
            {
                _normalMap = value;
                UnsafeSetValue(NormalMapKey, value);
            }
        }

        #endregion
    }
}
