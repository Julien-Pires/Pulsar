using System;
using System.Text;

using System.Collections.Generic;

namespace Pulsar.Assets
{
    /// <summary>
    /// Base class for all resources
    /// </summary>
    public abstract class Asset
    {
        #region Fields

        private static long uniqueID = long.MinValue;

        protected IAssetManager<Asset> assetManager = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Asset class
        /// </summary>
        /// <param name="name"></param>
        protected Asset(string name)
        {
            this.ID = Asset.uniqueID;
            Asset.uniqueID++;
            this.Name = name;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reload the asset and all it's resources
        /// </summary>
        internal virtual void Reload()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the asset ID
        /// </summary>
        public long ID { get; protected set; }

        /// <summary>
        /// Get the name of the asset
        /// </summary>
        public string Name { get; internal set; }

        #endregion
    }

    /// <summary>
    /// Interface for a pointer to an asset
    /// </summary>
    /// <typeparam name="T">Type of the asset kept in the pointer</typeparam>
    public interface IAssetPtr<out T> where T : Asset
    {
        #region Methods

        /// <summary>
        /// Increment the reference to the asset
        /// </summary>
        void AddRef();

        /// <summary>
        /// Decrement the reference to the asset
        /// </summary>
        void RemoveRef();

        #endregion

        #region Properties

        /// <summary>
        /// Get the name of the pointer
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the asset contained in this pointer
        /// </summary>
        T Resource { get; }

        #endregion
    }
    
    /// <summary>
    /// Class defining a pointer to an asset
    /// A pointer keep track of the number of reference to the asset
    /// </summary>
    /// <typeparam name="T">Type of the asset kept in the pointer</typeparam>
    public sealed class AssetPtr<T> : IAssetPtr<T> where T : Asset
    {
        #region Fields

        private T resource = default(T);
        private int counter = 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AssetPtr class
        /// </summary>
        /// <param name="resource">Asset referenced by this pointer</param>
        internal AssetPtr(T resource)
        {
            this.resource = resource;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Increment the reference to the asset 
        /// </summary>
        public void AddRef()
        {
            this.counter++;
            this.IsFree = false;
        }

        /// <summary>
        /// Decrement the reference to the asset
        /// </summary>
        public void RemoveRef()
        {
            this.counter--;
            if (this.counter == 0)
            {
                this.IsFree = true;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the name of the pointer
        /// </summary>
        public string Name
        {
            get { return this.resource.Name; }
        }

        /// <summary>
        /// Get a boolean indicating if the asset has no reference
        /// </summary>
        public bool IsFree { get; internal set; }

        /// <summary>
        /// Get the number of reference to the asset
        /// </summary>
        public int Counter
        {
            get { return this.counter; }
        }

        /// <summary>
        /// Get the asset
        /// </summary>
        public T Resource
        {
            get { return this.resource; }
        }

        #endregion
    }
}
