using System;

using System.Collections.Generic;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contains additional data about a sub mesh
    /// Used by the content pipeline, data are fetch by mesh and sub mesh class
    /// </summary>
    public sealed class SubMeshData
    {
        #region Fields

        private readonly Dictionary<string, string> texturesName = new Dictionary<string, string>();

        #endregion

        #region Properties

        /// <summary>
        /// Map of textures name associated to this sub mesh
        /// </summary>
        public Dictionary<string, string> TexturesName
        {
            get { return this.texturesName; }
        }

        /// <summary>
        /// Get or set the bounding volumes information
        /// </summary>
        public BoundingData BoundingVolume { get; set; }

        #endregion
    }

    /// <summary>
    /// Contains additional data about a mesh
    /// Used by the content pipeline, data are fetch by mesh and sub mesh class
    /// </summary>
    public sealed class MeshData
    {
        #region Fields

        private readonly List<SubMeshData> subData = new List<SubMeshData>();

        #endregion

        #region Properties

        /// <summary>
        /// Get the list of sub mesh data
        /// </summary>
        public List<SubMeshData> SubMeshData
        {
            get { return this.subData; }
        }

        /// <summary>
        /// Get or set the bounding volumes information
        /// </summary>
        public BoundingData BoundingVolume { get; set; }

        #endregion
    }
}
