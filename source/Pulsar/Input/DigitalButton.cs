using System;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Input;

namespace Pulsar.Input
{
    [StructLayout(LayoutKind.Explicit)]
    public struct DigitalButton
    {
        #region Fields

        [FieldOffset(0)]
        private InputDevice device;

        [FieldOffset(4)]
        private Keys key;

        [FieldOffset(4)]
        private MouseButtons mouseButton;

        [FieldOffset(4)]
        private Buttons gamePadButton;

        #endregion

        #region Constructors

        public DigitalButton(Keys k)
        {
            this.device = InputDevice.Keyboard;
            this.key = k;
            this.mouseButton = 0;
            this.gamePadButton = 0;
        }

        public DigitalButton(MouseButtons btn)
        {
            this.device = InputDevice.Mouse;
            this.key = 0;
            this.mouseButton = btn;
            this.gamePadButton = 0;
        }

        public DigitalButton(Buttons btn)
        {
            this.device = InputDevice.GamePad;
            this.key = 0;
            this.mouseButton = 0;
            this.gamePadButton = btn;
        }

        #endregion
    }
}
