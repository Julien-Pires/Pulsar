namespace Pulsar.Assets
{
    /// <summary>
    /// Defines a mechanism for retrieving an AssetEngine object
    /// </summary>
    public interface IAssetEngineService
    {
        #region Properties

        /// <summary>
        /// Gets the AssetEngine instance
        /// </summary>
        AssetEngine AssetEngine { get; }

        #endregion
    }
}
