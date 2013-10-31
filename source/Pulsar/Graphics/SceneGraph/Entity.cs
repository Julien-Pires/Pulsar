using System;
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
    public sealed class Entity : IMovable
    {
        #region Fields

        public bool RenderAabb;

        internal Matrix[] BonesTransform;
        internal int BonesCount;

        private MeshBoundingBox _meshAabb;
        private bool _isRendered;
        private readonly string _name = string.Empty;
        private Mesh _mesh;
        private SceneNode _parent;
        private readonly BoundingVolume _bounds = new BoundingVolume();
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

            _bounds.InitialBox = _mesh.AxisAlignedBoundingBox;
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
            _bounds.InitialBox = new BoundingBox();
        }

        /// <summary>
        /// Attachs this object to a scene node
        /// <remarks>(Used internally)</remarks>
        /// </summary>
        /// <param name="parent">Node parent</param>
        public void AttachParent(SceneNode parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Detachs this object from a scene node
        /// <remarks>(Used internally)</remarks>
        /// </summary>
        public void DetachParent()
        {
            _parent = null;
        }

        /// <summary>
        /// Notifies this Entity of the current camera
        /// </summary>
        /// <param name="cam">Current camera</param>
        public void CheckVisibilityWithCamera(Camera cam)
        {
            SpeedFrustum frustCull = cam.FastFrustum;
            UpdateBounds();

            _isRendered = _bounds.FrustumToAabbIntersect(ref frustCull);
        }

        /// <summary>
        /// Merges the bounding box of the entity with another one
        /// </summary>
        /// <param name="original">Bounding box to merge with the box of the entity</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns>Return the result of the merge</returns>
        public BoundingBox Merge(ref BoundingBox original)
        {
            throw new NotImplementedException();   
        }

        /// <summary>
        /// Updates the given render queue with all the sub-entities of this Entity
        /// </summary>
        /// <param name="queue">RenderQueue to fill</param>
        public void UpdateRenderQueue(RenderQueue queue)
        {
            for (int i = 0; i < _subEntities.Count; i++)
            {
                SubEntity sub = _subEntities[i];
                queue.AddRenderable(sub);
            }

            if (!RenderAabb) return;

            if (_meshAabb == null) _meshAabb = new MeshBoundingBox();
            BoundingBox aabb = _bounds.Box;
            _meshAabb.UpdateBox(ref aabb);
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

        /// <summary>
        /// Updates the entity bounding box
        /// </summary>
        private void UpdateBounds()
        {
            Matrix transform = _parent.LocalToWorld;
            _bounds.Update(ref transform);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating if this entity use instancing for draw calls
        /// </summary>
        public bool UseInstancing { get; set; }

        /// <summary>
        /// Sets the model attached to this Entity
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
        /// Gets the AABB of the entity
        /// </summary>
        public BoundingBox WorldBoundingBox
        {
            get 
            {
                UpdateBounds();

                return _bounds.Box;
            }
        }

        /// <summary>
        /// Gets a value indicating if this object is attached to a scene node
        /// </summary>
        public bool IsAttached
        {
            get { return _parent != null; }
        }

        /// <summary>
        /// Gets the name of this Entity
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets or sets a value indicating if this object is visible
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets a value indicating if this entity is rendered
        /// </summary>
        public bool IsRendered
        {
            get { return Visible && _isRendered; }
        }

        /// <summary>
        /// Gets a value indicating if the parent has changed
        /// </summary>
        public bool HasParentChanged { get; set; }

        /// <summary>
        /// Gets the parent scene node
        /// </summary>
        public SceneNode Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets the transform matrix of this object
        /// </summary>
        public Matrix Transform
        {
            get
            {
                return (_parent != null) ? _parent.LocalToWorld : Matrix.Identity;
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
