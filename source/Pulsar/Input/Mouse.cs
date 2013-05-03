using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using XnaMouse = Microsoft.Xna.Framework.Input.Mouse;

namespace Pulsar.Input
{
    public enum MouseButtons { Left, Right, Middle, XButton1, XButton2 }

    public enum MouseAnalogButtons { MouseX, MouseY, MouseWheel }

    public static class Mouse
    {
        #region Fields

        private static MouseState previousState;
        private static MouseState currentState;
        private static float wheelDelta = 0.0f;
        private static Vector2 previousPosition = Vector2.Zero;
        private static Vector2 currentPosition = Vector2.Zero;
        private static Vector2 positionDelta = Vector2.Zero;

        #endregion
        
        #region Methods

        internal static void Update()
        {
            Mouse.previousState = Mouse.currentState;
            Mouse.currentState = XnaMouse.GetState();

            Mouse.previousPosition = new Vector2(Mouse.previousState.X, Mouse.previousState.Y);
            Mouse.currentPosition = new Vector2(Mouse.currentState.X, Mouse.currentState.Y);
            Vector2.Subtract(ref Mouse.previousPosition, ref Mouse.currentPosition, out Mouse.positionDelta);
            Mouse.wheelDelta = Mouse.currentState.ScrollWheelValue - Mouse.previousState.ScrollWheelValue;
        }

        public static bool IsPressed(MouseButtons btn)
        {
            switch (btn)
            {
                case MouseButtons.Left: return (Mouse.previousState.LeftButton == ButtonState.Released) &&
                    (Mouse.currentState.LeftButton == ButtonState.Pressed);
                    break;
                case MouseButtons.Right: return (Mouse.previousState.RightButton == ButtonState.Released) &&
                    (Mouse.currentState.RightButton == ButtonState.Pressed);
                    break;
                case MouseButtons.Middle: return (Mouse.previousState.MiddleButton == ButtonState.Released) &&
                    (Mouse.currentState.MiddleButton == ButtonState.Pressed);
                    break;
                case MouseButtons.XButton1: return (Mouse.previousState.XButton1 == ButtonState.Released) &&
                    (Mouse.currentState.XButton1 == ButtonState.Pressed);
                    break;
                case MouseButtons.XButton2: return (Mouse.previousState.XButton2 == ButtonState.Released) &&
                    (Mouse.currentState.XButton2 == ButtonState.Pressed);
                    break;
            }

            return false;
        }

        public static bool IsReleased(MouseButtons btn)
        {
            switch (btn)
            {
                case MouseButtons.Left: return (Mouse.previousState.LeftButton == ButtonState.Pressed) &&
                    (Mouse.currentState.LeftButton == ButtonState.Released);
                    break;
                case MouseButtons.Right: return (Mouse.previousState.RightButton == ButtonState.Pressed) &&
                    (Mouse.currentState.RightButton == ButtonState.Released);
                    break;
                case MouseButtons.Middle: return (Mouse.previousState.MiddleButton == ButtonState.Pressed) &&
                    (Mouse.currentState.MiddleButton == ButtonState.Released);
                    break;
                case MouseButtons.XButton1: return (Mouse.previousState.XButton1 == ButtonState.Pressed) &&
                    (Mouse.currentState.XButton1 == ButtonState.Released);
                    break;
                case MouseButtons.XButton2: return (Mouse.previousState.XButton2 == ButtonState.Pressed) &&
                    (Mouse.currentState.XButton2 == ButtonState.Released);
                    break;
            }

            return false;
        }

        public static bool IsDown(MouseButtons btn)
        {
            switch (btn)
            {
                case MouseButtons.Left: return Mouse.currentState.LeftButton == ButtonState.Pressed;
                    break;
                case MouseButtons.Right: return Mouse.currentState.RightButton == ButtonState.Pressed;
                    break;
                case MouseButtons.Middle: return Mouse.currentState.MiddleButton == ButtonState.Pressed;
                    break;
                case MouseButtons.XButton1: return Mouse.currentState.XButton1 == ButtonState.Pressed;
                    break;
                case MouseButtons.XButton2: return Mouse.currentState.XButton2 == ButtonState.Pressed;
                    break;
            }

            return false;
        }

        public static bool IsUp(MouseButtons btn)
        {
            switch (btn)
            {
                case MouseButtons.Left: return Mouse.currentState.LeftButton == ButtonState.Released;
                    break;
                case MouseButtons.Right: return Mouse.currentState.RightButton == ButtonState.Released;
                    break;
                case MouseButtons.Middle: return Mouse.currentState.MiddleButton == ButtonState.Released;
                    break;
                case MouseButtons.XButton1: return Mouse.currentState.XButton1 == ButtonState.Released;
                    break;
                case MouseButtons.XButton2: return Mouse.currentState.XButton2 == ButtonState.Released;
                    break;
            }

            return false;
        }

        #endregion

        #region Properties

        public static float X
        {
            get { return Mouse.currentPosition.X; }
        }

        public static float Y
        {
            get { return Mouse.currentPosition.Y; }
        }

        public static float DeltaX
        {
            get { return Mouse.positionDelta.X; }
        }

        public static float DeltaY
        {
            get { return Mouse.positionDelta.Y; }
        }

        public static Vector2 Position
        {
            get { return Mouse.currentPosition; }
        }

        public static Vector2 PositionDelta
        {
            get { return Mouse.positionDelta; }
        }

        public static float WheelDelta
        {
            get { return Mouse.wheelDelta; }
        }

        #endregion
    }
}
