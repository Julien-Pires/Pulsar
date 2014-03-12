using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Extension;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents an effect composed of techniques to render an object
    /// </summary>
    public sealed class Shader
    {
        #region Fields

        private readonly Effect _underlyingFx;
        private string _fallback = string.Empty;
        private string _instancing = string.Empty;
        private readonly Dictionary<string, ShaderTechniqueDefinition> _techniquesMap
            = new Dictionary<string, ShaderTechniqueDefinition>();
        private readonly Dictionary<string, ShaderVariableDefinition> _variablesMap
            = new Dictionary<string, ShaderVariableDefinition>();
        private readonly Dictionary<int, List<ShaderVariableDefinition>> _variablesPerUpdate
            = new Dictionary<int, List<ShaderVariableDefinition>>();
        private ShaderVariableBindingCollection _globalVariables;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Shader class
        /// </summary>
        /// <param name="fx">Effect</param>
        internal Shader(Effect fx)
        {
            Debug.Assert(fx != null);

            _underlyingFx = fx;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Converts a binary input to a shader instance
        /// </summary>
        /// <param name="input">Input</param>
        /// <returns>Returns a shader instance</returns>
        internal static Shader Read(ContentReader input)
        {
            IGraphicsDeviceService graphicsDeviceService = input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService))
                as IGraphicsDeviceService;
            if (graphicsDeviceService == null)
                throw new Exception("");
            if (graphicsDeviceService.GraphicsDevice == null)
                throw new Exception("");

            int length = input.ReadInt32();
            byte[] compiledEffect = input.ReadBytes(length);
            Effect fx = new Effect(graphicsDeviceService.GraphicsDevice, compiledEffect);
            Shader shader = new Shader(fx);
            shader.ReadTechniques(input);
            shader.ReadVariables(input);
            shader.CreateMissingTechniqueDefinition();
            shader.CreateMissingVariableDefinition();
            shader.Fallback = input.ReadString();
            shader.Instancing = input.ReadString();

            return shader;
        }

        /// <summary>
        /// Gets the managed type from an effect parameter
        /// </summary>
        /// <param name="parameter">Effect parameter</param>
        /// <returns>Returns an instance of Type class that corresponds to the parameter</returns>
        private static Type ExtractVariableType(EffectParameter parameter)
        {
            Type result = null;
            switch (parameter.ParameterClass)
            {
                case EffectParameterClass.Matrix:
                    result = typeof(Matrix);
                    break;

                case EffectParameterClass.Object:
                    result = GetObjectType(parameter);
                    break;

                case EffectParameterClass.Scalar:
                    result = GetScalarType(parameter);
                    break;

                case EffectParameterClass.Vector:
                    result = GetVectorType(parameter);
                    break;
            }

            if (result == null)
                throw new Exception("Failed to load shader, unsupported parameter type detected");

            return result;
        }

        /// <summary>
        /// Gets the vector type from an effect parameter
        /// </summary>
        /// <param name="parameter">Effect parameter</param>
        /// <returns>Returns an instance of Type class that corresponds to the parameter</returns>
        private static Type GetVectorType(EffectParameter parameter)
        {
            Type result = null;
            switch (parameter.ColumnCount)
            {
                case 2:
                    result = typeof(Vector2);
                    break;

                case 3:
                    result = typeof(Vector3);
                    break;

                case 4:
                    result = typeof(Vector4);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the object type from an effect parameter
        /// </summary>
        /// <param name="parameter">Effect parameter</param>
        /// <returns>Returns an instance of Type class that corresponds to the parameter</returns>
        private static Type GetObjectType(EffectParameter parameter)
        {
            Type result = null;
            if (parameter.Elements.Count == 0)
            {
                switch (parameter.ParameterType)
                {
                    case EffectParameterType.Texture:
                        result = typeof(Texture);
                        break;

                    case EffectParameterType.Texture2D:
                        result = typeof(Texture2D);
                        break;

                    case EffectParameterType.Texture3D:
                        result = typeof(Texture3D);
                        break;

                    case EffectParameterType.String:
                        result = typeof(string);
                        break;
                }
            }
            else
            {
                switch (parameter.ParameterType)
                {
                    case EffectParameterType.Texture:
                        result = typeof(Texture[]);
                        break;

                    case EffectParameterType.Texture2D:
                        result = typeof(Texture2D[]);
                        break;

                    case EffectParameterType.Texture3D:
                        result = typeof(Texture3D[]);
                        break;

                    case EffectParameterType.String:
                        result = typeof(string[]);
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the scalar type from an effect parameter
        /// </summary>
        /// <param name="parameter">Effect parameter</param>
        /// <returns>Returns an instance of Type class that corresponds to the parameter</returns>
        private static Type GetScalarType(EffectParameter parameter)
        {
            Type result = null;
            if (parameter.Elements.Count == 0)
            {
                switch (parameter.ParameterType)
                {
                    case EffectParameterType.Bool:
                        result = typeof(bool);
                        break;

                    case EffectParameterType.Int32:
                        result = typeof(int);
                        break;

                    case EffectParameterType.Single:
                        result = typeof(float);
                        break;
                }
            }
            else
            {
                switch (parameter.ParameterType)
                {
                    case EffectParameterType.Bool:
                        result = typeof(bool[]);
                        break;

                    case EffectParameterType.Int32:
                        result = typeof(int[]);
                        break;

                    case EffectParameterType.Single:
                        result = typeof(float[]);
                        break;
                }
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates missing defintion for techniques
        /// </summary>
        private void CreateMissingTechniqueDefinition()
        {
            for (int i = 0; i < _underlyingFx.Techniques.Count; i++)
            {
                EffectTechnique technique = _underlyingFx.Techniques[i];
                if (_techniquesMap.ContainsKey(technique.Name)) continue;

                ShaderTechniqueDefinition definition = new ShaderTechniqueDefinition(technique.Name, technique);
                _techniquesMap.Add(technique.Name, definition);
            }
        }

        /// <summary>
        /// Creates missing definition for variables
        /// </summary>
        private void CreateMissingVariableDefinition()
        {
            for (int i = 0; i < _underlyingFx.Parameters.Count; i++)
            {
                EffectParameter parameter = _underlyingFx.Parameters[i];
                if (_variablesMap.ContainsKey(parameter.Name)) continue;

                Type type = ExtractVariableType(parameter);
                ShaderVariableDefinition definition = new ShaderVariableDefinition(parameter.Name, parameter, type)
                {
                    Usage = ShaderVariableUsage.Instance
                };

                if (!string.IsNullOrEmpty(parameter.Semantic))
                {
                    ShaderVariableSemantic semantic;
                    definition.Source = EnumExtension.TryParse(parameter.Semantic, true, out semantic) ?
                        ShaderVariableSource.Auto : ShaderVariableSource.Keyed;
                    definition.Semantic = parameter.Semantic;
                }
                else
                    definition.Source = ShaderVariableSource.Custom;

                List<ShaderVariableDefinition> list = EnsureVariableList( definition.Usage);
                list.Add(definition);
            }
        }

        /// <summary>
        /// Reads techniques definition from a binary input
        /// </summary>
        /// <param name="input">Input</param>
        internal void ReadTechniques(ContentReader input)
        {
            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = input.ReadString();
                EffectTechnique technique = _underlyingFx.Techniques[name];
                ShaderTechniqueDefinition definition = new ShaderTechniqueDefinition(name, technique)
                {
                    IsTransparent = input.ReadBoolean()
                };
                _techniquesMap.Add(name, definition);
            }
        }

        /// <summary>
        /// Reads variables definition from a binary input
        /// </summary>
        /// <param name="input">Input</param>
        internal void ReadVariables(ContentReader input)
        {
            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = input.ReadString();
                EffectParameter parameter = _underlyingFx.Parameters[name];
                Type type = ExtractVariableType(parameter);
                ShaderVariableDefinition definition = new ShaderVariableDefinition(name, parameter, type)
                {
                    Source = (ShaderVariableSource)input.ReadInt32(),
                    Usage = (ShaderVariableUsage)input.ReadInt32(),
                    Semantic = input.ReadString()
                };
                _variablesMap.Add(name, definition);

                List<ShaderVariableDefinition> list = EnsureVariableList(definition.Usage);
                list.Add(definition);
            }

            _globalVariables = CreateVariableBinding(ShaderVariableUsage.Global);
        }

        /// <summary>
        /// Sets the current technique
        /// </summary>
        /// <param name="definition">Technique</param>
        internal void SetCurrentTechnique(ShaderTechniqueDefinition definition)
        {
            _underlyingFx.CurrentTechnique = definition.Technique;
        }

        /// <summary>
        /// Creates a collection of variables binding for a specified update frequency
        /// </summary>
        /// <param name="usage">Update frequency</param>
        /// <returns>Returns a collection of variables binding</returns>
        internal ShaderVariableBindingCollection CreateVariableBinding(ShaderVariableUsage usage)
        {
            List<ShaderVariableDefinition> variableList = EnsureVariableList(usage);
            ShaderVariableBindingCollection bindingCollection = new ShaderVariableBindingCollection(variableList.Count);
            for (int i = 0; i < variableList.Count; i++)
            {
                ShaderVariableBinding binding = ShaderVariableBindingFactory.CreateBinding(variableList[i]);
                bindingCollection.Add(binding);
            }

            return bindingCollection;
        }

        /// <summary>
        /// Gets a collection of variable definition for a specific update frequency
        /// </summary>
        /// <param name="key">Update frequency</param>
        /// <returns>Returns a collection of variables definition</returns>
        private List<ShaderVariableDefinition> EnsureVariableList(ShaderVariableUsage key)
        {
            List<ShaderVariableDefinition> list;
            if (_variablesPerUpdate.TryGetValue((int)key, out list)) 
                return list;

            list = new List<ShaderVariableDefinition>();
            _variablesPerUpdate.Add((int)key, list);

            return list;
        }

        /// <summary>
        /// Gets a shader variable definition
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <returns>Returns a variable definition if found otherwise null</returns>
        public ShaderVariableDefinition GetVariableDefinition(string name)
        {
            ShaderVariableDefinition definition;

            return !_variablesMap.TryGetValue(name, out definition) ? null : definition;
        }

        /// <summary>
        /// Gets a shader technique definition
        /// </summary>
        /// <param name="name">Name of the technique</param>
        /// <returns>Returns a technique definition if found otherwise null</returns>
        public ShaderTechniqueDefinition GetTechniqueDefinition(string name)
        {
            ShaderTechniqueDefinition technique;

            return !_techniquesMap.TryGetValue(name, out technique) ? null : technique;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of variable binding used for global update
        /// </summary>
        internal ShaderVariableBindingCollection GlobalVariablesBinding
        {
            get { return _globalVariables; }
        }

        /// <summary>
        /// Gets the name of the technique used as fallback
        /// </summary>
        public string Fallback
        {
            get { return _fallback; }
            internal set
            {
                if(string.Equals(_fallback, value, StringComparison.OrdinalIgnoreCase))
                    return;

                ShaderTechniqueDefinition definition;
                if (_techniquesMap.TryGetValue(_fallback, out definition))
                    definition.IsFallback = false;

                if (!string.IsNullOrEmpty(value))
                {
                    if (!_techniquesMap.TryGetValue(value, out definition))
                        throw new Exception(string.Format("Shader {0} does not have a technique {1}", _underlyingFx.Name,
                            value));

                    definition.IsFallback = true;
                }
                else
                    value = string.Empty;

                _fallback = value;
            }
        }

        /// <summary>
        /// Gets the name of the technique used for instancing
        /// </summary>
        public string Instancing
        {
            get { return _instancing; }
            internal set
            {
                if (string.Equals(_instancing, value, StringComparison.OrdinalIgnoreCase))
                    return;

                ShaderTechniqueDefinition definition;
                if (_techniquesMap.TryGetValue(_instancing, out definition))
                    definition.IsInstancing = false;

                if (!string.IsNullOrEmpty(value))
                {
                    if (!_techniquesMap.TryGetValue(value, out definition))
                        throw new Exception(string.Format("Shader {0} does not have a technique {1}", _underlyingFx.Name,
                            value));

                    definition.IsInstancing = true;
                }
                else
                    value = string.Empty;

                _instancing = value;
            }
        }

        #endregion
    }
}
