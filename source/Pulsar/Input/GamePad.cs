using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Pulsar.Extension;

using XnaGamePad = Microsoft.Xna.Framework.Input.GamePad;

namespace Pulsar.Input
{
    public enum AnalogButtons 
    { 
        LeftThumbStickX, 
        LeftThumbStickY,
        RightThumbStickX,
        RightThumbStickY,
        LeftTrigger, 
        RightTrigger 
    }

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

        static GamePad()
        {
            for (short i = 0; i < GamePad.gamePadCount; i++)
            {
                GamePad pad = new GamePad((PlayerIndex)i);
                GamePad.gamePads[i] = pad;
            }
            GamePad.Initialize();
        }

        internal GamePad(PlayerIndex index)
        {
            this.gamePadIndex = index;
        }

        #endregion

        #region Methods

        internal static void Initialize()
        {
#if !XBOX
            GamePad.AllDigital = (Buttons[])Enum.GetValues(typeof(Buttons));
#else
            GamePad.AllDigital = EnumExtension.GetValues<Buttons>();
#endif
        }

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

        public static bool AnyKeyPressed()
        {
            return GamePad.ButtonPressed.Count > 0;
        }

        public static void HookConnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePad.gamePadCount; i++)
            {
                GamePad.gamePads[i].Connected += listener;
            }
        }

        public static void UnhookConnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePad.gamePadCount; i++)
            {
                GamePad.gamePads[i].Connected -= listener;
            }
        }

        public static void HookDisconnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePad.gamePadCount; i++)
            {
                GamePad.gamePads[i].Disconnected += listener;
            }
        }

        public static void UnhookDisconnectedEvent(EventHandler<GamePadEventArgs> listener)
        {
            for (int i = 0; i < GamePad.gamePadCount; i++)
            {
                GamePad.gamePads[i].Disconnected -= listener;
            }
        }

        public static GamePad GetGamePad(int player)
        {
            return GamePad.gamePads[player];
        }

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

        public bool IsPressed(Buttons button)
        {
            return (this.previousState.IsButtonUp(button)) && (this.currentState.IsButtonDown(button));
        }

        public bool IsReleased(Buttons button)
        {
            return (this.previousState.IsButtonDown(button)) && (this.currentState.IsButtonUp(button));
        }

        public bool IsDown(Buttons button)
        {
            return this.currentState.IsButtonDown(button);
        }

        public bool IsUp(Buttons button)
        {
            return this.currentState.IsButtonUp(button);
        }

        #endregion

        #region Properties

        public Vector2 ThumbLeftPosition
        {
            get { return this.currentState.ThumbSticks.Left; }
        }

        public Vector2 ThumbRightPosition
        {
            get { return this.currentState.ThumbSticks.Right; }
        }

        public Vector2 ThumbLeftDelta
        {
            get { return this.thumbLeftDelta; }
        }

        public Vector2 ThumbRightDelta
        {
            get { return this.thumbRightDelta; }
        }

        public float LeftTrigger
        {
            get { return this.currentState.Triggers.Left; }
        }

        public float RightTrigger
        {
            get { return this.currentState.Triggers.Right; }
        }

        public float LeftTriggerDelta
        {
            get { return this.triggerLeftDelta; }
        }

        public float RightTriggerDelta
        {
            get { return this.triggerRightDelta; }
        }

        public bool IsConnected
        {
            get { return this.currentState.IsConnected; }
        }

        #endregion
    }
}
