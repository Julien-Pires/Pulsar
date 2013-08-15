using System.Collections.Generic;

namespace Pulsar.Input
{
    /// <summary>
    /// Describes a virtual button
    /// </summary>
    public sealed class Button
    {
        #region Nested

        /// <summary>
        /// Describe a button priority
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

            #region Methods

            /// <summary>
            /// Compare two instance of ButtonBinding
            /// </summary>
            /// <param name="first">First instance</param>
            /// <param name="second">Second instance</param>
            /// <returns>Return a value indicating the position of the first instance</returns>
            internal int Comparison(ButtonBinding first, ButtonBinding second)
            {
                if (first._priority < second._priority) return -1;
                if (first._priority > second._priority) return 1;

                return 0;
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
        /// Link a real button to the virtual button
        /// </summary>
        /// <param name="btn">Button instance</param>
        /// <param name="priority">Priority of the button</param>
        public void AddButton(AbstractButton btn, short priority)
        {
            ButtonBinding binding = new ButtonBinding(btn, priority);
            _hardwareButtons.Add(binding);
            _hardwareButtons.Sort(binding.Comparison);
        }

        /// <summary>
        /// Remove all real buttons
        /// </summary>
        public void RemoveAllButtons()
        {
            _hardwareButtons.Clear();
        }

        /// <summary>
        /// Update the button
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
        /// Get or set the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get the owner of the button
        /// </summary>
        public VirtualInput Owner { get; internal set; }

        /// <summary>
        /// Get or set the deadzone
        /// </summary>
        public float DeadZone { get; set; }

        /// <summary>
        /// Get a value that indicates if the button is down
        /// </summary>
        public bool IsDown
        {
            get { return _down; }
        }

        /// <summary>
        /// Get a value that indicates if the button is up
        /// </summary>
        public bool IsUp
        {
            get { return !_down; }
        }

        /// <summary>
        /// Get a value that indicates if the button has just been pressed
        /// </summary>
        public bool IsPressed
        {
            get { return !_previousDown && _down; }
        }

        /// <summary>
        /// Get a value that indicates if the button has just been released
        /// </summary>
        public bool IsReleased
        {
            get { return _previousDown && !_down; }
        }

        /// <summary>
        /// Get the value of the button
        /// </summary>
        public float Value
        {
            get { return _pressedValue; }
        }

        #endregion
    }
}
