using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using XnaGamePad = Microsoft.Xna.Framework.Input.GamePad;

namespace Pulsar.Input
{
    public class GamePad : IPeripheric
    {
        #region Fields

        private PlayerIndex gamePadIndex;
        private GamePadState previousState;
        private GamePadState currentState;
        private Vector2 thumbRightDelta = Vector2.Zero;
        private Vector2 thumbLeftDelta = Vector2.Zero;
        private float triggerRightDelta = 0.0f;
        private float triggerLeftDelta = 0.0f;

        #endregion

        #region Event

        public event EventHandler<GamePadEventArgs> Connected;
        public event EventHandler<GamePadEventArgs> Disconnected;

        #endregion

        #region Constructors

        internal GamePad(PlayerIndex index)
        {
            this.gamePadIndex = index;
        }

        #endregion

        #region Methods

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
                this.thumbRightDelta = Vector2.Subtract(this.currentState.ThumbSticks.Right, this.previousState.ThumbSticks.Right);
                this.thumbLeftDelta = Vector2.Subtract(this.currentState.ThumbSticks.Left, this.previousState.ThumbSticks.Left);
                this.triggerRightDelta = this.currentState.Triggers.Right - this.previousState.Triggers.Right;
                this.triggerLeftDelta = this.currentState.Triggers.Left - this.previousState.Triggers.Left;
            }
        }

        public bool IsJustPressed(Buttons button)
        {
            return (this.previousState.IsButtonUp(button)) && (this.currentState.IsButtonDown(button));
        }

        public bool IsJustReleased(Buttons button)
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
