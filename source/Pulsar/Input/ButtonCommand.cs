using System;

namespace Pulsar.Input
{
    /// <summary>
    /// Type of event for a button
    /// </summary>
    public enum ButtonEventType : byte
    {
        IsPressed,
        IsReleased,
        IsDown,
        IsUp
    }

    /// <summary>
    /// Check the state of a button against a specific event
    /// </summary>
    internal sealed class ButtonCommand : IInputCommand
    {
        #region Fields

        private ButtonEventType buttonEvent;
        private Button button;
        private CheckCommandState checkMethod;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ButtonCommand class
        /// </summary>
        /// <param name="btn">Button instance</param>
        /// <param name="btnEvent">Type of event</param>
        internal ButtonCommand(Button btn, ButtonEventType btnEvent)
        {
            this.button = btn;
            this.buttonEvent = btnEvent;
            this.AssignCheckMethod();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Assign a method that check the state of the button for a specific event
        /// </summary>
        private void AssignCheckMethod()
        {
            switch (this.buttonEvent)
            {
                case ButtonEventType.IsPressed: this.checkMethod = this.IsPressed;
                    break;
                case ButtonEventType.IsReleased: this.checkMethod = this.IsReleased;
                    break;
                case ButtonEventType.IsDown: this.checkMethod = this.IsDown;
                    break;
                case ButtonEventType.IsUp: this.checkMethod = this.IsUp;
                    break;
                default: throw new Exception("Invalid command event code provided");
                    break;
            }
        }

        /// <summary>
        /// Check if the button has just been pressed
        /// </summary>
        /// <returns></returns>
        private bool IsPressed()
        {
            return this.button.IsPressed;
        }

        /// <summary>
        /// Check if the button has just been released
        /// </summary>
        /// <returns></returns>
        private bool IsReleased()
        {
            return this.button.IsReleased;
        }

        /// <summary>
        /// Check if the button is down
        /// </summary>
        /// <returns></returns>
        private bool IsDown()
        {
            return this.button.IsDown;
        }

        /// <summary>
        /// Check if the button is up
        /// </summary>
        /// <returns></returns>
        private bool IsUp()
        {
            return this.button.IsUp;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get a value that indicates if the commande has been triggered
        /// </summary>
        public bool IsTriggered
        {
            get { return this.checkMethod(); }
        }

        #endregion
    }
}
