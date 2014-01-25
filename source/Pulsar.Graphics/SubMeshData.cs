using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    internal sealed class SubMeshData
    {
        #region Fields

        private readonly Dictionary<string, string> _texturesName = new Dictionary<string, string>();

        #endregion

        #region Properties

        /// <summary>
        /// Map of textures name associated to this sub mesh
        /// </summary>
        public Dictionary<string, string> TexturesName
        {
            get { return _texturesName; }
        }

        public BoundingBox Aabb { get; set; }

        public BoundingSphere BoundingSphere { get; set; }

        #endregion
    }
}