using System;
using Microsoft.Xna.Framework;
using Pulsar.Core;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Defines a movable object in a scene graph
    /// </summary>
    public interface IMovable : IDisposable
    {
        #region Methods

        /// <summary>
        /// Attaches this object to a parent
        /// </summary>
        /// <param name="parent"></param>
        void AttachParent(SceneNode parent);

        /// <summary>
        /// Detaches this object from its parent
        /// </summary>
        void DetachParent();

        /// <summary>
        /// Performs a frustum culling against a specific camera
        /// </summary>
        /// <param name="camera">Camera to test visibility with</param>
        void FrustumCulling(Camera camera);

        /// <summary>
        /// Updates bounds of this object
        /// </summary>
        void UpdateBounds();

        /// <summary>
        /// Updates the render queue
        /// </summary>
        /// <param name="queue">Current render queue</param>
        void UpdateRenderQueue(RenderQueue queue);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the local AABB
        /// </summary>
        AxisAlignedBox LocalAabb { get; }

        /// <summary>
        /// Gets the world AABB
        /// </summary>
        AxisAlignedBox WorldAabb { get; }

        /// <summary>
        /// Gets a boolean indicating if this object is attached to a scene node
        /// </summary>
        bool IsAttached { get; }

        /// <summary>
        /// Gets the name of this object
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a boolean indicating if this object will be draw
        /// </summary>
        bool IsRendered { get; }

        /// <summary>
        /// Gets a boolean indicating if this object is visible
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets the parent scene node
        /// </summary>
        SceneNode Parent { get; }

        /// <summary>
        /// Gets the transform matrix
        /// </summary>
        Matrix Transform { get; }

        #endregion
    }
}
