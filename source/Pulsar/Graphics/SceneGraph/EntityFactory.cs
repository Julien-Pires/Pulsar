using Pulsar.Assets.Graphics.Models;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Factory to create Entity
    /// </summary>
    internal sealed class EntityFactory : Factory<Entity>
    {
        #region Methods

        /// <summary>
        /// Create an instance of the Entity class
        /// </summary>
        /// <param name="data">Datas</param>
        /// <returns>Return an instance of the Entity class</returns>
        public override Entity Create(params string[] data)
        {
            string modelName = data[0];
            Mesh m = MeshManager.Instance.Load(modelName, "Default", modelName);
            Entity ent = new Entity(m);

            return ent;
        }

        #endregion
    }
}
