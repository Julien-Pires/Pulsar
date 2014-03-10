using System;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Fx
{
    public sealed partial class Shader
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
        private readonly ShaderVariableBindingCollection _globalVariables;

        #endregion

        #region Constructors

        internal Shader(Effect fx, List<ShaderVariableDefinition> variables, List<ShaderTechniqueDefinition> techniques,
            string fallback = null, string instancing = null)
        {
            Debug.Assert(fx != null);

            _underlyingFx = fx;

            for (int i = 0; i < techniques.Count; i++)
                _techniquesMap.Add(techniques[i].Name, techniques[i]);

            for (int i = 0; i < variables.Count; i++)
            {
                List<ShaderVariableDefinition> list = EnsureVariableList((int)variables[i].Usage);
                list.Add(variables[i]);
                _variablesMap.Add(variables[i].Name, variables[i]);
            }
            _globalVariables = CreateVariableBinding(ShaderVariableUsage.Global);

            Fallback = fallback;
            Instancing = instancing;
        }

        #endregion

        #region Methods

        internal void SetCurrentTechnique(ShaderTechniqueDefinition definition)
        {
            _underlyingFx.CurrentTechnique = definition.Technique;
        }

        internal ShaderVariableBindingCollection CreateVariableBinding(ShaderVariableUsage usage)
        {
            int key = (int) usage;
            List<ShaderVariableDefinition> variableList = EnsureVariableList(key);
            ShaderVariableBindingCollection bindingCollection = new ShaderVariableBindingCollection(variableList.Count);
            for (int i = 0; i < variableList.Count; i++)
            {
                ShaderVariableBinding binding = ShaderVariableBindingFactory.CreateBinding(variableList[i]);
                bindingCollection.Add(binding);
            }

            return bindingCollection;
        }

        private List<ShaderVariableDefinition> EnsureVariableList(int key)
        {
            List<ShaderVariableDefinition> list;
            if (_variablesPerUpdate.TryGetValue(key, out list)) return list;

            list = new List<ShaderVariableDefinition>();
            _variablesPerUpdate.Add(key, list);

            return list;
        }

        public ShaderVariableDefinition GetVariableDefinition(string name)
        {
            ShaderVariableDefinition definition;

            return !_variablesMap.TryGetValue(name, out definition) ? null : definition;
        }

        public ShaderTechniqueDefinition GetTechniqueDefinition(string name)
        {
            ShaderTechniqueDefinition technique;

            return !_techniquesMap.TryGetValue(name, out technique) ? null : technique;
        }

        #endregion

        #region Properties

        internal ShaderVariableBindingCollection GlobalVariablesBinding
        {
            get { return _globalVariables; }
        }

        public string Fallback
        {
            get { return _fallback; }
            internal set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!_techniquesMap.ContainsKey(value))
                        throw new Exception(string.Format("Shader {0} does not have a technique {1}", _underlyingFx.Name,
                            value));
                }
                else
                    value = string.Empty;

                _fallback = value;
            }
        }

        public string Instancing
        {
            get { return _instancing; }
            internal set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!_techniquesMap.ContainsKey(value))
                        throw new Exception(string.Format("Shader {0} does not have a technique {1}", _underlyingFx.Name,
                            value));
                }
                else
                    value = string.Empty;

                _instancing = value;
            }
        }

        #endregion
    }
}
