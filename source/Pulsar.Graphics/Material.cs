using System;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Pulsar.Graphics.Fx;
using ContentReader = Pulsar.Assets.ContentReader;

namespace Pulsar.Graphics
{
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

        public event MaterialTechniqueChangedHandler TechniqueChanged;

        #endregion

        #region Constructors

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

        private static ushort GetId()
        {
            int id = IndexPool.Get();
            if(id > ushort.MaxValue)
                throw new Exception("");

            return (ushort)id;
        }

        internal static Material Read(Microsoft.Xna.Framework.Content.ContentReader input)
        {
            string name = input.ReadString();
            Material material = new Material(name);

            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string key = input.ReadString();
                Type type = Type.GetType(input.ReadString());
                object value = ContentReader.ReadObject(input, type);
                material.SetValue(key, value);
            }

            string shaderName = input.ReadString();
            string technique = input.ReadString();
            Shader shader = input.ContentManager.Load<Shader>(shaderName);
            material.BindShader(shader, technique);

            return material;
        }

        #endregion

        #region Methods

        public void BindShader(Shader shader, string technique = null)
        {
            if(shader == null)
                throw new ArgumentNullException("shader");

            if (string.IsNullOrWhiteSpace(technique))
                technique = shader.DefaultTechnique;

            _currentTechnique = new TechniqueBinding(shader, technique);
            IsTransparent = _currentTechnique.Definition.IsTransparent;
            BindConstantValue();

            OnTechniqueChanged(this, _currentTechnique.Definition);
        }

        private void BindConstantValue()
        {
            foreach (object value in _dataMap.Values)
            {
                IDictionary map = (IDictionary)value;
                foreach (DictionaryEntry entry in map)
                    _currentTechnique.TrySetConstantValue((string)entry.Key, entry.Value);
            }
        }

        private void OnTechniqueChanged(Material material, TechniqueDefinition definition)
        {
            MaterialTechniqueChangedHandler handler = TechniqueChanged;
            if (handler != null)
                handler(material, definition);
        }

        private void InitializeDataMap()
        {
            _dataMap.Add(typeof(float), new Dictionary<string, float>());
            _dataMap.Add(typeof(Texture), new Dictionary<string, Texture>());
            _dataMap.Add(typeof(Color), new Dictionary<string, Color>());
        }

        public void UnsafeSetValue<T>(string key, T value)
        {
            Dictionary<string, T> map = (Dictionary<string, T>)_dataMap[typeof(T)];
            map[key] = value;
        }

        private void SetValue(string key, object value)
        {
            IDictionary map = EnsureDictionary(value.GetType());
            map[key] = value;
        }

        public void SetValue<T>(string key, T value)
        {
            Dictionary<string, T> map = EnsureDictionary<T>();
            map[key] = value;
        }

        public T UnsafeGetValue<T>(string key)
        {
            Dictionary<string, T> map = (Dictionary<string, T>)_dataMap[typeof(T)];

            return map[key];
        }

        public T GetValue<T>(string key)
        {
            Dictionary<string, T> map = EnsureDictionary<T>();
            T result;

            return !map.TryGetValue(key, out result) ? default(T) : result;
        }

        private IDictionary EnsureDictionary(Type type)
        {
            MethodInfo methodInf = typeof (Material).GetMethod("EnsureDictionary", 
                BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            MethodInfo ensureMethod = methodInf.MakeGenericMethod(new[] {type});
            
            return (IDictionary)ensureMethod.Invoke(this, null);
        }

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

        public ushort Id
        {
            get { return _id; }
        }

        public string Name { get; private set; }

        public bool IsTransparent { get; private set; }

        public TechniqueBinding Technique
        {
            get { return _currentTechnique; }
        }

        public float Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                UnsafeSetValue(OpacityKey, value);
            }
        }

        public Color Diffuse
        {
            get { return _diffuse; }
            set
            {
                _diffuse = value;
                UnsafeSetValue(DiffuseKey, value);
            }
        }

        public Texture DiffuseMap
        {
            get { return _diffuseMap; }
            set
            {
                _diffuseMap = value;
                UnsafeSetValue(DiffuseMapKey, value);
            }
        }

        public Color Specular
        {
            get { return _specular; }
            set
            {
                _specular = value;
                UnsafeSetValue(SpecularKey, value);
            }
        }

        public Texture SpecularMap
        {
            get { return _specularMap; }
            set
            {
                _specularMap = value;
                UnsafeSetValue(SpecularMapKey, value);
            }
        }

        public float SpecularPower
        {
            get { return _specularPower; }
            set
            {
                _specularPower = value;
                UnsafeSetValue(SpecularPowerKey, value);
            }
        }

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
