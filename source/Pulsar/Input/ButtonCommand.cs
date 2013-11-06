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
    /// Checks the state of a button for a specific event
    /// </summary>
    internal sealed class ButtonCommand : IInputCommand
    {
        #region Fields

        private readonly ButtonEventType _buttonEvent;
        private readonly Button _button;
        private CheckCommandState _checkMethod;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ButtonCommand class
        /// </summary>
        /// <param name="btn">Button instance</param>
        /// <param name="btnEvent">Type of event</param>
        internal ButtonCommand(Button btn, ButtonEventType btnEvent)
        {
            _button = btn;
            _buttonEvent = btnEvent;
            AssignCheckMethod();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Assignes a method that check the state of the button for a specific event
        /// </summary>
        private void AssignCheckMethod()
        {
            switch (_buttonEvent)
            {
                case ButtonEventType.IsPressed: 
                    _checkMethod = IsPressed;
                    break;
                case ButtonEventType.IsReleased:
                    _checkMethod = IsReleased;
                    break;
                case ButtonEventType.IsDown: 
                    _checkMethod = IsDown;
                    break;
                case ButtonEventType.IsUp: 
                    _checkMethod = IsUp;
                    break;
                default: 
                    throw new Exception("Invalid command event code provided");
            }
        }

        /// <summary>
        /// Checks if the button has just been pressed
        /// </summary>
        /// <returns></returns>
        private bool IsPressed()
        {
            return _button.IsPressed;
        }

        /// <summary>
        /// Checks if the button has just been released
        /// </summary>
        /// <returns></returns>
        private bool IsReleased()
        {
            return _button.IsReleased;
        }

        /// <summary>
        /// Checks if the button is down
        /// </summary>
        /// <returns></returns>
        private bool IsDown()
        {
            return _button.IsDown;
        }

        /// <summary>
        /// Checks if the button is up
        /// </summary>
        /// <returns></returns>
        private bool IsUp()
        {
            return _button.IsUp;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that indicates if the commande has been triggered
        /// </summary>
        public bool IsTriggered
        {
            get { return _checkMethod(); }
        }

        #endregion
    }
}
