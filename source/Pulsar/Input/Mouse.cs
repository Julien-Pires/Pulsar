using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XnaMouse = Microsoft.Xna.Framework.Input.Mouse;

#if XBOX
using Pulsar.Extension;
#endif

namespace Pulsar.Input
{
    /// <summary>
    /// Enumerates digital mouse button
    /// </summary>
    public enum MouseButtons { Left, Right, Middle, XButton1, XButton2 }

    /// <summary>
    /// Enumerates analog mouse button
    /// </summary>
    public enum MouseAnalogButtons { MouseX, MouseY, MouseWheel }

    /// <summary>
    /// Allows to retrieve the state of the mouse
    /// </summary>
    public static class Mouse
    {
        #region Fields

        private static MouseButtons[] AllDigital;
        internal static List<ButtonEvent> ButtonPressed = new List<ButtonEvent>();
        private static MouseState previousState;
        private static MouseState currentState;
        private static float wheelDelta;
        private static Vector2 previousPosition = Vector2.Zero;
        private static Vector2 currentPosition = Vector2.Zero;
        private static Vector2 positionDelta = Vector2.Zero;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor of Mouse class
        /// </summary>
        static Mouse()
        {
            Mouse.Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the mouse
        /// </summary>
        internal static void Initialize()
        {
#if !XBOX
            Mouse.AllDigital = (MouseButtons[])Enum.GetValues(typeof(MouseButtons));
#else
            Mouse.AllDigital = EnumExtension.GetValues<MouseButtons>();
#endif
        }

        /// <summary>
        /// Update mouse states
        /// </summary>
        internal static void Update()
        {
            Mouse.previousState = Mouse.currentState;
            Mouse.currentState = XnaMouse.GetState();

            Mouse.previousPosition = new Vector2(Mouse.previousState.X, Mouse.previousState.Y);
            Mouse.currentPosition = new Vector2(Mouse.currentState.X, Mouse.currentState.Y);
            Vector2.Subtract(ref Mouse.currentPosition, ref Mouse.previousPosition, out Mouse.positionDelta);
            Mouse.wheelDelta = Mouse.currentState.ScrollWheelValue - Mouse.previousState.ScrollWheelValue;

            Mouse.ButtonPressed.Clear();
            for (int i = 0; i < Mouse.AllDigital.Length; i++)
            {
                if (Mouse.IsPressed(Mouse.AllDigital[i]))
                {
                    AbstractButton btn = new AbstractButton(Mouse.AllDigital[i]);
                    Mouse.ButtonPressed.Add(new ButtonEvent(btn, ButtonEventType.IsPressed));
                }
            }
        }

        /// <summary>
        /// Check if any key is pressed
        /// </summary>
        /// <returns></returns>
        public static bool AnyKeyPressed()
        {
            return Mouse.ButtonPressed.Count > 0;
        }

        /// <summary>
        /// Get the value of an analog mouse button
        /// </summary>
        /// <param name="btn">Analog button</param>
        /// <returns>Return the value of the button</returns>
        public static float GetValue(MouseAnalogButtons btn)
        {
            switch (btn)
            {
                case MouseAnalogButtons.MouseX: return Mouse.currentPosition.X;
                    break;
                case MouseAnalogButtons.MouseY: return Mouse.currentPosition.Y;
                    break;
                case MouseAnalogButtons.MouseWheel: return Mouse.currentState.ScrollWheelValue;
                    break;
            }

            return 0.0f;
        }

        /// <summary>
        /// Get the delta value of an anlog mouse button
        /// </summary>
        /// <param name="btn">Analog button</param>
        /// <returns>Return the delta value of the button</returns>
        public static float GetDeltaValue(MouseAnalogButtons btn)
        {
            switch (btn)
            {
                case MouseAnalogButtons.MouseX: return Mouse.positionDelta.X;
                    break;
                case MouseAnalogButtons.MouseY: return Mouse.positionDelta.Y;
                    break;
                case MouseAnalogButtons.MouseWheel: return Mouse.wheelDelta;
                    break;
            }

            return 0.0f;
        }

        /// <summary>
        /// Check if a button has been pressed
        /// </summary>
        /// <param name="btn">Button to check</param>
        /// <returns>Return true if the button has been pressed otherwise false</returns>
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

        /// <summary>
        /// Check if a button has been released
        /// </summary>
        /// <param name="btn">Button to check</param>
        /// <returns>Return true if the button has been released otherwise false</returns>
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

        /// <summary>
        /// Check if a button is down
        /// </summary>
        /// <param name="btn">Button to check</param>
        /// <returns>Return true if the button is down otherwise false</returns>
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

        /// <summary>
        /// Check if a button is up
        /// </summary>
        /// <param name="btn">Button to check</param>
        /// <returns>Return true if the button is up otherwise false</returns>
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

        /// <summary>
        /// Get the X position of the mouse
        /// </summary>
        public static float X
        {
            get { return Mouse.currentPosition.X; }
        }

        /// <summary>
        /// Get the Y position of the mouse
        /// </summary>
        public static float Y
        {
            get { return Mouse.currentPosition.Y; }
        }

        /// <summary>
        /// Get the X position delta of the mouse
        /// </summary>
        public static float DeltaX
        {
            get { return Mouse.positionDelta.X; }
        }

        /// <summary>
        /// Get the Y position delta of the mouse
        /// </summary>
        public static float DeltaY
        {
            get { return Mouse.positionDelta.Y; }
        }

        /// <summary>
        /// Get the position of the mouse
        /// </summary>
        public static Vector2 Position
        {
            get { return Mouse.currentPosition; }
        }

        /// <summary>
        /// Get the position delta of the mouse
        /// </summary>
        public static Vector2 PositionDelta
        {
            get { return Mouse.positionDelta; }
        }

        /// <summary>
        /// Get the wheel value delta of the mouse
        /// </summary>
        public static float WheelDelta
        {
            get { return Mouse.wheelDelta; }
        }

        #endregion
    }
}
