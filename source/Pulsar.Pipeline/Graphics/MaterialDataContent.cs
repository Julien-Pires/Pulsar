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

        public object Value { get; set; }

        #endregion
    }
}
