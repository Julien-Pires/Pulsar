using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Assets.Graphics.Models;
using Pulsar.Graphics.Debugger;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// An Entity represent a complete mesh object in the 3D scene. 
    /// The Entity is composed of multiple SubEntity wich represent a subpart of the mesh.
    /// </summary>
    public sealed class Entity : IMovable
    {
        #region Fields

        public bool RenderAABB = false;

        internal Matrix[] bonesTransform;
        internal int bonesCount;

        private MeshBoundingBox meshAABB = null;
        private bool isRendered = false;
        private string name = string.Empty;
        private Mesh mesh = null;
        private SceneNode parent = null;
        private BoundingVolume bounds = new BoundingVolume();
        private List<SubEntity> subEntities = new List<SubEntity>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Entity class
        /// </summary>
        internal Entity()
        {
            this.Visible = true;
            this.bounds.Type = BoundingType.AABB;
        }

        /// <summary>
        /// Constructor of Entity class
        /// </summary>
        /// <param name="m">Mesh associated to this entity</param>
        internal Entity(Mesh m) : this()
        {
            this.mesh = m;
            this.ProcessMesh();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process the mesh associated to this entity to extract data
        /// </summary>
        private void ProcessMesh()
        {
            this.CreateSubEntities();

            this.bounds.InitialBox = this.mesh.AxisAlignedBoundingBox;
            this.bonesTransform = this.mesh.Bones;
            this.bonesCount = this.bonesTransform.Length;
        }

        /// <summary>
        /// Reset the entity
        /// </summary>
        private void Reset()
        {
            this.subEntities.Clear();
            this.bonesTransform = null;
            this.bounds.InitialBox = new BoundingBox();
        }

        /// <summary>
        /// Attach this object to a scene node
        /// <remarks>(Used internally)</remarks>
        /// </summary>
        /// <param name="parent">Node parent</param>
        public void AttachParent(SceneNode parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Detach this object from a scene node
        /// <remarks>(Used internally)</remarks>
        /// </summary>
        public void DetachParent()
        {
            this.parent = null;
        }

        /// <summary>
        /// Notify this Entity of the current camera
        /// </summary>
        /// <param name="cam">Current camera</param>
        public void NotifyCurrentCamera(Camera cam)
        {
            SpeedFrustum frustCull = cam.FastFrustum;
            this.UpdateBounds();

            this.isRendered = this.bounds.FrustumIntersect(ref frustCull);
        }

        /// <summary>
        /// Merge the bounding box of the entity with another one
        /// </summary>
        /// <param name="original">Bounding box to merge with the box of the entity</param>
        /// <returns>Return the result of the merge</returns>
        public BoundingBox Merge(ref BoundingBox original)
        {
            throw new NotImplementedException();   
        }

        /// <summary>
        /// Update the given render queue with all the sub-entities of this Entity
        /// </summary>
        /// <param name="queue">RenderQueue to fill</param>
        public void UpdateRenderQueue(RenderQueue queue)
        {
            for (int i = 0; i < this.subEntities.Count; i++)
            {
                SubEntity sub = this.subEntities[i];
                queue.AddRenderable(sub);
            }

            if (this.RenderAABB)
            {
                if (this.meshAABB == null)
                {
                    this.meshAABB = new MeshBoundingBox();
                }

                BoundingBox aabb = this.bounds.Box;
                this.meshAABB.UpdateBox(ref aabb);
                queue.AddRenderable(this.meshAABB);
            }
        }

        /// <summary>
        /// Create all the sub-entities for this Entity
        /// </summary>
        private void CreateSubEntities()
        {
            List<SubMesh> subMeshes = this.mesh.SubMeshes;

            for (int i = 0; i < subMeshes.Count; i++)
            {
                SubMesh meshPart = subMeshes[i];
                SubEntity subEnt = new SubEntity(meshPart.ID.ToString(), this, meshPart);

                this.subEntities.Add(subEnt);
            }
        }

        /// <summary>
        /// Compute the entity bounding box
        /// </summary>
        private void UpdateBounds()
        {
            Matrix transform = this.parent.FullTransform;
            this.bounds.Update(ref transform);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set a boolean indicating if this entity use batch for draw calls
        /// </summary>
        public bool UseInstancing { get; set; }

        /// <summary>
        /// Set the model attached to this Entity
        /// </summary>
        public Mesh Mesh
        {
            get { return this.mesh; }
            set 
            {
                this.mesh = value;

                this.Reset();
                this.ProcessMesh();
            }
        }

        /// <summary>
        /// Get the AABB of the entity
        /// </summary>
        public BoundingBox WorldBoundingBox
        {
            get 
            {
                this.UpdateBounds();

                return this.bounds.Box;
            }
        }

        /// <summary>
        /// Get a boolean indicating if this object is attached to a scene node
        /// </summary>
        public bool IsAttached
        {
            get { return this.parent != null; }
        }

        /// <summary>
        /// Get the name of this Entity
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Get or set a boolean indicating if this object is visible
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Get a boolean indicating if this entity is rendered
        /// </summary>
        public bool IsRendered
        {
            get 
            {
                if (!this.Visible || !this.isRendered)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Get a boolean indicating if the parent has changed
        /// </summary>
        public bool HasParentChanged { get; set; }

        /// <summary>
        /// Get the parent scene node
        /// </summary>
        public SceneNode Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// Get the transform matrix of this object
        /// </summary>
        public Matrix Transform
        {
            get
            {
                if (this.parent != null)
                    return this.parent.FullTransform;

                return Matrix.Identity;
            }
        }

        /// <summary>
        /// Get the list of all sub entity
        /// </summary>
        internal List<SubEntity> SubEntities
        {
            get { return this.subEntities; }
        }

        #endregion
    }
}
