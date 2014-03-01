using System;
using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics
{
    /// <summary>
    /// List of render queue ID
    /// </summary>
    public enum RenderQueueGroupId
    {
        Background = 0,
        Default = 5,
        Overlay = 10,
        Count = 11
    }

    /// <summary>
    /// Represents a render queue
    /// </summary>
    public sealed class RenderQueue
    {
        #region Fields

        private readonly RenderQueueGroup[] _queueGroups = new RenderQueueGroup[(int)RenderQueueGroupId.Count];

        #endregion

        #region Methods

        /// <summary>
        /// Adds a renderable object in the render queue
        /// </summary>
        /// <param name="renderable">IRenderable instance</param>
        internal void AddRenderable(IRenderable renderable)
        {
            AddRenderable(renderable, renderable.RenderQueueId);
        }

        /// <summary>
        /// Adds a renderable object in the render queue at a specific position
        /// </summary>
        /// <param name="renderable">IRenderable instance</param>
        /// <param name="groupId">Render queue id to attached the IRenderable</param>
        internal void AddRenderable(IRenderable renderable, int groupId)
        {
            RenderQueueGroup group = GetGroup(groupId);
            group.AddRenderable(renderable);
        }

        /// <summary>
        /// Clears all render queue group
        /// </summary>
        internal void Clear()
        {
            for (int i = 0; i < _queueGroups.Length; i++)
            {
                RenderQueueGroup group = _queueGroups[i];
                if(group != null) 
                    group.Clear();
            }
        }

        /// <summary>
        /// Checks if an object is visible and adds it to the render queue
        /// </summary>
        /// <param name="camera">Current camera</param>
        /// <param name="movable">IMovable instance</param>
        internal void ProcessesVisibleObject(Camera camera, IMovable movable)
        {
            movable.FrustumCulling(camera);

            if (movable.IsRendered) 
                movable.UpdateRenderQueue(this);
        }

        /// <summary>
        /// Get a specific render queue group
        /// </summary>
        /// <param name="id">ID of the render queue group</param>
        /// <returns>Returns an RenderQueueGroup</returns>
        private RenderQueueGroup GetGroup(int id)
        {
            if (id > (int)RenderQueueGroupId.Count) 
                throw new ArgumentException("Invalid RenderQueueGroup id", "id");

            if (_queueGroups[id] != null) 
                return _queueGroups[id];

            RenderQueueGroup group = new RenderQueueGroup(id);
            AddNewRenderGroup(group);

            return group;
        }

        /// <summary>
        /// Create a new RenderQueueGroup
        /// </summary>
        /// <param name="renderGroup"></param>
        private void AddNewRenderGroup(RenderQueueGroup renderGroup)
        {
            _queueGroups[renderGroup.Id] = renderGroup;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an array of RenderQueueGroup
        /// </summary>
        internal RenderQueueGroup[] QueueGroupList
        {
            get { return _queueGroups; }
        }

        #endregion
    }
}
