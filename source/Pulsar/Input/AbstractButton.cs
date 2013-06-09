using System;

using Microsoft.Xna.Framework.Input;

namespace Pulsar.Input
{
    public enum ButtonType
    {
        Analog,
        Digital
    }

    public struct AbstractButton
    {
        #region Fields

        public readonly InputDevice Device;

        public readonly ButtonType Type;

        public readonly int ButtonCode;

        #endregion

        #region Constructors

        public AbstractButton(MouseButtons button)
        {
            this.Device = InputDevice.Mouse;
            this.Type = ButtonType.Digital;
            this.ButtonCode = (int)button;
        }

        public AbstractButton(MouseAnalogButtons button)
        {
            this.Device = InputDevice.Mouse;
            this.Type = ButtonType.Analog;
            this.ButtonCode = (int)button;
        }

        public AbstractButton(Keys button)
        {
            this.Device = InputDevice.Keyboard;
            this.Type = ButtonType.Digital;
            this.ButtonCode = (int)button;
        }

        public AbstractButton(AnalogButtons button)
        {
            this.Device = InputDevice.GamePad;
            this.Type = ButtonType.Analog;
            this.ButtonCode = (int)button;
        }

        public AbstractButton(Buttons button)
        {
            this.Device = InputDevice.GamePad;
            this.Type = ButtonType.Digital;
            this.ButtonCode = (int)button;
        }

        #endregion

        #region Methods

        public float GetValue(short player)
        {
            float val = 0.0f;
            if (this.Type == ButtonType.Digital)
            {
                switch (this.Device)
                {
                    case InputDevice.Mouse: val = Mouse.IsDown((MouseButtons)this.ButtonCode) ? 1.0f : 0.0f;
                        break;
                    case InputDevice.Keyboard: val = Keyboard.IsDown((Keys)this.ButtonCode) ? 1.0f : 0.0f;
                        break;
                    case InputDevice.GamePad:
                        GamePad pad = GamePad.GetGamePad(player);
                        val = pad.IsDown((Buttons)this.ButtonCode) ? 1.0f : 0.0f;
                        break;
                }
            }
            else
            {
                switch (this.Device)
                {
                    case InputDevice.Mouse: val = Mouse.GetDeltaValue((MouseAnalogButtons)this.ButtonCode);
                        break;
                    case InputDevice.GamePad:
                        GamePad pad = GamePad.GetGamePad(player);
                        val = pad.GetValue((AnalogButtons)this.ButtonCode);
                        break;
                }
            }

            return val;
        }

        public bool IsPressed(short player)
        {
            bool val = false;
            if (this.Type == ButtonType.Digital)
            {
                switch (this.Device)
                {
                    case InputDevice.Mouse: val = Mouse.IsPressed((MouseButtons)this.ButtonCode);
                        break;
                    case InputDevice.Keyboard: val = Keyboard.IsPressed((Keys)this.ButtonCode);
                        break;
                    case InputDevice.GamePad:
                        GamePad pad = GamePad.GetGamePad(player);
                        val = pad.IsPressed((Buttons)this.ButtonCode);
                        break;
                }
            }

            return val;
        }

        public bool IsReleased(short player)
        {
            bool val = false;
            if (this.Type == ButtonType.Digital)
            {
                switch (this.Device)
                {
                    case InputDevice.Mouse: val = Mouse.IsReleased((MouseButtons)this.ButtonCode);
                        break;
                    case InputDevice.Keyboard: val = Keyboard.IsReleased((Keys)this.ButtonCode);
                        break;
                    case InputDevice.GamePad:
                        GamePad pad = GamePad.GetGamePad(player);
                        val = pad.IsReleased((Buttons)this.ButtonCode);
                        break;
                }
            }

            return val;
        }

        #endregion
    }
}
