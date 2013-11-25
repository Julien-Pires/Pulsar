using System.Collections.Generic;
using System.Globalization;

using Microsoft.Xna.Framework;

using Pulsar.Assets.Graphics.Models;
using Pulsar.Graphics.Debugger;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Represents and manipulates a mesh in a 3D scene graph
    /// </summary>
    public sealed class Entity : Movable
    {
        #region Fields

        /// <summary>
        /// Indicates if the AABB of this entity must be rendered
        /// </summary>
        public bool RenderAabb;

        internal Matrix[] BonesTransform;
        internal int BonesCount;

        private MeshBoundingBox _meshAabb;
        private Mesh _mesh;
        private readonly List<SubEntity> _subEntities = new List<SubEntity>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Entity class
        /// </summary>
        internal Entity()
        {
            Visible = true;
        }

        /// <summary>
        /// Constructor of Entity class
        /// </summary>
        /// <param name="m">Mesh associated to this entity</param>
        internal Entity(Mesh m) : this()
        {
            _mesh = m;
            ProcessMesh();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Extracts data from the associated mesh
        /// </summary>
        private void ProcessMesh()
        {
            CreateSubEntities();

            LocalAabb.SetFromAabb(Mesh.AxisAlignedBoundingBox);
            BonesTransform = _mesh.Bones;
            BonesCount = BonesTransform.Length;
        }

        /// <summary>
        /// Resets the entity
        /// </summary>
        private void Reset()
        {
            _subEntities.Clear();
            BonesTransform = null;
            BonesCount = 0;
            ResetBounds();
        }

        /// <summary>
        /// Updates the local AABB
        /// </summary>
        public override void UpdateLocalBounds()
        {
        }

        /// <summary>
        /// Updates the given render queue with all the sub-entities of this Entity
        /// </summary>
        /// <param name="queue">RenderQueue to fill</param>
        public override void UpdateRenderQueue(RenderQueue queue)
        {
            for (int i = 0; i < _subEntities.Count; i++)
                queue.AddRenderable(_subEntities[i]);

            if (!RenderAabb) return;

            if (_meshAabb == null) 
                _meshAabb = new MeshBoundingBox();

            _meshAabb.UpdateBox(WorldAabb);
            queue.AddRenderable(_meshAabb);
        }

        /// <summary>
        /// Gets the sub entity at the specified index
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Returns a SubEntity</returns>
        public SubEntity GetSubEntity(int index)
        {
            return _subEntities[index];
        }

        /// <summary>
        /// Gets the sub entity with a specified name
        /// </summary>
        /// <param name="name">Name of the sub entity</param>
        /// <returns>Returns a SubEntity</returns>
        public SubEntity GetSubEntity(string name)
        {
            int index = _mesh.GetSubMeshIndex(name);

            return GetSubEntity(index);
        }

        /// <summary>
        /// Creates all the sub-entities for this Entity
        /// </summary>
        private void CreateSubEntities()
        {
            List<SubMesh> subMeshes = _mesh.SubMeshes;

            for (int i = 0; i < subMeshes.Count; i++)
            {
                SubMesh meshPart = subMeshes[i];
                SubEntity subEnt = new SubEntity(meshPart.Id.ToString(CultureInfo.InvariantCulture), this, meshPart);

                _subEntities.Add(subEnt);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating if this entity use instancing for draw calls
        /// </summary>
        public bool UseInstancing { get; set; }

        /// <summary>
        /// Gets or sets the model attached to this Entity
        /// </summary>
        public Mesh Mesh
        {
            get { return _mesh; }
            set 
            {
                _mesh = value;

                Reset();
                ProcessMesh();
            }
        }

        /// <summary>
        /// Gets the list of all sub entity
        /// </summary>
        internal List<SubEntity> SubEntities
        {
            get { return _subEntities; }
        }

        #endregion
    }
}
