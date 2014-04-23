using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Contains built-time and runtime extra datas for a mesh
    /// </summary>
    internal sealed class MeshData
    {
        #region Fields

        private int _current;
        private readonly List<SubMeshData> _subData = new List<SubMeshData>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of MeshData class
        /// </summary>
        internal MeshData()
        {
        }

        /// <summary>
        /// Constructor of MeshData class
        /// </summary>
        /// <param name="subMeshCount">Number of submesh</param>
        internal MeshData(int subMeshCount)
        {
            for(int i = 0; i < subMeshCount; i++)
                _subData.Add(new SubMeshData());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the current enumeration
        /// </summary>
        internal void Reset()
        {
            _current = 0;
        }

        /// <summary>
        /// Moves the enumeration to the next submesh data
        /// </summary>
        /// <returns>Returns true if there is a next submesh otherwise false</returns>
        internal bool MoveNext()
        {
            _current++;

            return _current < _subData.Count;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the list of submesh
        /// </summary>
        public List<SubMeshData> SubMeshDatas
        {
            get { return _subData; }
        }

        /// <summary>
        /// Gets the current submesh
        /// </summary>
        internal SubMeshData CurrentSubMeshData
        {
            get { return _subData[_current]; }
        }

        /// <summary>
        /// Gets or sets the AABB of the mesh
        /// </summary>
        public BoundingBox Aabb { get; set; }

        /// <summary>
        /// Gets or sets the bounding sphere of the mesh
        /// </summary>
        public BoundingSphere BoundingSphere { get; set; }

        #endregion
    }
}
