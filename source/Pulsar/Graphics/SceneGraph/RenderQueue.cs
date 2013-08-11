using System;

namespace Pulsar.Graphics.SceneGraph
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
    /// Class describing a render queue
    /// </summary>
    public sealed class RenderQueue
    {
        #region Fields

        private readonly RenderQueueGroup[] _queueGroups = new RenderQueueGroup[(int)RenderQueueGroupId.Count];

        #endregion

        #region Methods

        /// <summary>
        /// Add a IRenderable in the render queue
        /// </summary>
        /// <param name="renderable">IRenderable instance</param>
        internal void AddRenderable(IRenderable renderable)
        {
            AddRenderable(renderable, renderable.RenderQueueId);
        }

        /// <summary>
        /// Add a IRenderable in the render queue at a specific position
        /// </summary>
        /// <param name="renderable">IRenderable instance</param>
        /// <param name="groupId">Render queue id to attached the IRenderable</param>
        internal void AddRenderable(IRenderable renderable, int groupId)
        {
            RenderQueueGroup group = GetGroup(groupId);

            group.AddRenderable(renderable);
        }

        /// <summary>
        /// Clear all render queue group
        /// </summary>
        internal void Clear()
        {
            for (int i = 0; i < _queueGroups.Length; i++)
            {
                RenderQueueGroup group = _queueGroups[i];

                if(group != null) group.Clear();
            }
        }

        /// <summary>
        /// Check if an object is visible and add it to the render queue
        /// </summary>
        /// <param name="cam">Current camera</param>
        /// <param name="movObj">IMovable instance</param>
        internal void ProcessVisibleObject(Camera cam, IMovable movObj)
        {
            movObj.NotifyCurrentCamera(cam);

            if (movObj.IsRendered) movObj.UpdateRenderQueue(this);
        }

        /// <summary>
        /// Get a specific render queue group
        /// </summary>
        /// <param name="id">ID of the render queue group</param>
        /// <returns>Returns an RenderQueueGroup</returns>
        private RenderQueueGroup GetGroup(int id)
        {
            if (id > (int)RenderQueueGroupId.Count) throw new Exception();

            if (_queueGroups[id] != null) return _queueGroups[id];

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
        /// Get an array of RenderQueueGroup
        /// </summary>
        internal RenderQueueGroup[] QueueGroupList
        {
            get { return _queueGroups; }
        }

        #endregion
    }
}
