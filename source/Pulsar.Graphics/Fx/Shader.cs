using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        private Dictionary<string, TechniqueDefinition> _techniquesMap;
        private Dictionary<string, ShaderConstantDefinition> _constantsMap;
        private Dictionary<int, List<ShaderConstantDefinition>> _constantsPerUpdate =
            new Dictionary<int, List<ShaderConstantDefinition>>();

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

            EffectTechniqueCollection techniques = effect.Techniques;
            _techniquesMap = new Dictionary<string, TechniqueDefinition>(techniques.Count);
            for (int i = 0; i < techniques.Count; i++)
            {
                TechniqueDefinition techniqueDefinition = new TechniqueDefinition(techniques[i]);
                _techniquesMap.Add(techniqueDefinition.Name, techniqueDefinition);
            }

            EffectParameterCollection constants = effect.Parameters;
            _constantsMap = new Dictionary<string, ShaderConstantDefinition>(constants.Count);
            for (int i = 0; i < constants.Count; i++)
            {
                EffectParameter effectConstant = constants[i];
                Type constantType = EffectParameterHelper.GetManagedType(effectConstant, null);
                ShaderConstantDefinition constantDefinition = new ShaderConstantDefinition(effectConstant, constantType)
                {
                    Semantic = effectConstant.Semantic,
                    UpdateFrequency = UpdateFrequency.Instance,
                    Source = ShaderConstantSource.Custom
                };
                _constantsMap.Add(constantDefinition.Name, constantDefinition);
            }
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
            IGraphicsDeviceService graphicsDeviceService =
                input.ContentManager.ServiceProvider.GetService(typeof (IGraphicsDeviceService)) as IGraphicsDeviceService;
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
            shader.Fallback = input.ReadString();
            shader.Instancing = input.ReadString();
            shader.DefaultTechnique = input.ReadString();

            foreach (ShaderConstantDefinition definition in shader._constantsMap.Values)
            {
                List<ShaderConstantDefinition> list = shader.EnsureConstantsList(definition.UpdateFrequency);
                list.Add(definition);
            }

            shader.GlobalConstantsBinding = shader.CreateConstantsBinding(UpdateFrequency.Global);
            shader.InstanceConstantsBinding = shader.CreateConstantsBinding(UpdateFrequency.Instance);

            return shader;
        }

        /// <summary>
        /// Reads a binary input to extract pass definition
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="definition">Existing instance</param>
        private static void ReadPass(ContentReader input, PassDefinition definition)
        {
            StateObject<RasterizerState> rasterizerState =
                        RenderState.GetRasterizerState((CullMode)input.ReadInt32(), (FillMode)input.ReadInt32());

            StateObject<DepthStencilState> depthStencilState =
                RenderState.GetDepthStencilState(input.ReadBoolean(), (CompareFunction)input.ReadInt32(),
                input.ReadInt32(), input.ReadInt32(), input.ReadInt32(), (CompareFunction)input.ReadInt32(),
                (StencilOperation)input.ReadInt32(), (StencilOperation)input.ReadInt32(),
                (StencilOperation)input.ReadInt32());

            StateObject<BlendState> blendState = RenderState.GetBlendState((BlendFunction)input.ReadInt32(),
                (BlendFunction)input.ReadInt32(), (Blend)input.ReadInt32(), (Blend)input.ReadInt32(),
                (Blend)input.ReadInt32(), (Blend)input.ReadInt32());

            RenderState renderState = RenderState.GetRenderState(rasterizerState, depthStencilState, blendState);
            definition.State = renderState;
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
                InstanceConstantsBinding.Clear();
                _constantsMap.Clear();
                _constantsPerUpdate.Clear();
                _techniquesMap.Clear();

                _underlyingEffect.Dispose();
            }
            finally
            {
                GlobalConstantsBinding = null;
                InstanceConstantsBinding = null;
                _constantsMap = null;
                _constantsPerUpdate = null;
                _techniquesMap = null;

                _underlyingEffect = null;

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Reads techniques definition from a binary input
        /// </summary>
        /// <param name="input">Input</param>
        private void ReadTechniques(ContentReader input)
        {
            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = input.ReadString();
                TechniqueDefinition technique;
                if (!_techniquesMap.TryGetValue(name, out technique))
                    throw new Exception(string.Format("Failed to load shader, {0} doesn't have a technique named {1}",
                        Name, name));

                technique.IsTransparent = input.ReadBoolean();

                int passesCount = input.ReadInt32();
                for (int j = 0; j < passesCount; j++)
                {
                    string passName = input.ReadString();
                    PassDefinition passDefinition = technique[passName];
                    if (passDefinition == null)
                        throw new Exception(string.Format("Failed to load shader, {0} doesn't have a pass named {1}",
                            Name, passName));

                    ReadPass(input, passDefinition);
                }
            }
        }

        /// <summary>
        /// Reads constants definition from a binary input
        /// </summary>
        /// <param name="input">Input</param>
        private void ReadConstants(ContentReader input)
        {
            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = input.ReadString();
                ShaderConstantDefinition constantDefinition;
                if (!_constantsMap.TryGetValue(name, out constantDefinition))
                    throw new Exception(string.Format("Failed to load shader, {0} doesn't have a constant named {1}",
                        Name, name));

                constantDefinition.Source = (ShaderConstantSource)input.ReadInt32();
                constantDefinition.UpdateFrequency = (UpdateFrequency)input.ReadInt32();
                constantDefinition.Semantic = input.ReadString();

                string equivalentType = input.ReadString();
                constantDefinition.Type = EffectParameterHelper.GetManagedType(constantDefinition.Parameter,
                    equivalentType);
            }
        }

        /// <summary>
        /// Sets the current technique
        /// </summary>
        /// <param name="definition">Technique</param>
        internal void SetCurrentTechnique(TechniqueDefinition definition)
        {
            _underlyingEffect.CurrentTechnique = definition.Technique;
        }

        /// <summary>
        /// Creates a collection of constants binding for a specified update frequency
        /// </summary>
        /// <param name="update">Update frequency</param>
        /// <returns>Returns a collection of constants binding</returns>
        internal ShaderConstantBindingCollection CreateConstantsBinding(UpdateFrequency update)
        {
            List<ShaderConstantDefinition> constantList = EnsureConstantsList(update);
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
        public TechniqueDefinition GetTechniqueDefinition(string name)
        {
            TechniqueDefinition technique;

            return !_techniquesMap.TryGetValue(name, out technique) ? null : technique;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the shader
        /// </summary>
        public string Name
        {
            get { return _underlyingEffect.Name; }
        }

        /// <summary>
        /// Gets the collection of constants binding used for global update
        /// </summary>
        public ShaderConstantBindingCollection GlobalConstantsBinding { get; private set; }

        /// <summary>
        /// Gets the collection of constants binding used for instance update
        /// </summary>
        public ShaderConstantBindingCollection InstanceConstantsBinding { get; private set; }

        /// <summary>
        /// Gets the name of the default technique
        /// </summary>
        public string DefaultTechnique
        {
            get { return _default; }
            internal set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (!_techniquesMap.ContainsKey(value))
                        throw new Exception(string.Format("Failed to set technique {0} as default", value));
                }

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

                TechniqueDefinition definition;
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

                TechniqueDefinition definition;
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
