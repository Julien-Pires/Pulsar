using System;
using System.Runtime.InteropServices;

namespace Pulsar.Input
{
    [StructLayout(LayoutKind.Explicit)]
    public struct AnalogButton
    {
        #region Fields

        [FieldOffset(0)]
        private InputDevice device;

        [FieldOffset(4)]
        private MouseAnalogButtons mouseButton;

        [FieldOffset(4)]
        private GamePadAnalogButtons gamePadButton;

        #endregion

        #region Constructors

        public AnalogButton(MouseAnalogButtons btn)
        {
            this.gamePadButton = 0;
            this.mouseButton = btn;
            this.device = InputDevice.Mouse;
        }

        public AnalogButton(GamePadAnalogButtons btn)
        {
            this.gamePadButton = btn;
            this.mouseButton = 0;
            this.device = InputDevice.GamePad;
        }

        #endregion
    }
}
