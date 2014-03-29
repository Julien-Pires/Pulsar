using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Extension;

namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Represents an effect composed of techniques to render an object
    /// </summary>
    public sealed class Shader : IDisposable
    {
        #region Fields

        private bool _isDisposed;
        private Effect _underlyingEffect;
        private string _fallback = string.Empty;
        private string _instancing = string.Empty;
        private string _default = string.Empty;
        private Dictionary<string, ShaderTechniqueDefinition> _techniquesMap
            = new Dictionary<string, ShaderTechniqueDefinition>();
        private Dictionary<string, ShaderConstantDefinition> _constantsMap
            = new Dictionary<string, ShaderConstantDefinition>();
        private Dictionary<int, List<ShaderConstantDefinition>> _constantsPerUpdate
            = new Dictionary<int, List<ShaderConstantDefinition>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Shader class
        /// </summary>
        /// <param name="effect">Effect</param>
        internal Shader(Effect effect)
        {
            Debug.Assert(effect != null);

            _underlyingEffect = effect;
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
                throw new Exception("Failed to find a graphics device service");

            if (graphicsDeviceService.GraphicsDevice == null)
                throw new Exception("Failed to find a graphics device");
            
            int length = input.ReadInt32();
            byte[] compiledEffect = input.ReadBytes(length);
            Effect fx = new Effect(graphicsDeviceService.GraphicsDevice, compiledEffect);
            Shader shader = new Shader(fx);
            shader.ReadTechniques(input);
            shader.ReadConstants(input);
            shader.CreateMissingTechniqueDefinition();
            shader.CreateMissingConstantDefinition();

            shader.Fallback = input.ReadString();
            shader.Instancing = input.ReadString();
            shader.DefaultTechnique = input.ReadString();

            return shader;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Releases all resources
        /// </summary>
        public void Dispose()
        {
            if(_isDisposed) return;

            try
            {
                GlobalConstantsBinding.Clear();
                _constantsMap.Clear();
                _constantsPerUpdate.Clear();
                _techniquesMap.Clear();

                _underlyingEffect.Dispose();
            }
            finally
            {
                GlobalConstantsBinding = null;
                _constantsMap = null;
                _constantsPerUpdate = null;
                _techniquesMap = null;

                _underlyingEffect = null;

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Creates missing defintion for techniques
        /// </summary>
        private void CreateMissingTechniqueDefinition()
        {
            for (int i = 0; i < _underlyingEffect.Techniques.Count; i++)
            {
                EffectTechnique technique = _underlyingEffect.Techniques[i];
                if (_techniquesMap.ContainsKey(technique.Name)) continue;

                ShaderTechniqueDefinition definition = new ShaderTechniqueDefinition(technique.Name, technique);
                _techniquesMap.Add(technique.Name, definition);
            }
        }

        /// <summary>
        /// Creates missing definition for constants
        /// </summary>
        private void CreateMissingConstantDefinition()
        {
            for (int i = 0; i < _underlyingEffect.Parameters.Count; i++)
            {
                EffectParameter parameter = _underlyingEffect.Parameters[i];
                if (_constantsMap.ContainsKey(parameter.Name)) continue;

                Type type = EffectParameterHelper.GetManagedType(parameter, string.Empty);
                ShaderConstantDefinition definition = new ShaderConstantDefinition(parameter.Name, parameter, type)
                {
                    UpdateFrequency = UpdateFrequency.Instance
                };

                if (!string.IsNullOrEmpty(parameter.Semantic))
                {
                    ShaderConstantSemantic semantic;
                    definition.Source = EnumExtension.TryParse(parameter.Semantic, true, out semantic) ?
                        ShaderConstantSource.Auto : ShaderConstantSource.Keyed;
                    definition.Semantic = parameter.Semantic;
                }
                else
                    definition.Source = ShaderConstantSource.Custom;

                _constantsMap.Add(parameter.Name, definition);

                List<ShaderConstantDefinition> list = EnsureConstantsList( definition.UpdateFrequency);
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
                EffectTechnique technique = _underlyingEffect.Techniques[name];
                if (technique == null)
                    throw new Exception(
                        string.Format("Shader definition contains a technique {0} but doesn't exist in effect", name));

                ShaderTechniqueDefinition definition = new ShaderTechniqueDefinition(name, technique)
                {
                    IsTransparent = input.ReadBoolean()
                };
                _techniquesMap.Add(name, definition);
            }
        }

        /// <summary>
        /// Reads constants definition from a binary input
        /// </summary>
        /// <param name="input">Input</param>
        internal void ReadConstants(ContentReader input)
        {
            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = input.ReadString();
                EffectParameter parameter = _underlyingEffect.Parameters[name];
                if (parameter == null)
                    throw new Exception(
                        string.Format("Shader definition contains a constant {0} but doesn't exist in effect", name));

                ShaderConstantSource source = (ShaderConstantSource) input.ReadInt32();
                UpdateFrequency update = (UpdateFrequency) input.ReadInt32();
                string semantic = input.ReadString();
                string equivalentType = input.ReadString();
                Type type = EffectParameterHelper.GetManagedType(parameter, equivalentType);
                ShaderConstantDefinition definition = new ShaderConstantDefinition(name, parameter, type)
                {
                    Source = source,
                    UpdateFrequency = update,
                    Semantic = semantic
                };
                _constantsMap.Add(name, definition);

                List<ShaderConstantDefinition> list = EnsureConstantsList(definition.UpdateFrequency);
                list.Add(definition);
            }

            GlobalConstantsBinding = CreateConstantsBinding(UpdateFrequency.Global);
        }

        /// <summary>
        /// Sets the current technique
        /// </summary>
        /// <param name="definition">Technique</param>
        internal void SetCurrentTechnique(ShaderTechniqueDefinition definition)
        {
            _underlyingEffect.CurrentTechnique = definition.Technique;
        }

        /// <summary>
        /// Creates a collection of constants binding for a specified update frequency
        /// </summary>
        /// <param name="usage">Update frequency</param>
        /// <returns>Returns a collection of constants binding</returns>
        internal ShaderConstantBindingCollection CreateConstantsBinding(UpdateFrequency usage)
        {
            List<ShaderConstantDefinition> constantList = EnsureConstantsList(usage);
            ShaderConstantBindingCollection bindingCollection = new ShaderConstantBindingCollection(constantList.Count);
            for (int i = 0; i < constantList.Count; i++)
            {
                ShaderConstantBinding binding = ShaderConstantBindingFactory.CreateBinding(constantList[i]);
                bindingCollection.Add(binding);
            }

            return bindingCollection;
        }

        /// <summary>
        /// Gets a collection of constant definition for a specific update frequency
        /// </summary>
        /// <param name="key">Update frequency</param>
        /// <returns>Returns a collection of constants definition</returns>
        private List<ShaderConstantDefinition> EnsureConstantsList(UpdateFrequency key)
        {
            List<ShaderConstantDefinition> list;
            if (_constantsPerUpdate.TryGetValue((int)key, out list)) 
                return list;

            list = new List<ShaderConstantDefinition>();
            _constantsPerUpdate.Add((int)key, list);

            return list;
        }

        /// <summary>
        /// Gets a shader constant definition
        /// </summary>
        /// <param name="name">Name of the constant</param>
        /// <returns>Returns a constant definition if found otherwise null</returns>
        public ShaderConstantDefinition GetConstantDefinition(string name)
        {
            ShaderConstantDefinition definition;

            return !_constantsMap.TryGetValue(name, out definition) ? null : definition;
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
        /// Gets the collection of constants binding used for global update
        /// </summary>
        public ShaderConstantBindingCollection GlobalConstantsBinding { get; private set; }

        public string DefaultTechnique
        {
            get { return _default; }
            set
            {
                if(!_techniquesMap.ContainsKey(value))
                    throw new Exception("");

                _default = value;
            }
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
                        throw new Exception(string.Format("Shader {0} does not have a technique {1}", _underlyingEffect.Name,
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
                        throw new Exception(string.Format("Shader {0} does not have a technique {1}", _underlyingEffect.Name,
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
