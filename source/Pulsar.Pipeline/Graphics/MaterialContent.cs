using System.Collections.Generic;

namespace Pulsar.Pipeline.Graphics
{
    public sealed class MaterialContent
    {
        #region Fields

        private readonly List<MaterialDataContent> _data = new List<MaterialDataContent>();

        #endregion

        #region Constructors

        public MaterialContent(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public List<MaterialDataContent> Data
        {
            get { return _data; }
        }

        #endregion
    }
}
