using System;

namespace Pulsar.Input
{
    public enum ButtonEvent
    {
        IsPressed,
        IsReleased,
        IsDown,
        IsUp
    }

    internal sealed class ButtonCommand : IInputCommand
    {
        #region Fields

        private ButtonEvent buttonEvent;
        private Button button;
        private CommandCheckState checkMethod;

        #endregion

        #region Constructors

        internal ButtonCommand(Button btn, ButtonEvent btnEvent)
        {
            this.button = btn;
            this.buttonEvent = btnEvent;
            this.AssignCheckMethod();
        }

        #endregion

        #region Methods

        private void AssignCheckMethod()
        {
            switch (this.buttonEvent)
            {
                case ButtonEvent.IsPressed: this.checkMethod = this.IsPressed;
                    break;
                case ButtonEvent.IsReleased: this.checkMethod = this.IsReleased;
                    break;
                case ButtonEvent.IsDown: this.checkMethod = this.IsDown;
                    break;
                case ButtonEvent.IsUp: this.checkMethod = this.IsUp;
                    break;
                default: throw new Exception("Invalid command event code provided");
                    break;
            }
        }

        private bool IsPressed()
        {
            return this.button.IsPressed;
        }

        private bool IsReleased()
        {
            return this.button.IsReleased;
        }

        private bool IsDown()
        {
            return this.button.IsDown;
        }

        private bool IsUp()
        {
            return this.button.IsUp;
        }

        #endregion

        #region Properties

        public bool IsTriggered
        {
            get { return this.checkMethod(); }
        }

        #endregion
    }
}
