using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics.Rendering
{
    public interface IRenderingTechnique
    {
        #region Methods

        void Render(Viewport vp, Camera cam, RenderQueue queue);

        #endregion
    }
}
