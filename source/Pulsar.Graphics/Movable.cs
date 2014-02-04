using System;

using Microsoft.Xna.Framework;

using Pulsar;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents a movable object in a scene graph
    /// </summary>
    public abstract class Movable : IMovable
    {
        #region Fields

        private bool _isDisposed;
        private SceneNode _parentNode;
        private readonly AxisAlignedBox _worldAabb = new AxisAlignedBox();
        private readonly AxisAlignedBox _localAabb = new AxisAlignedBox();
        private string _name = string.Empty;
        private bool _inCameraSight;
        private bool _visible;

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_isDisposed) return;

            Dispose(true);

            _isDisposed = true;
        }

        protected virtual void Dispose(bool dispose)
        {
        }

        /// <summary>
        /// Attaches a parent node
        /// </summary>
        /// <param name="parent">Parent node</param>
        void IMovable.AttachParent(SceneNode parent)
        {
            if(parent == null)
                throw new ArgumentNullException("parent");

            _parentNode = parent;
        }

        /// <summary>
        /// Detaches parent node
        /// </summary>
        void IMovable.DetachParent()
        {
            _parentNode = null;
        }

        /// <summary>
        /// Performs a frustum culling against a specified camera
        /// </summary>
        /// <param name="camera">Camera</param>
        public virtual void FrustumCulling(Camera camera)
        {
            _inCameraSight = camera.Frustum.Intersect(_worldAabb);
        }

        /// <summary>
        /// Updates bounds of the movable object
        /// </summary>
        public void UpdateBounds()
        {
            UpdateLocalBounds();

            _worldAabb.SetFromAabb(_localAabb);
            if (_parentNode != null)
                _worldAabb.Transform(_parentNode.LocalToWorld);
        }

        /// <summary>
        /// Updates bounds of the movable object in local space
        /// </summary>
        public abstract void UpdateLocalBounds();

        /// <summary>
        /// Resets bounds
        /// </summary>
        protected void ResetBounds()
        {
            _localAabb.Reset();
            _worldAabb.Reset();
        }

        /// <summary>
        /// Updates the render queue with renderable instances
        /// </summary>
        /// <param name="queue">Render queue</param>
        public abstract void UpdateRenderQueue(RenderQueue queue);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets the AABB in local space
        /// </summary>
        public AxisAlignedBox LocalAabb
        {
            get { return _localAabb; }
        }

        /// <summary>
        /// Gets the AABB in world space
        /// </summary>
        public AxisAlignedBox WorldAabb
        {
            get { return _worldAabb; }
        }

        /// <summary>
        /// Gets a value that indicates if the object is attached to a node
        /// </summary>
        public bool IsAttached
        {
            get { return _parentNode != null; }
        }

        /// <summary>
        /// Gets a value that indicates if the object will be draw
        /// </summary>
        public bool IsRendered
        {
            get { return _visible && _inCameraSight; }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the object is visible
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// Gets the parent node
        /// </summary>
        public SceneNode Parent
        {
            get { return _parentNode; }
        }

        /// <summary>
        /// Gets the transform matrix
        /// </summary>
        public Matrix Transform
        {
            get { return (_parentNode != null) ? _parentNode.LocalToWorld : Matrix.Identity; }
        }

        #endregion
    }
}
