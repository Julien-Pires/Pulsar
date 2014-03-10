using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics
{
    public sealed class FrameContext
    {
        #region Properties

        public float ElapsedTime { get; internal set; }

        public Camera Camera { get; internal set; }

        public IRenderable Renderable { get; internal set; }

        #endregion
    }
}
