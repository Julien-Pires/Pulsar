using System;

namespace Pulsar.Input
{
    public sealed class Axis
    {
        #region Fields

        private bool useDigital;
        private float value;
        private AnalogButton analogKey;
        private DigitalButton positiveDigitalKey;
        private DigitalButton negativeDigitalKey;
        private bool inverse;
        private float deadZone = 0.0f;
        private int player = 0;
        private float sensitivity = 1.0f;

        #endregion

        #region Constructor

        public Axis(AnalogButton analog)
        {
            this.analogKey = analog;
        }

        public Axis(DigitalButton positiveDigital, DigitalButton negativeDigital)
        {
            this.positiveDigitalKey = positiveDigital;
            this.negativeDigitalKey = negativeDigital;
            this.useDigital = true;
        }

        #endregion

        #region Methods

        internal void Update()
        {
            float rawValue = 0.0f;

            if (this.useDigital)
            {
                if (this.positiveDigitalKey.IsDown(this.player))
                {
                    rawValue += 1.0f;
                }
                if (this.negativeDigitalKey.IsDown(this.player))
                {
                    rawValue -= 1.0f;
                }

                if (this.inverse)
                {
                    rawValue *= -1.0f;
                }
                this.value = rawValue;
            }
            else
            {
                rawValue = this.analogKey.GetValue(this.player);

                if ((rawValue < this.deadZone) && (rawValue > -this.deadZone))
                {
                    rawValue = 0.0f;
                }
                if (this.inverse)
                {
                    rawValue *= -1.0f;
                }
                rawValue *= this.sensitivity;
                this.value = rawValue;
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

        public bool Inverse
        {
            get { return this.inverse; }
            set { this.inverse = value; }
        }

        public float Sensitivity
        {
            get { return this.sensitivity; }
            set { this.sensitivity = value; }
        }

        public float DeadZone
        {
            get { return this.deadZone; }
            set { this.deadZone = value; }
        }

        public float Value
        {
            get { return this.value; }
        }

        #endregion
    }
}
