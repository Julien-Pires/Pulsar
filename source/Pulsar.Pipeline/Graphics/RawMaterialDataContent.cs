using System.Collections.Generic;

namespace Pulsar.Pipeline.Graphics
{
    public sealed class RawMaterialDataContent
    {
        #region Fields

        private static readonly string[] Empty = new string[0];

        private readonly List<Dictionary<string, object>> _parameters = new List<Dictionary<string, object>>();

        #endregion

        #region Constructors

        public RawMaterialDataContent(string name)
        {
            Name = name;
            Type = string.Empty;
            Value = Empty;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public string Type { get; set; }

        public bool IsNativeArray { get; set; }

        public string[] Value { get; set; }

        public List<Dictionary<string, object>> Parameters
        {
            get { return _parameters; }
        }

        #endregion
    }
}
