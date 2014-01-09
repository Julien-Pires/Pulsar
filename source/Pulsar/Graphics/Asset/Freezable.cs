namespace Pulsar.Graphics.Asset
{
    public abstract class Freezable
    {
        #region Methods

        protected void Freeze()
        {
            IsFrozen = true;
        }

        #endregion

        #region Properties

        protected bool IsFrozen { get; private set; }

        #endregion
    }
}
