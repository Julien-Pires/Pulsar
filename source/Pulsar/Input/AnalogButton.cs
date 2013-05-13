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
        private readonly MouseAnalogButtons mouseButton;

        [FieldOffset(4)]
        private readonly GamePadAnalogButtons gamePadButton;

        #endregion

        #region Constructors

        public AnalogButton(MouseAnalogButtons btn)
        {
            this.Device = InputDevice.Mouse;
            this.gamePadButton = 0;
            this.mouseButton = btn;
        }

        public AnalogButton(GamePadAnalogButtons btn)
        {
            this.Device = InputDevice.GamePad;
            this.mouseButton = 0;
            this.gamePadButton = btn;
        }

        #endregion

        #region Methods

        public float GetValue(int playerIndex)
        {
            switch (this.Device)
            {
                case InputDevice.Mouse: return Mouse.GetDeltaValue(this.mouseButton);
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
