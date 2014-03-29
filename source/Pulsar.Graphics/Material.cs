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

        private float _opacity;
        private Color _diffuse;
        private Texture _diffuseMap;
        private Color _specular;
        private Texture _specularMap;
        private float _specularPower;
        private Texture _normalMap;
        private readonly Dictionary<Type, object> _dataMap = new Dictionary<Type, object>();
        private ShaderTechniqueBinding _currentTechnique;

        #endregion

        #region Constructors

        public Material(string name)
        {
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

        internal static Material Read(Microsoft.Xna.Framework.Content.ContentReader input)
        {
            string name = input.ReadString();
            Material material = new Material(name);

            string shaderName = input.ReadString();
            string technique = input.ReadString();
            Shader shader = input.ContentManager.Load<Shader>(shaderName);
            material.BindShader(shader, technique);

            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string key = input.ReadString();
                Type type = Type.GetType(input.ReadString());
                object value = ContentReader.ReadObject(input, type);
                material.SetValue(key, value);
                material._currentTechnique.TrySetConstantValue(key, value);
            }

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

            _currentTechnique = new ShaderTechniqueBinding(shader, technique);
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

        public string Name { get; private set; }

        public bool IsTransparent
        {
            get { return _currentTechnique.IsTransparent; }
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
