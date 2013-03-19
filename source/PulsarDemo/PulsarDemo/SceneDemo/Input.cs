using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Pulsar;

namespace Pulsar.SceneDemo
{
    public sealed class Input : Singleton<Input>
    {
        #region Fields

        private MouseState previousMouse;
        private MouseState currentMouse;
        private KeyboardState previousKey;
        private KeyboardState currentKey;

        #endregion

        #region Constructors

        private Input() { }

        #endregion

        #region Methods

        public bool IsKeyDown(Keys k)
        {
            return this.currentKey.IsKeyDown(k);
        }

        public bool IsKeyUp(Keys k)
        {
            return this.currentKey.IsKeyUp(k);
        }

        public bool IsKeyPressed(Keys k)
        {
            return (this.previousKey.IsKeyDown(k)) && (this.currentKey.IsKeyUp(k));
        }

        public Vector2 GetMousePositionDiff()
        {
            float diffX = this.previousMouse.X - this.currentMouse.X;
            float diffY = this.previousMouse.Y - this.currentMouse.Y;

            return new Vector2(diffX, diffY);
        }

        public void GetMousePositionDiff(out Vector2 diffPos)
        {
            float diffX = this.previousMouse.X - this.currentMouse.X;
            float diffY = this.previousMouse.Y - this.currentMouse.Y;
            diffPos.X = diffX;
            diffPos.Y = diffY;
        }

        public void ResetMouse()
        {
            Mouse.SetPosition(0, 0);
        }

        public void Update()
        {
            this.previousMouse = this.currentMouse;
            this.previousKey = this.currentKey;
            this.currentMouse = Mouse.GetState();
            this.currentKey = Keyboard.GetState();
        }

        #endregion
    }
}
