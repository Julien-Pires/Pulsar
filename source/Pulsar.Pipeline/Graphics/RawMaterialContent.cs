using System.Collections.Generic;

namespace Pulsar.Pipeline.Graphics
{
    public sealed class RawMaterialContent
    {
        #region Fields

        private readonly List<RawMaterialDataContent> _data = new List<RawMaterialDataContent>();

        #endregion

        #region Constructors

        public RawMaterialContent(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public List<RawMaterialDataContent> Data
        {
            get { return _data; }
        }

        #endregion
    }
}
