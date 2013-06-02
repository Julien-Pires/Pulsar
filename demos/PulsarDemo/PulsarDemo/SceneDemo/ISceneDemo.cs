using System;

using Microsoft.Xna.Framework;

namespace PulsarDemo.SceneDemo
{
    public interface ISceneDemo
    {
        #region Methods

        void Load();

        void Activate();

        void Update(GameTime time);

        void Render();

        #endregion

        #region Properties

        bool IsLoaded { get; }

        #endregion
    }
}
