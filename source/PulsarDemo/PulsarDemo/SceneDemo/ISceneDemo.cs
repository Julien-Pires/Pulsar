using System;

using Microsoft.Xna.Framework;

namespace Pulsar.SceneDemo
{
    public interface ISceneDemo
    {
        #region Methods

        void Load();

        void Update(GameTime time);

        void Render();

        #endregion
    }
}
