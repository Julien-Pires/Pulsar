using System.Collections.Generic;

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

        #region Properties

        public string Name { get; private set; }

        public List<MaterialDataContent> Datas { get; private set; }

        #endregion
    }
}
