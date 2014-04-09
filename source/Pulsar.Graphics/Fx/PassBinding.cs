using System.Diagnostics;

namespace Pulsar.Graphics.Fx
{
    public sealed class PassBinding
    {
        #region Fields

        private readonly MaterialPassId _id;
        private readonly PassDefinition _passDefinition;

        #endregion

        #region Constructors

        internal PassBinding(PassDefinition passDefinition, ushort materialId)
        {
            Debug.Assert(passDefinition != null);

            _passDefinition = passDefinition;
            _id = new MaterialPassId(materialId, passDefinition.Id);
        }

        #endregion

        #region Methods

        internal void Apply()
        {
            _passDefinition.Pass.Apply();
        }

        #endregion

        #region Properties

        public MaterialPassId Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _passDefinition.Name; }
        }

        #endregion
    }
}
