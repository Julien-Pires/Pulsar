using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Pulsar.Extension;

using XnaGamePad = Microsoft.Xna.Framework.Input.GamePad;

namespace Pulsar.Input
{
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
    /// Allows to retrieve state of a Xbox 360 controller
    /// </summary>
    public sealed class GamePad
    {
        #region Fields

        private const short gamePadCount = 4;
        private static Buttons[] AllDigital;
        internal static List<ButtonEvent> ButtonPressed = new List<ButtonEvent>();
        private static GamePad[] gamePads = new GamePad[GamePad.gamePadCount];

        private PlayerIndex gamePadIndex;
        private GamePadState previousState;
        private GamePadState currentState;
        private Vector2 thumbRightDelta = Vector2.Zero;
        private Vector2 thumbLeftDelta = Vector2.Zero;
        private float triggerRightDelta = 0.0f;
        private float triggerLeftDelta = 0.0f;

        #endregion

        #region Event

        internal event EventHandler<GamePadEventArgs> Connected;
        internal event EventHandler<GamePadEventArgs> Disconnected;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor of GamePad class
        /// </summary>
        static GamePad()
        {
            for (short i = 0; i < GamePad.gamePadCount; i++)
            {
                GamePad pad = new GamePad((PlayerIndex)i);
                GamePad.gamePads[i] = pad;
            }
            GamePad.Initialize();
        }

        /// <summary>
        /// Constructor of GamePad class
        /// </summary>
        /// <param name="index"></param>
        internal GamePad(PlayerIndex index)
        {
            this.gamePadIndex = index;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize static GamePad
        /// </summary>
        internal static void Initialize()
        {
#if !XBOX
            GamePad.AllDigital = (Buttons[])Enum.GetValues(typeof(Buttons));
#else
            GamePad.AllDigital = EnumExtension.GetValues<Buttons>();
#endif
        }

        /// <summary>
        /// Update the four gamepad
        /// </summary>
        internal static void UpdatePads()
        {
            GamePad.ButtonPressed.Clear(); 
            for (short i = 0; i < GamePad.gamePadCount; i++)
            {
                GamePad pad = GamePad.gamePads[i];
                pad.Update();

                if (pad.IsConnected)
                {
                    for (short j = 0; j < GamePad.AllDigital.Length; j++)
                    {
                        if (pad.IsPressed(GamePad.AllDigital[j]))
                        {
                            AbstractButton btn = new AbstractButton(GamePad.AllDigital[j]);
                            GamePad.ButtonPressed.Add(new ButtonEvent(btn, ButtonEventType.IsPressed, i));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if any key has been pressed on any gamepad
        /// </summary>
        /// <returns>Return true if any key has been pressed otherwise false</returns>
        public static bool AnyKeyPressed()
        {
            return GamePad.ButtonPressed.Count > 0;
        }

        /// <summary>
        /// Hook a delegate to the gamepad connected event
        /// </summary>
        /// <param name="listener">Delegate to trigger</param>
        public static void HookConnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePad.gamePadCount; i++)
            {
                GamePad.gamePads[i].Connected += listener;
            }
        }

        /// <summary>
        /// Unhook a delegate to the gamepad connected event
        /// </summary>
        /// <param name="listener">Delegate to trigger</param>
        public static void UnhookConnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePad.gamePadCount; i++)
            {
                GamePad.gamePads[i].Connected -= listener;
            }
        }

        /// <summary>
        /// Hook a delegate to the gamepad disconnected event
        /// </summary>
        /// <param name="listener">Delegate to trigger</param>
        public static void HookDisconnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePad.gamePadCount; i++)
            {
                GamePad.gamePads[i].Disconnected += listener;
            }
        }

