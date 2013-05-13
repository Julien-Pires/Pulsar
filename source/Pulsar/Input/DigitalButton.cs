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
        public readonly InputDevice Device;

        [FieldOffset(4)]
        private readonly Keys key;

        [FieldOffset(4)]
        private readonly MouseButtons mouseButton;

        [FieldOffset(4)]
        private readonly Buttons gamePadButton;

        #endregion

        #region Constructors

        public DigitalButton(Keys k)
        {
            this.Device = InputDevice.Keyboard;
            this.mouseButton = 0;
            this.gamePadButton = 0;
            this.key = k;
        }

        public DigitalButton(MouseButtons btn)
        {
            this.Device = InputDevice.Mouse;
            this.key = 0;
            this.gamePadButton = 0;
            this.mouseButton = btn;
        }

        public DigitalButton(Buttons btn)
        {
            this.Device = InputDevice.GamePad;
            this.key = 0;
            this.mouseButton = 0;
            this.gamePadButton = btn;
        }

        #endregion

        #region Methods

        public bool IsDown(int playerIndex)
        {
            switch (this.Device)
            {
                case InputDevice.Mouse: return Mouse.IsDown(this.mouseButton);
                    break;
                case InputDevice.Keyboard: return Keyboard.IsDown(this.key);
                    break;
                case InputDevice.GamePad:
                    GamePad pad = GamePad.GetGamePad(playerIndex);
                    return pad.IsDown(this.gamePadButton);
                    break;
            }

            return false;
        }

        #endregion
    }
}
