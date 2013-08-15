using Microsoft.Xna.Framework.Input;

namespace Pulsar.Input
{
    /// <summary>
    /// Describe a type of button
    /// </summary>
    public enum ButtonType : byte
    {
        Analog,
        Digital
    }

    /// <summary>
    /// Allows to abstract any button for use in virtual input
    /// </summary>
    public struct AbstractButton
    {
        #region Fields

        /// <summary>
        /// The device associated with the button
        /// </summary>
        public readonly InputDevice Device;

        /// <summary>
        /// The type of button
        /// </summary>
        public readonly ButtonType Type;

        /// <summary>
        /// The button code
        /// </summary>
        public readonly int ButtonCode;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AbstractButton struct
        /// </summary>
        /// <param name="button">Underlying button</param>
        public AbstractButton(MouseButtons button)
        {
            Device = InputDevice.Mouse;
            Type = ButtonType.Digital;
            ButtonCode = (int)button;
        }

        /// <summary>
        /// Constructor of AbstractButton struct
        /// </summary>
        /// <param name="button">Underlying button</param>
        public AbstractButton(MouseAnalogButtons button)
        {
            Device = InputDevice.Mouse;
            Type = ButtonType.Analog;
            ButtonCode = (int)button;
        }

        /// <summary>
        /// Constructor of AbstractButton struct
        /// </summary>
        /// <param name="button">Underlying button</param>
        public AbstractButton(Keys button)
        {
            Device = InputDevice.Keyboard;
            Type = ButtonType.Digital;
            ButtonCode = (int)button;
        }

        /// <summary>
        /// Constructor of AbstractButton struct
        /// </summary>
        /// <param name="button">Underlying button</param>
        public AbstractButton(AnalogButtons button)
        {
            Device = InputDevice.GamePad;
            Type = ButtonType.Analog;
            ButtonCode = (int)button;
        }

        /// <summary>
        /// Constructor of AbstractButton struct
        /// </summary>
        /// <param name="button">Underlying button</param>
        public AbstractButton(Buttons button)
        {
            Device = InputDevice.GamePad;
            Type = ButtonType.Digital;
            ButtonCode = (int)button;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the value of the underlying button
        /// </summary>
        /// <param name="player">Index of the player</param>
        /// <returns>Return the value of the button</returns>
        public float GetValue(short player)
        {
            float val = 0.0f;
            if (Type == ButtonType.Digital)
            {
                switch (Device)
                {
                    case InputDevice.Mouse: val = Mouse.IsDown((MouseButtons)ButtonCode) ? 1.0f : 0.0f;
                        break;
                    case InputDevice.Keyboard: val = Keyboard.IsDown((Keys)ButtonCode) ? 1.0f : 0.0f;
                        break;
                    case InputDevice.GamePad:
                        GamePad pad = GamePad.GetGamePad(player);
                        val = pad.IsDown((Buttons)ButtonCode) ? 1.0f : 0.0f;
                        break;
                }
            }
            else
            {
                switch (Device)
                {
                    case InputDevice.Mouse: val = Mouse.GetDeltaValue((MouseAnalogButtons)ButtonCode);
                        break;
                    case InputDevice.GamePad:
                        GamePad pad = GamePad.GetGamePad(player);
                        val = pad.GetValue((AnalogButtons)ButtonCode);
                        break;
                }
            }

            return val;
        }

        public bool IsPressed(short player)
        {
            if (Type != ButtonType.Digital) return false;

            bool val = false;
            switch (Device)
            {
                case InputDevice.Mouse: val = Mouse.IsPressed((MouseButtons)ButtonCode);
                    break;
                case InputDevice.Keyboard: val = Keyboard.IsPressed((Keys)ButtonCode);
                    break;
                case InputDevice.GamePad:
                    GamePad pad = GamePad.GetGamePad(player);
                    val = pad.IsPressed((Buttons)ButtonCode);
                    break;
            }

            return val;
        }

        public bool IsReleased(short player)
        {
            if (Type != ButtonType.Digital) return false;

            bool val = false;
            switch (Device)
            {
                case InputDevice.Mouse: val = Mouse.IsReleased((MouseButtons)ButtonCode);
                    break;
                case InputDevice.Keyboard: val = Keyboard.IsReleased((Keys)ButtonCode);
                    break;
                case InputDevice.GamePad:
                    GamePad pad = GamePad.GetGamePad(player);
                    val = pad.IsReleased((Buttons)ButtonCode);
                    break;
            }

            return val;
        }

        #endregion
    }
}
