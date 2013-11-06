using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Pulsar.Extension;

namespace Pulsar.Input
{
    using XnaGamePad = Microsoft.Xna.Framework.Input.GamePad;
    using XnaPlayerIndex = Microsoft.Xna.Framework.PlayerIndex;

    /// <summary>
    /// Enumerates analog buttons for a gamepad
    /// </summary>
    public enum AnalogButtons 
    { 
        LeftThumbStickX, 
        LeftThumbStickY,
        RightThumbStickX,
        RightThumbStickY,
        LeftTrigger, 
        RightTrigger 
    }

    /// <summary>
    /// Defines a Xbox 360 controller
    /// </summary>
    public sealed class GamePad
    {
        #region Fields

        public const short GamePadCount = 4;

        private static readonly Buttons[] AllDigital;
        private static readonly GamePad[] GamePads = new GamePad[GamePadCount];

        public readonly ReadOnlyCollection<ButtonEvent> ButtonPressed;

        internal readonly List<ButtonEvent> InternalButtonPressed = new List<ButtonEvent>();

        private readonly XnaPlayerIndex _gamePadIndex;
        private GamePadState _previousState;
        private GamePadState _currentState;
        private Vector2 _thumbRightDelta = Vector2.Zero;
        private Vector2 _thumbLeftDelta = Vector2.Zero;
        private float _triggerRightDelta;
        private float _triggerLeftDelta;

        #endregion

        #region Event

        internal event EventHandler<GamePadEventArgs> Connected;
        internal event EventHandler<GamePadEventArgs> Disconnected;

        #endregion

        #region Static constructors

        /// <summary>
        /// Static constructor of GamePad class
        /// </summary>
        static GamePad()
        {
            for (short i = 0; i < GamePadCount; i++)
            {
                GamePad pad = new GamePad((XnaPlayerIndex)i);
                GamePads[i] = pad;
            }
            AllDigital = EnumExtension.GetValues<Buttons>();
            
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of GamePad class
        /// </summary>
        /// <param name="index"></param>
        internal GamePad(XnaPlayerIndex index)
        {
            _gamePadIndex = index;
            ButtonPressed = new ReadOnlyCollection<ButtonEvent>(InternalButtonPressed);
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Updates the four gamepad
        /// </summary>
        internal static void UpdatePads()
        {
            for (short i = 0; i < GamePadCount; i++)
                GamePads[i].Update();
        }

        /// <summary>
        /// Checks if any key has been pressed on any gamepad
        /// </summary>
        /// <returns>Return true if any key has been pressed otherwise false</returns>
        public static bool AnyKeyPressed()
        {
            for (short i = 0; i < GamePadCount; i++)
            {
                if (GamePads[i].InternalButtonPressed.Count > 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Releases all listeners for all events
        /// </summary>
        public static void ReleaseAllListeners()
        {
            for (int i = 0; i < GamePadCount; i++)
                GamePads[i].ReleaseListeners();
        }

        /// <summary>
        /// Hooks a delegate to the gamepad connected event
        /// </summary>
        /// <param name="listener">Delegate to trigger</param>
        public static void AddListenerConnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePadCount; i++)
            {
                GamePads[i].Connected += listener;
            }
        }

        /// <summary>
        /// Unhooks a delegate to the gamepad connected event
        /// </summary>
        /// <param name="listener">Delegate to trigger</param>
        public static void RemoveListenerConnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePadCount; i++)
            {
                GamePads[i].Connected -= listener;
            }
        }

        /// <summary>
        /// Hooks a delegate to the gamepad disconnected event
        /// </summary>
        /// <param name="listener">Delegate to trigger</param>
        public static void AddListenerDisconnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePadCount; i++)
            {
                GamePads[i].Disconnected += listener;
            }
        }

