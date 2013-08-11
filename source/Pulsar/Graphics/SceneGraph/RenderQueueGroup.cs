using System.Collections.Generic;

using Pulsar.Assets.Graphics.Materials;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Class describing a group of the render queue
    /// This class do distinction between solids objects and transparents
    /// </summary>
    internal sealed class RenderQueueGroup
    {
        #region Fields

        private readonly int _groupId;
        private readonly List<IRenderable> _solids = new List<IRenderable>();
        private readonly List<IRenderable> _transparents = new List<IRenderable>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the RenderQueueGroup class
        /// </summary>
        /// <param name="id">ID of this group</param>
        public RenderQueueGroup(int id)
        {
            _groupId = id;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear this group
        /// </summary>
        internal void Clear()
        {
            _solids.Clear();
            _transparents.Clear();
        }

        /// <summary>
        /// Add a IRenderable to this group
        /// </summary>
        /// <param name="renderable">IRenderable instance</param>
        internal void AddRenderable(IRenderable renderable)
        {
            Material material = renderable.Material;

            if (!material.IsTransparent) _solids.Add(renderable);
            else _transparents.Add(renderable);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the list of transparent object
        /// </summary>
        public List<IRenderable> TransparentList
        {
            get { return _transparents; }
        }

        /// <summary>
        /// Get the list of solid object
        /// </summary>
        public List<IRenderable> SolidList
        {
            get { return _solids; }
        }

        /// <summary>
        /// Get the ID of this group
        /// </summary>
        public int Id
        {
            get { return _groupId; }
        }

        #endregion
    }
}