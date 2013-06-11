using System;
using System.Collections.Generic;

namespace Pulsar.Input
{
    /// <summary>
    /// Describes an axis linked to one or more button
    /// </summary>
    public sealed class Axis
    {
        #region Nested

        /// <summary>
        /// Describes a set of button
        /// </summary>
        private class AxisBinding
        {
            #region Fields

            public readonly short Priority;
            public readonly AbstractButton negative;
            public readonly AbstractButton positive;
            private readonly bool digital;

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor of AxisBinding class
            /// </summary>
            /// <param name="button">AbstractButton instance</param>
            /// <param name="priority">Priority of the button</param>
            internal AxisBinding(AbstractButton button, short priority)
            {
                this.positive = button;
                this.Priority = priority;
            }

            /// <summary>
            /// Constructor of AxisBinding class
            /// </summary>
            /// <param name="negative">Button used to go in negative range</param>
            /// <param name="positive">Button used to go in positive range</param>
            /// <param name="priority">Priority of the button</param>
            internal AxisBinding(AbstractButton negative, AbstractButton positive, short priority)
            {
                this.negative = negative;
                this.positive = positive;
                this.Priority = priority;
                this.digital = true;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Compare two AxisBinding instance
            /// </summary>
            /// <param name="first">First instance</param>
            /// <param name="second">Second instance</param>
            /// <returns>Return a value indicating the position of the first instance</returns>
            internal int Comparison(AxisBinding first, AxisBinding second)
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

        private List<AxisBinding> hardwareButtons = new List<AxisBinding>();
        private float value;
        private bool inverse;
        private float deadZone = 0.0f;
        private short player = 0;
        private float sensitivity = 1.0f;

        #endregion

        #region Methods

        /// <summary>
        /// Link a new button to the axis
        /// </summary>
        /// <param name="btn">AbstractButton instance</param>
        /// <param name="priority">Priority of the button</param>
        public void AddButton(AbstractButton btn, short priority)
        {
            AxisBinding binding = new AxisBinding(btn, priority);
            this.hardwareButtons.Add(binding);
            this.hardwareButtons.Sort(binding.Comparison);
        }

        /// <summary>
        /// Link two button to the axis
        /// </summary>
        /// <param name="negative">Button used to go in negative range</param>
        /// <param name="positive">Button used to go in positive range</param>
        /// <param name="priority">Priority of the button</param>
        public void AddButton(AbstractButton negative, AbstractButton positive, short priority)
        {
            AxisBinding binding = new AxisBinding(negative, positive, priority);
            this.hardwareButtons.Add(binding);
            this.hardwareButtons.Sort(binding.Comparison);
        }

        /// <summary>
        /// Remove all buttons
        /// </summary>
        public void RemoveAllButtons()
        {
            this.hardwareButtons.Clear();
        }

        /// <summary>
        /// Update the axis
        /// </summary>
        internal void Update()
        {
            float rawValue = 0.0f;

            bool activated = false;
            for (int i = 0; i < this.hardwareButtons.Count; i++)
            {
                AxisBinding binding = this.hardwareButtons[i];
                AbstractButton positive = binding.positive;
                if (positive.Type == ButtonType.Digital)
                {
                    rawValue += positive.GetValue(this.player);
                    rawValue -= binding.negative.GetValue(this.player);

                    if (rawValue != 0.0f)
                    {
                        activated = true;
                        if (this.inverse)
                        {
                            rawValue *= -1.0f;
                        }
                    }
                }
                else
                {
                    rawValue = positive.GetValue(this.player);

                    if (positive.Device == InputDevice.GamePad)
                    {
                        if ((rawValue < this.deadZone) && (rawValue > -this.deadZone))
                        {
                            rawValue = 0.0f;
                        }
                    }

                    if (rawValue != 0.0f)
                    {
                        activated = true;
                        if (this.inverse)
                        {
                            rawValue *= -1.0f;
                        }
                        rawValue *= this.sensitivity;
                    }
                }
                this.value = rawValue;

                if (activated)
                {
                    break;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get or set the name of the axis
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get the owner of the axis
        /// </summary>
        public VirtualInput Owner { get; internal set; }

        /// <summary>
        /// Get or set the player index
        /// </summary>
        public short PlayerIndex
        {
            get { return this.player; }
            set
            {
                if (value < 0)
                {
                    throw new Exception("Player index can't be inferior to zero");
                }
                this.player = value;
            }
        }

        /// <summary>
        /// Get or set a value that indicates of the axis is inversed
        /// </summary>
        public bool Inverse
        {
            get { return this.inverse; }
            set { this.inverse = value; }
        }

        /// <summary>
        /// Get or set the sensitivity
        /// </summary>
        public float Sensitivity
        {
            get { return this.sensitivity; }
            set { this.sensitivity = value; }
        }

        /// <summary>
        /// Get or set the deadzone
        /// </summary>
        public float DeadZone
        {
            get { return this.deadZone; }
            set { this.deadZone = value; }
        }

        /// <summary>
        /// Get the value of the axis
        /// </summary>
        public float Value
        {
            get { return this.value; }
        }

        #endregion
    }
}