        /// <summary>
        /// Unhook a delegate to the gamepad disconnected event
        /// </summary>
        /// <param name="listener">Delegate to trigger</param>
        public static void UnhookDisconnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePad.gamePadCount; i++)
            {
                GamePad.gamePads[i].Disconnected -= listener;
            }
        }

        /// <summary>
        /// Get a gamepad
        /// </summary>
        /// <param name="player">Index of the gamepad</param>
        /// <returns>Return an instance of GamePad class</returns>
        public static GamePad GetGamePad(int player)
        {
            return GamePad.gamePads[player];
        }

        /// <summary>
        /// Update one gamepad state
        /// </summary>
        internal void Update()
        {
            this.previousState = this.currentState;
            this.currentState = XnaGamePad.GetState(this.gamePadIndex);

            if (!this.previousState.IsConnected)
            {
                if (this.currentState.IsConnected)
                {
                    if (this.Connected != null)
                    {
                        this.Connected(this, new GamePadEventArgs(this));
                    }
                }
            }
            else
            {
                if (!this.currentState.IsConnected)
                {
                    if (this.Disconnected != null)
                    {
                        this.Disconnected(this, new GamePadEventArgs(this));
                    }
                }
            }

            if (this.currentState.IsConnected)
            {
                GamePadThumbSticks prevThumb = this.previousState.ThumbSticks;
                GamePadThumbSticks currThumb = this.currentState.ThumbSticks;
                this.thumbRightDelta = Vector2.Subtract(currThumb.Right, prevThumb.Right);
                this.thumbLeftDelta = Vector2.Subtract(currThumb.Left, prevThumb.Left);

                GamePadTriggers prevTrigger = this.previousState.Triggers;
                GamePadTriggers currTrigger = this.currentState.Triggers;
                this.triggerRightDelta = currTrigger.Right - prevTrigger.Right;
                this.triggerLeftDelta = currTrigger.Left - prevTrigger.Left;
            }
        }

        /// <summary>
        /// Get the value for an analog button
        /// </summary>
        /// <param name="btn">Analog button to find</param>
        /// <returns>Return the value of the button</returns>
        public float GetValue(AnalogButtons btn)
        {
            switch (btn)
            {
                case AnalogButtons.LeftThumbStickX: return this.currentState.ThumbSticks.Left.X;
                    break;
                case AnalogButtons.LeftThumbStickY: return this.currentState.ThumbSticks.Left.Y;
                    break;
                case AnalogButtons.RightThumbStickX: return this.currentState.ThumbSticks.Right.X;
                    break;
                case AnalogButtons.RightThumbStickY: return this.currentState.ThumbSticks.Right.Y;
                    break;
                case AnalogButtons.LeftTrigger: return this.currentState.Triggers.Left;
                    break;
                case AnalogButtons.RightTrigger: return this.currentState.Triggers.Right;
                    break;
            }

            return 0.0f;
        }

        /// <summary>
        /// Check if a button has just been pressed
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <returns>Return true if the button has just been pressed otherwise false</returns>
        public bool IsPressed(Buttons button)
        {
            return (this.previousState.IsButtonUp(button)) && (this.currentState.IsButtonDown(button));
        }

        /// <summary>
        /// Check if a button has just been released
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <returns>Return true if the button has just been released otherwise false</returns>
        public bool IsReleased(Buttons button)
        {
            return (this.previousState.IsButtonDown(button)) && (this.currentState.IsButtonUp(button));
        }

        /// <summary>
        /// Check if a button is down
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <returns>Return true if the button is down otherwise false</returns>
        public bool IsDown(Buttons button)
        {
            return this.currentState.IsButtonDown(button);
        }

        /// <summary>
        /// Check if a button is up
        /// </summary>
        /// <param name="button">Button to check</param>
        /// <returns>Return true if the button is up otherwise false</returns>
        public bool IsUp(Buttons button)
        {
            return this.currentState.IsButtonUp(button);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the position of the left thumbstick
        /// </summary>
        public Vector2 ThumbLeftPosition
        {
            get { return this.currentState.ThumbSticks.Left; }
        }

        /// <summary>
        /// Get the position of the right thumbstick
        /// </summary>
        public Vector2 ThumbRightPosition
        {
            get { return this.currentState.ThumbSticks.Right; }
        }

        /// <summary>
        /// Get the delta of the left thumbstick
        /// </summary>
        public Vector2 ThumbLeftDelta
        {
            get { return this.thumbLeftDelta; }
        }

        /// <summary>
        /// Get the delta of the right thumbstick
        /// </summary>
        public Vector2 ThumbRightDelta
        {
            get { return this.thumbRightDelta; }
        }

        /// <summary>
        /// Get the value of the left trigger
        /// </summary>
        public float LeftTrigger
        {
            get { return this.currentState.Triggers.Left; }
        }

        /// <summary>
        /// Get the value of the right trigger
        /// </summary>
        public float RightTrigger
        {
            get { return this.currentState.Triggers.Right; }
        }

        /// <summary>
        /// Get the delta of the left trigger
        /// </summary>
        public float LeftTriggerDelta
        {
            get { return this.triggerLeftDelta; }
        }

        /// <summary>
        /// Get the delta of the right trigger
        /// </summary>
        public float RightTriggerDelta
        {
            get { return this.triggerRightDelta; }
        }

        /// <summary>
        /// Get a value that indicates if the gamepad is connected
        /// </summary>
        public bool IsConnected
        {
            get { return this.currentState.IsConnected; }
        }

        #endregion
    }
}
