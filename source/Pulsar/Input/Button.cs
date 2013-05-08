using System;

namespace Pulsar.Input
{
    public sealed class Button
    {
        #region Fields

        private bool useDigital = false;
        private AnalogButton analogKey;
        private DigitalButton digitalKey;
        private bool previousDown = false;
        private bool down = false;
        private float pressedValue = 0.0f;
        private float deadZone = 0.0f;
        private int player = 0;

        #endregion

        #region Constructor

        public Button(DigitalButton digital)
        {
            this.digitalKey = digital;
            this.useDigital = true;
        }

        public Button(AnalogButton analog)
        {
            this.analogKey = analog;
        }

        #endregion

        #region Methods

        internal void Update()
        {
            this.previousDown = this.down;

            if (this.useDigital)
            {
                this.down = this.digitalKey.IsDown(this.player);
                if (this.down)
                {
                    this.pressedValue = 1.0f;
                }
                else
                {
                    this.pressedValue = 0.0f;
                }
            }
            else
            {
                this.pressedValue = this.analogKey.GetValue(this.player);
                this.down = this.pressedValue > this.deadZone;
            }
        }

        #endregion

        #region Properties

        public int PlayerIndex
        {
            get { return this.player; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Player index can't be inferior to zero");
                }
                this.player = value;
            }
        }

        public float DeadZone
        {
            get { return this.deadZone; }
            set { this.deadZone = value; }
        }

        public bool IsDown
        {
            get { return this.down; }
        }

        public bool IsUp
        {
            get { return !this.down; }
        }

        public bool IsPressed
        {
            get { return !this.previousDown && this.down; }
        }

        public bool IsReleased
        {
            get { return this.previousDown && !this.down; }
        }

        public float Value
        {
            get { return this.pressedValue; }
        }

        #endregion
    }
}
