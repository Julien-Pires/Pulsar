using System.Collections.Generic;

namespace Pulsar.Components
{
    public interface IManager
    {
        #region Methods

        void Added(List<GameObject> gameObjects);

        void Removed(List<GameObject> gameObjects);

        void Disabled(List<GameObject> gameObjects);

        void Enabled(List<GameObject> gameObjects);

        #endregion
    }
}
