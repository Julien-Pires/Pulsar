using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pulsar.Input
{
    public class Button
    {
        #region Fields

        private bool useDigital;
        private AnalogButton analogKey;
        private DigitalButton digitalKey;
        private bool previousDown;
        private bool down;
        private float pressedValue = 0.0f;

        #endregion

        #region Constructor

        public Button(DigitalButton k)
        {
            this.digitalKey = k;
            this.useDigital = true;
        }

        public Button(AnalogButton k)
        {
            this.analogKey = k;
        }

        #endregion

        #region Methods

        internal void Update()
        {
            
        }

        #endregion

        #region Properties

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

        public float PressedValue
        {
            get { return this.pressedValue; }
        }

        #endregion
    }
}
