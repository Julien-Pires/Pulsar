using System;
using System.Collections.Generic;

namespace Pulsar.Pipeline.Graphics
{
    public sealed class RawMaterialContent
    {
        #region Fields

        private readonly List<RawMaterialDataContent> _data = new List<RawMaterialDataContent>();

        #endregion

        #region Constructors

        public RawMaterialContent(string name, string shader, string technique)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            if(string.IsNullOrWhiteSpace(shader))
                throw new ArgumentNullException("shader");

            Name = name;
            Shader = shader;
            Technique = technique;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Shader { get; private set; }

        public string Technique { get; private set; }

        public List<RawMaterialDataContent> Data
        {
            get { return _data; }
        }

        #endregion
    }
}
