using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Interface defining a movable object in the scene graph
    /// </summary>
    public interface IMovable
    {
        #region Methods

        /// <summary>
        /// Attach this object to a parent
        /// </summary>
        /// <param name="parent"></param>
        void AttachParent(SceneNode parent);

        /// <summary>
        /// Detach this object from his parent
        /// </summary>
        void DetachParent();

        /// <summary>
        /// Notify this instance of the current camera
        /// </summary>
        /// <param name="cam">Current camera</param>
        void NotifyCurrentCamera(Camera cam);

        /// <summary>
        /// Update the render queue
        /// </summary>
        /// <param name="queue">Current render queue</param>
        void UpdateRenderQueue(RenderQueue queue);

        #endregion

        #region Properties

        /// <summary>
        /// Get the bounding box
        /// </summary>
        BoundingBox WorldBoundingBox { get; }

        /// <summary>
        /// Get a boolean indicating if this object is attached to a scene node
        /// </summary>
        bool IsAttached { get; }

        /// <summary>
        /// Get the name of this object
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get a boolean indicating if this object will be draw
        /// </summary>
        bool IsRendered { get; }

        /// <summary>
        /// Get a boolean indicating if this object is visible
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Get or set a boolean indicating that the parent node has changed
        /// </summary>
        bool HasParentChanged { get; set; }

        /// <summary>
        /// Get the parent scene node
        /// </summary>
        SceneNode Parent { get; }

        /// <summary>
        /// Get the transform matrix
        /// </summary>
        Matrix Transform { get; }

        #endregion
    }
}
