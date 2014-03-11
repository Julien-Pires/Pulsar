namespace Pulsar.Pipeline.Graphics
{
    public class ShaderTechniqueContent
    {
        #region Constructors

        public ShaderTechniqueContent(string name)
        {
            Name = name;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public bool IsTransparent { get; set; }

        #endregion
    }
}
