using System.Collections.Generic;

using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Pulsar.Pipeline.Graphics
{
    public sealed class MaterialContent
    {
        #region Constructors

        public MaterialContent(string name, string shader, List<MaterialDataContent> datas)
        {
            Name = name;
            Datas = datas;
            Shader = shader;
        }

        #endregion

        #region Methods

        internal void Write(ContentWriter output)
        {
            output.Write(Name);
            output.Write(Datas.Count);
            for (int i = 0; i < Datas.Count; i++)
            {
                output.Write(Datas[i].Name);
                output.Write(Datas[i].Type.AssemblyQualifiedName);
                output.WriteObject(Datas[i].Value);
            }
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Shader { get; private set; }

        public List<MaterialDataContent> Datas { get; private set; }

        #endregion
    }
}
