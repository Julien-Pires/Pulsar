using System;
using System.Collections.Generic;

namespace Pulsar.Input
{
    public sealed class Axis
    {
        #region Nested

        private class AxisBinding
        {
            #region Fields

            public readonly short Priority;
            public readonly AbstractButton negative;
            public readonly AbstractButton positive;
            private readonly bool digital;

            #endregion

            #region Constructors

            internal AxisBinding(AbstractButton button, short priority)
            {
                this.positive = button;
                this.Priority = priority;
            }

            internal AxisBinding(AbstractButton negative, AbstractButton positive, short priority)
            {
                this.negative = negative;
                this.positive = positive;
                this.Priority = priority;
                this.digital = true;
            }

            #endregion

            #region Methods

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

        public void AddButton(AbstractButton btn, short priority)
        {
            AxisBinding binding = new AxisBinding(btn, priority);
            this.hardwareButtons.Add(binding);
            this.hardwareButtons.Sort(binding.Comparison);
        }

        public void AddButton(AbstractButton negative, AbstractButton positive, short priority)
        {
            AxisBinding binding = new AxisBinding(negative, positive, priority);
            this.hardwareButtons.Add(binding);
            this.hardwareButtons.Sort(binding.Comparison);
        }

        public void RemoveAllButtons()
        {
            this.hardwareButtons.Clear();
        }

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

        public string Name { get; set; }

        public VirtualInput Owner { get; internal set; }

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

        public bool Inverse
        {
            get { return this.inverse; }
            set { this.inverse = value; }
        }

        public float Sensitivity
        {
            get { return this.sensitivity; }
            set { this.sensitivity = value; }
        }

        public float DeadZone
        {
            get { return this.deadZone; }
            set { this.deadZone = value; }
        }

        public float Value
        {
            get { return this.value; }
        }

        #endregion
    }
}
