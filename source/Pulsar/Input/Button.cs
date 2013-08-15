using System;
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
            /// Priority of the button
            /// </summary>
            public readonly short Priority;

            /// <summary>
            /// Button used by the binding
            /// </summary>
            public readonly AbstractButton Button;

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor of ButtonBinding class
            /// </summary>
            /// <param name="btn">Button instance</param>
            /// <param name="priority">Priority of the button</param>
            internal ButtonBinding(AbstractButton btn, short priority)
            {
                this.Button = btn;
                this.Priority = priority;
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
                if (first.Priority < second.Priority)
                {
                    return -1;
                }
                else if (first.Priority > second.Priority)
                {
                    return 1;
                }

                return 0;
            }

            #endregion
        }

        #endregion

        #region Fields

        private List<ButtonBinding> hardwareButtons = new List<ButtonBinding>();
        private bool previousDown = false;
        private bool down = false;
        private float pressedValue = 0.0f;
        private float deadZone = 0.0f;

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
            this.hardwareButtons.Add(binding);
            this.hardwareButtons.Sort(binding.Comparison);
        }

        /// <summary>
        /// Remove all real buttons
        /// </summary>
        public void RemoveAllButtons()
        {
            this.hardwareButtons.Clear();
        }

        /// <summary>
        /// Update the button
        /// </summary>
        internal void Update()
        {
            this.previousDown = this.down;

            bool activated = false;
            for (int i = 0; i < this.hardwareButtons.Count; i++)
            {
                AbstractButton btn = this.hardwareButtons[i].Button;
                if (btn.Type == ButtonType.Digital)
                {
                    this.pressedValue = btn.GetValue(Owner.PlayerIndex.GamePadIndex);
                    this.down = this.pressedValue > 0.0f;
                }
                else
                {
                    this.pressedValue = btn.GetValue(Owner.PlayerIndex.GamePadIndex);
                    this.down = this.pressedValue > this.deadZone;
                }
                activated = this.down;

                if (activated)
                {
                    break;
                }
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
        public float DeadZone
        {
            get { return this.deadZone; }
            set { this.deadZone = value; }
        }

        /// <summary>
        /// Get a value that indicates if the button is down
        /// </summary>
        public bool IsDown
        {
            get { return this.down; }
        }

        /// <summary>
        /// Get a value that indicates if the button is up
        /// </summary>
        public bool IsUp
        {
            get { return !this.down; }
        }

        /// <summary>
        /// Get a value that indicates if the button has just been pressed
        /// </summary>
        public bool IsPressed
        {
            get { return !this.previousDown && this.down; }
        }

        /// <summary>
        /// Get a value that indicates if the button has just been released
        /// </summary>
        public bool IsReleased
        {
            get { return this.previousDown && !this.down; }
        }

        /// <summary>
        /// Get the value of the button
        /// </summary>
        public float Value
        {
            get { return this.pressedValue; }
        }

        #endregion
    }
}
