namespace Pulsar.Graphics.Asset
{
    /// <summary>
    /// Represents an object that can become immutable
    /// </summary>
    public abstract class Freezable
    {
        #region Methods

        /// <summary>
        /// Makes this instance immutable
        /// </summary>
        protected void Freeze()
        {
            IsFrozen = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that indicates if the object is immutable
        /// </summary>
        protected bool IsFrozen { get; private set; }

        #endregion
    }
}
