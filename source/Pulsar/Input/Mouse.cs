using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using XnaMouse = Microsoft.Xna.Framework.Input.Mouse;

namespace Pulsar.Input
{
    public enum MouseButton { Left, Right, Middle, XButton1, XButton2 }

    public class Mouse : IPeripheric
    {
        #region Fields

        private MouseState previousState;
        private MouseState currentState;
        private float wheelDelta = 0.0f;
        private Vector2 previousPosition = Vector2.Zero;
        private Vector2 currentPosition = Vector2.Zero;
        private Vector2 positionDelta = Vector2.Zero;

        #endregion
        
        #region Methods

        internal void Update()
        {
            this.previousState = this.currentState;
            this.currentState = XnaMouse.GetState();

            this.previousPosition = new Vector2(this.previousState.X, this.previousState.Y);
            this.currentPosition = new Vector2(this.currentState.X, this.currentState.Y);
            Vector2.Subtract(ref this.previousPosition, ref this.currentPosition, out this.positionDelta);
            this.wheelDelta = this.currentState.ScrollWheelValue - this.previousState.ScrollWheelValue;
        }

        public bool IsJustPressed(MouseButton btn)
        {
            switch (btn)
            {
                case MouseButton.Left: return (this.previousState.LeftButton == ButtonState.Released) &&
                    (this.currentState.LeftButton == ButtonState.Pressed);
                    break;
                case MouseButton.Right: return (this.previousState.RightButton == ButtonState.Released) &&
                    (this.currentState.RightButton == ButtonState.Pressed);
                    break;
                case MouseButton.Middle: return (this.previousState.MiddleButton == ButtonState.Released) &&
                    (this.currentState.MiddleButton == ButtonState.Pressed);
                    break;
                case MouseButton.XButton1: return (this.previousState.XButton1 == ButtonState.Released) &&
                    (this.currentState.XButton1 == ButtonState.Pressed);
                    break;
                case MouseButton.XButton2: return (this.previousState.XButton2 == ButtonState.Released) &&
                    (this.currentState.XButton2 == ButtonState.Pressed);
                    break;
            }

            return false;
        }

        public bool IsJustReleased(MouseButton btn)
        {
            switch (btn)
            {
                case MouseButton.Left: return (this.previousState.LeftButton == ButtonState.Pressed) &&
                    (this.currentState.LeftButton == ButtonState.Released);
                    break;
                case MouseButton.Right: return (this.previousState.RightButton == ButtonState.Pressed) &&
                    (this.currentState.RightButton == ButtonState.Released);
                    break;
                case MouseButton.Middle: return (this.previousState.MiddleButton == ButtonState.Pressed) &&
                    (this.currentState.MiddleButton == ButtonState.Released);
                    break;
                case MouseButton.XButton1: return (this.previousState.XButton1 == ButtonState.Pressed) &&
                    (this.currentState.XButton1 == ButtonState.Released);
                    break;
                case MouseButton.XButton2: return (this.previousState.XButton2 == ButtonState.Pressed) &&
                    (this.currentState.XButton2 == ButtonState.Released);
                    break;
            }

            return false;
        }

        public bool IsDown(MouseButton btn)
        {
            switch (btn)
            {
                case MouseButton.Left: return this.currentState.LeftButton == ButtonState.Pressed;
                    break;
                case MouseButton.Right: return this.currentState.RightButton == ButtonState.Pressed;
                    break;
                case MouseButton.Middle: return this.currentState.MiddleButton == ButtonState.Pressed;
                    break;
                case MouseButton.XButton1: return this.currentState.XButton1 == ButtonState.Pressed;
                    break;
                case MouseButton.XButton2: return this.currentState.XButton2 == ButtonState.Pressed;
                    break;
            }

            return false;
        }

        public bool IsUp(MouseButton btn)
        {
            switch (btn)
            {
                case MouseButton.Left: return this.currentState.LeftButton == ButtonState.Released;
                    break;
                case MouseButton.Right: return this.currentState.RightButton == ButtonState.Released;
                    break;
                case MouseButton.Middle: return this.currentState.MiddleButton == ButtonState.Released;
                    break;
                case MouseButton.XButton1: return this.currentState.XButton1 == ButtonState.Released;
                    break;
                case MouseButton.XButton2: return this.currentState.XButton2 == ButtonState.Released;
                    break;
            }

            return false;
        }

        #endregion

        #region Properties

        public Vector2 Position
        {
            get { return this.currentPosition; }
        }

        public Vector2 PositionDelta
        {
            get { return this.positionDelta; }
        }

        public float WheelDelta
        {
            get { return this.wheelDelta; }
        }

        #endregion
    }
}