        /// <summary>
        /// Unhooks a delegate to the gamepad disconnected event
        /// </summary>
        /// <param name="listener">Delegate to trigger</param>
        public static void RemoveListenerDisconnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePadCount; i++)
            {
                GamePads[i].Disconnected -= listener;
            }
        }

        /// <summary>
        /// Gets a gamepad
        /// </summary>
        /// <param name="player">Index of the gamepad</param>
        /// <returns>Return an instance of GamePad class</returns>
        public static GamePad GetGamePad(int player)
        {
            return GamePads[player];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Realeases all listeners for Connected/Disconnected events
        /// </summary>
        public void ReleaseListeners()
        {
            Connected = null;
            Disconnected = null;
        }

        /// <summary>
        /// Updates one gamepad state
        /// </summary>
        internal void Update()
        {
            _previousState = _currentState;
            _currentState = XnaGamePad.GetState(_gamePadIndex);

            if (!_previousState.IsConnected)
            {
                if (_currentState.IsConnected)
                {
                    if (Connected != null) 
                        Connected(this, new GamePadEventArgs(this));
                }
            }
            else
            {
                if (!_currentState.IsConnected)
                {
                    if (Disconnected != null) 
                        Disconnected(this, new GamePadEventArgs(this));
                }
            }

            if (!_currentState.IsConnected) return;
            if (_previousState.PacketNumber != _currentState.PacketNumber)
            {
                GamePadThumbSticks prevThumb = _previousState.ThumbSticks;
                GamePadThumbSticks currThumb = _currentState.ThumbSticks;
                _thumbRightDelta = Vector2.Subtract(currThumb.Right, prevThumb.Right);
                _thumbLeftDelta = Vector2.Subtract(currThumb.Left, prevThumb.Left);

                GamePadTriggers prevTrigger = _previousState.Triggers;
                GamePadTriggers currTrigger = _currentState.Triggers;
                _triggerRightDelta = currTrigger.Right - prevTrigger.Right;
                _triggerLeftDelta = currTrigger.Left - prevTrigger.Left;

                InternalButtonPressed.Clear();
                for (short i = 0; i < AllDigital.Length; i++)
                {
                    if (!IsPressed(AllDigital[i])) continue;
                    AbstractButton btn = new AbstractButton(AllDigital[i]);
                    InternalButtonPressed.Add(new ButtonEvent(btn, ButtonEventType.IsPressed, (short)_gamePadIndex));
                }
            }
        }

        /// <summary>
        /// Gets the value for an analog button
        /// </summary>
        /// <param name="btn">Analog button to find</param>
        /// <returns>Return the value of the button</returns>
        public float GetValue(AnalogButtons btn)
        {
            float result = 0.0f;
            switch (btn)
            {
                case AnalogButtons.LeftThumbStickX: result = _currentState.ThumbSticks.Left.X;
                    break;
                case AnalogButtons.LeftThumbStickY: result = _currentState.ThumbSticks.Left.Y;
                    break;
                case AnalogButtons.RightThumbStickX: result = _currentState.ThumbSticks.Right.X;
                    break;
                case AnalogButtons.RightThumbStickY: result = _currentState.ThumbSticks.Right.Y;
                    break;
                case AnalogButtons.LeftTrigger: result = _currentState.Triggers.Left;
                    break;
                case AnalogButtons.RightTrigger: result = _currentState.Triggers.Right;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Checks if a button has just been pressed
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <returns>Return true if the button has just been pressed otherwise false</returns>
        public bool IsPressed(Buttons button)
        {
            return (_previousState.IsButtonUp(button)) && (_currentState.IsButtonDown(button));
        }

        /// <summary>
        /// Checks if a button has just been released
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <returns>Return true if the button has just been released otherwise false</returns>
        public bool IsReleased(Buttons button)
        {
            return (_previousState.IsButtonDown(button)) && (_currentState.IsButtonUp(button));
        }

        /// <summary>
        /// Checks if a button is down
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <returns>Return true if the button is down otherwise false</returns>
        public bool IsDown(Buttons button)
        {
            return _currentState.IsButtonDown(button);
        }

        /// <summary>
        /// Checks if a button is up
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <returns>Return true if the button is up otherwise false</returns>
        public bool IsUp(Buttons button)
        {
            return _currentState.IsButtonUp(button);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the position of the left thumbstick
        /// </summary>
        public Vector2 ThumbLeftPosition
        {
            get { return _currentState.ThumbSticks.Left; }
        }

        /// <summary>
        /// Gets the position of the right thumbstick
        /// </summary>
        public Vector2 ThumbRightPosition
        {
            get { return _currentState.ThumbSticks.Right; }
        }

        /// <summary>
        /// Gets the delta of the left thumbstick
        /// </summary>
        public Vector2 ThumbLeftDelta
        {
            get { return _thumbLeftDelta; }
        }

        /// <summary>
        /// Gets the delta of the right thumbstick
        /// </summary>
        public Vector2 ThumbRightDelta
        {
            get { return _thumbRightDelta; }
        }

        /// <summary>
        /// Gets the value of the left trigger
        /// </summary>
        public float LeftTrigger
        {
            get { return _currentState.Triggers.Left; }
        }

        /// <summary>
        /// Gets the value of the right trigger
        /// </summary>
        public float RightTrigger
        {
            get { return _currentState.Triggers.Right; }
        }

        /// <summary>
        /// Gets the delta of the left trigger
        /// </summary>
        public float LeftTriggerDelta
        {
            get { return _triggerLeftDelta; }
        }

        /// <summary>
        /// Gets the delta of the right trigger
        /// </summary>
        public float RightTriggerDelta
        {
            get { return _triggerRightDelta; }
        }

        /// <summary>
        /// Gets a value that indicates if the gamepad is connected
        /// </summary>
        public bool IsConnected
        {
            get { return _currentState.IsConnected; }
        }

        #endregion
    }
}
