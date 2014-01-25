using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    internal sealed class MeshData
    {
        #region Fields

        private int _current;
        private readonly List<SubMeshData> _subData = new List<SubMeshData>();

        #endregion

        #region Constructors

        internal MeshData()
        {
        }

        internal MeshData(int subMeshCount)
        {
            for(int i = 0; i < subMeshCount; i++)
                _subData.Add(new SubMeshData());
        }

        #endregion

        #region Methods

        internal void Reset()
        {
            _current = 0;
        }

        internal bool MoveNext()
        {
            _current++;

            return _current < _subData.Count;
        }

        #endregion

        #region Properties

        public List<SubMeshData> SubMeshDatas
        {
            get { return _subData; }
        }

        internal SubMeshData CurrentSubMeshData
        {
            get { return _subData[_current]; }
        }

        public BoundingBox Aabb { get; set; }

        public BoundingSphere BoundingSphere { get; set; }

        #endregion
    }
}
