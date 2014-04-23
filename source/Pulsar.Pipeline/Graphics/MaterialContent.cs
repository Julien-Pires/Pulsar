using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Pulsar.Pipeline.Graphics
{
    public sealed class MaterialContent
    {
        #region Constructors

        public MaterialContent(string name, List<MaterialDataContent> datas)
        {
            Name = name;
            Datas = datas;
        }

        #endregion

        #region Methods

        internal void Write(ContentWriter output)
        {
            output.Write(Name);

            output.Write(Datas.Count);
            for (int i = 0; i < Datas.Count; i++)
            {
                string fullTypeName = Datas[i].RuntimeType.AssemblyQualifiedName;
                if(fullTypeName == null)
                    throw new Exception("Failed to write data, type cannot be null");

                output.Write(Datas[i].Name);
                output.Write(fullTypeName);

                dynamic typedValue = Datas[i].Value;
                output.WriteRawObject(typedValue);
            }

            output.Write(Shader);
            output.Write(Technique);
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Shader { get; set; }

        public string Technique { get; set; }

        public List<MaterialDataContent> Datas { get; private set; }

        #endregion
    }
}
