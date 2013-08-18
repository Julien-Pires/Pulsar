using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XnaMouse = Microsoft.Xna.Framework.Input.Mouse;

using Pulsar.Extension;

namespace Pulsar.Input
{
    /// <summary>
    /// Enumerates digital mouse button
    /// </summary>
    public enum MouseButtons
    {
        Left, 
        Right, 
        Middle, 
        XButton1, 
        XButton2
    }

    /// <summary>
    /// Enumerates analog mouse button
    /// </summary>
    public enum MouseAnalogButtons
    {
        MouseX,
        MouseY, 
        MouseWheel
    }

    /// <summary>
    /// Allows to retrieve the state of the mouse
    /// </summary>
    public static class Mouse
    {
        #region Fields

        internal static readonly List<ButtonEvent> ButtonPressed = new List<ButtonEvent>();

        private static readonly MouseButtons[] AllDigital;
        private static MouseState _previousState;
        private static MouseState _currentState;
        private static Vector2 _previousPosition = Vector2.Zero;
        private static Vector2 _currentPosition = Vector2.Zero;
        private static Vector2 _positionDelta = Vector2.Zero;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor of Mouse class
        /// </summary>
        static Mouse()
        {
            AllDigital = EnumExtension.GetValues<MouseButtons>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update mouse states
        /// </summary>
        internal static void Update()
        {
            _previousState = _currentState;
            _currentState = XnaMouse.GetState();

            _previousPosition = new Vector2(_previousState.X, _previousState.Y);
            _currentPosition = new Vector2(_currentState.X, _currentState.Y);
            Vector2.Subtract(ref _currentPosition, ref _previousPosition, out _positionDelta);
            WheelDelta = _currentState.ScrollWheelValue - _previousState.ScrollWheelValue;

            ButtonPressed.Clear();
            for (int i = 0; i < AllDigital.Length; i++)
            {
                if (!IsPressed(AllDigital[i])) continue;
                AbstractButton btn = new AbstractButton(AllDigital[i]);
                ButtonPressed.Add(new ButtonEvent(btn, ButtonEventType.IsPressed));
            }
        }

        /// <summary>
        /// Check if any key is pressed
        /// </summary>
        /// <returns></returns>
        public static bool AnyKeyPressed()
        {
            return ButtonPressed.Count > 0;
        }

        /// <summary>
        /// Get the value of an analog mouse button
        /// </summary>
        /// <param name="btn">Analog button</param>
        /// <returns>Return the value of the button</returns>
        public static float GetValue(MouseAnalogButtons btn)
        {
            float result = 0.0f;
            switch (btn)
            {
                case MouseAnalogButtons.MouseX: result = _currentPosition.X;
                    break;
                case MouseAnalogButtons.MouseY: result = _currentPosition.Y;
                    break;
                case MouseAnalogButtons.MouseWheel: result = _currentState.ScrollWheelValue;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Get the delta value of an anlog mouse button
        /// </summary>
        /// <param name="btn">Analog button</param>
        /// <returns>Return the delta value of the button</returns>
        public static float GetDeltaValue(MouseAnalogButtons btn)
        {
            float result = 0.0f;
            switch (btn)
            {
                case MouseAnalogButtons.MouseX: result = _positionDelta.X;
                    break;
                case MouseAnalogButtons.MouseY: result = _positionDelta.Y;
                    break;
                case MouseAnalogButtons.MouseWheel: result = WheelDelta;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Check if a button has been pressed
        /// </summary>
        /// <param name="btn">Button to check</param>
        /// <returns>Return true if the button has been pressed otherwise false</returns>
        public static bool IsPressed(MouseButtons btn)
        {
            bool result = false;
            switch (btn)
            {
                case MouseButtons.Left: result = (_previousState.LeftButton == ButtonState.Released) &&
                    (_currentState.LeftButton == ButtonState.Pressed);
                    break;
                case MouseButtons.Right: result = (_previousState.RightButton == ButtonState.Released) &&
                    (_currentState.RightButton == ButtonState.Pressed);
                    break;
                case MouseButtons.Middle: result = (_previousState.MiddleButton == ButtonState.Released) &&
                    (_currentState.MiddleButton == ButtonState.Pressed);
                    break;
                case MouseButtons.XButton1: result = (_previousState.XButton1 == ButtonState.Released) &&
                    (_currentState.XButton1 == ButtonState.Pressed);
                    break;
                case MouseButtons.XButton2: result = (_previousState.XButton2 == ButtonState.Released) &&
                    (_currentState.XButton2 == ButtonState.Pressed);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Check if a button has been released
        /// </summary>
        /// <param name="btn">Button to check</param>
        /// <returns>Return true if the button has been released otherwise false</returns>
        public static bool IsReleased(MouseButtons btn)
        {
            bool result = false;
            switch (btn)
            {
                case MouseButtons.Left: result = (_previousState.LeftButton == ButtonState.Pressed) &&
                    (_currentState.LeftButton == ButtonState.Released);
                    break;
                case MouseButtons.Right: result = (_previousState.RightButton == ButtonState.Pressed) &&
                    (_currentState.RightButton == ButtonState.Released);
                    break;
                case MouseButtons.Middle: result = (_previousState.MiddleButton == ButtonState.Pressed) &&
                    (_currentState.MiddleButton == ButtonState.Released);
                    break;
                case MouseButtons.XButton1: result = (_previousState.XButton1 == ButtonState.Pressed) &&
                    (_currentState.XButton1 == ButtonState.Released);
                    break;
                case MouseButtons.XButton2: result = (_previousState.XButton2 == ButtonState.Pressed) &&
                    (_currentState.XButton2 == ButtonState.Released);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Check if a button is down
        /// </summary>
        /// <param name="btn">Button to check</param>
        /// <returns>Return true if the button is down otherwise false</returns>
        public static bool IsDown(MouseButtons btn)
        {
            bool result = false;
            switch (btn)
            {
                case MouseButtons.Left: result = _currentState.LeftButton == ButtonState.Pressed;
                    break;
                case MouseButtons.Right: result = _currentState.RightButton == ButtonState.Pressed;
                    break;
                case MouseButtons.Middle: result = _currentState.MiddleButton == ButtonState.Pressed;
                    break;
                case MouseButtons.XButton1: result = _currentState.XButton1 == ButtonState.Pressed;
                    break;
                case MouseButtons.XButton2: result = _currentState.XButton2 == ButtonState.Pressed;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Check if a button is up
        /// </summary>
        /// <param name="btn">Button to check</param>
        /// <returns>Return true if the button is up otherwise false</returns>
        public static bool IsUp(MouseButtons btn)
        {
            bool result = false;
            switch (btn)
            {
                case MouseButtons.Left: result = _currentState.LeftButton == ButtonState.Released;
                    break;
                case MouseButtons.Right: result = _currentState.RightButton == ButtonState.Released;
                    break;
                case MouseButtons.Middle: result = _currentState.MiddleButton == ButtonState.Released;
                    break;
                case MouseButtons.XButton1: result = _currentState.XButton1 == ButtonState.Released;
                    break;
                case MouseButtons.XButton2: result = _currentState.XButton2 == ButtonState.Released;
                    break;
            }

            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the X position of the mouse
        /// </summary>
        public static float X
        {
            get { return _currentPosition.X; }
        }

        /// <summary>
        /// Get the Y position of the mouse
        /// </summary>
        public static float Y
        {
            get { return _currentPosition.Y; }
        }

        /// <summary>
        /// Get the X position delta of the mouse
        /// </summary>
        public static float DeltaX
        {
            get { return _positionDelta.X; }
        }

        /// <summary>
        /// Get the Y position delta of the mouse
        /// </summary>
        public static float DeltaY
        {
            get { return _positionDelta.Y; }
        }

        /// <summary>
        /// Get the position of the mouse
        /// </summary>
        public static Vector2 Position
        {
            get { return _currentPosition; }
        }

        /// <summary>
        /// Get the position delta of the mouse
        /// </summary>
        public static Vector2 PositionDelta
        {
            get { return _positionDelta; }
        }

        /// <summary>
        /// Get the wheel value delta of the mouse
        /// </summary>
        public static float WheelDelta { get; private set; }

        #endregion
    }
}
