using System;
using System.Collections.Generic;

namespace Pulsar.Input
{
    /// <summary>
    /// Represents a virtual axis linked to one or more hardware buttons
    /// </summary>
    public sealed class Axis
    {
        #region Nested

        /// <summary>
        /// Describes a hardware button binding
        /// </summary>
        private class AxisBinding
        {
            #region Fields

            public readonly AbstractButton Negative;
            public readonly AbstractButton Positive;

            private readonly short _priority;

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor of AxisBinding class
            /// </summary>
            /// <param name="positive">AbstractButton instance</param>
            /// <param name="priority">Priority of the button</param>
            internal AxisBinding(AbstractButton positive, short priority)
            {
                Positive = positive;
                _priority = priority;
            }

            /// <summary>
            /// Constructor of AxisBinding class
            /// </summary>
            /// <param name="negative">Button used to go in negative range</param>
            /// <param name="positive">Button used to go in positive range</param>
            /// <param name="priority">Priority of the button</param>
            internal AxisBinding(AbstractButton negative, AbstractButton positive, short priority)
            {
                Negative = negative;
                Positive = positive;
                _priority = priority;
            }

            #endregion

            #region Static methods

            /// <summary>
            /// Compares two AxisBinding instance
            /// </summary>
            /// <param name="first">First instance</param>
            /// <param name="second">Second instance</param>
            /// <returns>Return a value indicating the position of the first instance</returns>
            internal static int Comparison(AxisBinding first, AxisBinding second)
            {
                if (first._priority < second._priority) return -1;

                return (first._priority > second._priority) ? 1 : 0;
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly List<AxisBinding> _hardwareButtons = new List<AxisBinding>();
        private float _value;
        private bool _inverse;
        private float _deadZone;
        private float _sensitivity = 1.0f;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Axis class
        /// </summary>
        internal Axis()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Links an analog button to the axis
        /// </summary>
        /// <param name="button">Analog button</param>
        /// <param name="priority">Priority of the button</param>
        public void AddAnalogButton(AbstractButton button, short priority)
        {
            if (button.Type != ButtonType.Analog) 
                throw new ArgumentException("Provided button is not analog");

            AxisBinding binding = new AxisBinding(button, priority);
            _hardwareButtons.Add(binding);
            _hardwareButtons.Sort(AxisBinding.Comparison);
        }

        /// <summary>
        /// Links two digital button to the axis
        /// </summary>
        /// <param name="negative">Button used to go in negative range</param>
        /// <param name="positive">Button used to go in positive range</param>
        /// <param name="priority">Priority of the button</param>
        public void AddDigitalButton(AbstractButton negative, AbstractButton positive, short priority)
        {
            if((negative.Type != ButtonType.Digital)) 
                throw new ArgumentException("Negative button is not digital");

            if ((positive.Type != ButtonType.Digital)) 
                throw new ArgumentException("Positive button is not digital");

            AxisBinding binding = new AxisBinding(negative, positive, priority);
            _hardwareButtons.Add(binding);
            _hardwareButtons.Sort(AxisBinding.Comparison);
        }

        /// <summary>
        /// Removes all buttons
        /// </summary>
        public void RemoveAllButtons()
        {
            _hardwareButtons.Clear();
        }

        /// <summary>
        /// Updates the axis
        /// </summary>
        internal void Update()
        {
            float rawValue = 0.0f;
            bool activated = false;
            for (int i = 0; i < _hardwareButtons.Count; i++)
            {
                AxisBinding binding = _hardwareButtons[i];
                AbstractButton positive = binding.Positive;
                if (positive.Type == ButtonType.Digital)
                {
                    AbstractButton negative = binding.Negative;
                    rawValue += positive.GetValue(Owner.PlayerIndex.GamePadIndex);
                    rawValue -= negative.GetValue(Owner.PlayerIndex.GamePadIndex);

                    if (Math.Abs(rawValue) > 0.0f)
                    {
                        activated = true;
                        if (_inverse) 
                            rawValue *= -1.0f;
                    }
                }
                else
                {
                    rawValue = positive.GetValue(Owner.PlayerIndex.GamePadIndex);

                    if (positive.Device == InputDevice.GamePad)
                    {
                        if ((rawValue < _deadZone) && (rawValue > -_deadZone)) 
                            rawValue = 0.0f;
                    }

                    if (Math.Abs(rawValue) > 0.0f)
                    {
                        activated = true;
                        if (_inverse)
                            rawValue *= -1.0f;
                        rawValue *= _sensitivity;
                    }
                }
                _value = rawValue;

                if (activated) 
                    break;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the owner of the axis
        /// </summary>
        public VirtualInput Owner { get; internal set; }

        /// <summary>
        /// Gets or sets a value that indicates of the axis is inversed
        /// </summary>
        public bool Inverse
        {
            get { return _inverse; }
            set { _inverse = value; }
        }

        /// <summary>
        /// Gets or sets the sensitivity
        /// </summary>
        public float Sensitivity
        {
            get { return _sensitivity; }
            set { _sensitivity = value; }
        }

        /// <summary>
        /// Gets or sets the deadzone
        /// </summary>
        public float DeadZone
        {
            get { return _deadZone; }
            set { _deadZone = value; }
        }

        /// <summary>
        /// Gets the value of the axis
        /// </summary>
        public float Value
        {
            get { return _value; }
        }

        #endregion
    }
}