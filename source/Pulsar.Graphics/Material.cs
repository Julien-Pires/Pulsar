using System;
using System.Reflection;
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
        private readonly Dictionary<Type, IMaterialDataCollection> _dataMap =
            new Dictionary<Type, IMaterialDataCollection>();
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
        internal Material(string name)
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
            NormalMap = null;
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
            foreach (IMaterialDataCollection collection in _dataMap.Values)
                collection.BindToTechnique(_currentTechnique);

            OnTechniqueChanged(_currentTechnique.Definition);
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
            _dataMap.Add(typeof(float), new MaterialDataCollection<float>());
            _dataMap.Add(typeof(Texture), new MaterialDataCollection<Texture>());
            _dataMap.Add(typeof(Color), new MaterialDataCollection<Color>());
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
            IMaterialDataCollection<T> collection = (IMaterialDataCollection<T>)_dataMap[typeof(T)];
            collection[key] = value;
        }

        /// <summary>
        /// Sets a value to this material
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void SetValue<T>(string key, T value)
        {
            IMaterialDataCollection<T> collection = EnsureDataCollection<T>();
            collection[key] = value;
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
            IMaterialDataCollection<T> collection = (IMaterialDataCollection<T>)_dataMap[typeof(T)];

            return collection[key];
        }

        /// <summary>
        /// Gets a value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Returns the data if found otherwise default value for T</returns>
        public T GetValue<T>(string key)
        {
            IMaterialDataCollection<T> collection = EnsureDataCollection<T>();
            T result;

            return !collection.TryGetValue(key, out result) ? default(T) : result;
        }

        /// <summary>
        /// Gets a map for a specified type
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>Returns a map</returns>
        private IMaterialDataCollection<T> EnsureDataCollection<T>()
        {
            Type type = typeof (T);
            IMaterialDataCollection value;
            if (_dataMap.TryGetValue(type, out value))
                return value as IMaterialDataCollection<T>;

            value = new MaterialDataCollection<T>();
            _dataMap.Add(type, value);

            return value as IMaterialDataCollection<T>;
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
            get { return UnsafeGetValue<float>(OpacityKey); }
            set { UnsafeSetValue(OpacityKey, value); }
        }

        /// <summary>
        /// Gets or sets the diffuse color
        /// </summary>
        public Color Diffuse
        {
            get { return UnsafeGetValue<Color>(DiffuseKey); }
            set { UnsafeSetValue(DiffuseKey, value); }
        }

        /// <summary>
        /// Gets or sets the diffuse texture
        /// </summary>
        public Texture DiffuseMap
        {
            get { return UnsafeGetValue<Texture>(DiffuseMapKey); }
            set { UnsafeSetValue(DiffuseMapKey, value); }
        }

        /// <summary>
        /// Gets or sets the specular color
        /// </summary>
        public Color Specular
        {
            get { return UnsafeGetValue<Color>(SpecularKey); }
            set { UnsafeSetValue(SpecularKey, value); }
        }

        /// <summary>
        /// Gets or sets the specular texture
        /// </summary>
        public Texture SpecularMap
        {
            get { return UnsafeGetValue<Texture>(SpecularMapKey); }
            set { UnsafeSetValue(SpecularMapKey, value); }
        }

        /// <summary>
        /// Gets or sets the specular power
        /// </summary>
        public float SpecularPower
        {
            get { return UnsafeGetValue<float>(SpecularPowerKey); }
            set { UnsafeSetValue(SpecularPowerKey, value); }
        }

        /// <summary>
        /// Gets or sets the normal texture
        /// </summary>
        public Texture NormalMap
        {
            get { return UnsafeGetValue<Texture>(NormalMapKey); }
            set { UnsafeSetValue(NormalMapKey, value); }
        }

        #endregion
    }
}
