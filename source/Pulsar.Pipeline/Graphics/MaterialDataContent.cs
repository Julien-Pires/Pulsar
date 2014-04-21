using System;

namespace Pulsar.Pipeline.Graphics
{
    public class MaterialDataContent
    {
        #region Constructors

        public MaterialDataContent(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public Type BuildType { get; internal set; }

        public Type RuntimeType { get; internal set; }

        public object Value { get; internal set; }

        #endregion
    }
}
