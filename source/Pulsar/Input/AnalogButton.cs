using System;
using System.Runtime.InteropServices;

namespace Pulsar.Input
{
    [StructLayout(LayoutKind.Explicit)]
    public struct AnalogButton
    {
        #region Fields

        [FieldOffset(0)]
        public readonly InputDevice Device;

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
            this.Device = InputDevice.Mouse;
        }

        public AnalogButton(GamePadAnalogButtons btn)
        {
            this.gamePadButton = btn;
            this.mouseButton = 0;
            this.Device = InputDevice.GamePad;
        }

        #endregion

        #region Methods

        public float GetValue(int playerIndex)
        {
            switch (this.Device)
            {
                case InputDevice.Mouse: return Mouse.GetValue(this.mouseButton);
                    break;
                case InputDevice.GamePad:
                    GamePad pad = GamePad.GetGamePad(playerIndex);
                    return pad.GetValue(this.gamePadButton);
                    break;
            }

            return 0.0f;
        }

        #endregion
    }
}
