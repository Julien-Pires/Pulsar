using System.Collections.Generic;

namespace Pulsar.Input
{
    /// <summary>
    /// Represents a virtual button linked to one or more hardware buttons
    /// </summary>
    public sealed class Button
    {
        #region Nested

        /// <summary>
        /// Describes a hardware button binding
        /// </summary>
        private class ButtonBinding
        {
            #region Fields

            /// <summary>
            /// Button used by the binding
            /// </summary>
            public readonly AbstractButton Button;

            private readonly short _priority;

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor of ButtonBinding class
            /// </summary>
            /// <param name="btn">Button instance</param>
            /// <param name="priority">Priority of the button</param>
            internal ButtonBinding(AbstractButton btn, short priority)
            {
                Button = btn;
                _priority = priority;
            }

            #endregion

            #region Static Methods

            /// <summary>
            /// Compare two instance of ButtonBinding
            /// </summary>
            /// <param name="first">First instance</param>
            /// <param name="second">Second instance</param>
            /// <returns>Return a value indicating the position of the first instance</returns>
            internal static int Comparison(ButtonBinding first, ButtonBinding second)
            {
                if (first._priority < second._priority) return -1;

                return (first._priority > second._priority) ? 1 : 0;
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly List<ButtonBinding> _hardwareButtons = new List<ButtonBinding>();
        private bool _previousDown;
        private bool _down;
        private float _pressedValue;

        #endregion

        #region Methods

        /// <summary>
        /// Links a real button to the virtual button
        /// </summary>
        /// <param name="btn">Button instance</param>
        /// <param name="priority">Priority of the button</param>
        public void AddButton(AbstractButton btn, short priority)
        {
            ButtonBinding binding = new ButtonBinding(btn, priority);
            _hardwareButtons.Add(binding);
            _hardwareButtons.Sort(ButtonBinding.Comparison);
        }

        /// <summary>
        /// Removes all real buttons
        /// </summary>
        public void RemoveAllButtons()
        {
            _hardwareButtons.Clear();
        }

        /// <summary>
        /// Updates the button
        /// </summary>
        internal void Update()
        {
            _previousDown = _down;
            for (int i = 0; i < _hardwareButtons.Count; i++)
            {
                AbstractButton btn = _hardwareButtons[i].Button;
                if (btn.Type == ButtonType.Digital)
                {
                    _pressedValue = btn.GetValue(Owner.PlayerIndex.GamePadIndex);
                    _down = _pressedValue > 0.0f;
                }
                else
                {
                    _pressedValue = btn.GetValue(Owner.PlayerIndex.GamePadIndex);
                    _down = _pressedValue > DeadZone;
                }
                if (_down) break;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the owner of the button
        /// </summary>
        public VirtualInput Owner { get; internal set; }

        /// <summary>
        /// Gets or sets the deadzone
        /// </summary>
        public float DeadZone { get; set; }

        /// <summary>
        /// Gets a value that indicates if the button is down
        /// </summary>
        public bool IsDown
        {
            get { return _down; }
        }

        /// <summary>
        /// Gets a value that indicates if the button is up
        /// </summary>
        public bool IsUp
        {
            get { return !_down; }
        }

        /// <summary>
        /// Gets a value that indicates if the button has just been pressed
        /// </summary>
        public bool IsPressed
        {
            get { return !_previousDown && _down; }
        }

        /// <summary>
        /// Gets a value that indicates if the button has just been released
        /// </summary>
        public bool IsReleased
        {
            get { return _previousDown && !_down; }
        }

        /// <summary>
        /// Gets the value of the button
        /// </summary>
        public float Value
        {
            get { return _pressedValue; }
        }

        #endregion
    }
}
