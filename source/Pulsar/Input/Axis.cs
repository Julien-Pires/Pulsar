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
            public readonly bool IsAnalogBinding;
            public readonly AnalogButton AnalogKey;
            public readonly DigitalButton NegativeButton;
            public readonly DigitalButton PositiveButton;

            #endregion

            #region Constructors

            internal AxisBinding(AnalogButton btn, short priority)
            {
                this.AnalogKey = btn;
                this.IsAnalogBinding = true;
                this.Priority = priority;
            }

            internal AxisBinding(DigitalButton negBtn, DigitalButton posBtn, short priority)
            {
                this.NegativeButton = negBtn;
                this.PositiveButton = posBtn;
                this.Priority = priority;
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
        private int player = 0;
        private float sensitivity = 1.0f;

        #endregion

        #region Methods

        public void AddButton(AnalogButton btn, short priority)
        {
            AxisBinding binding = new AxisBinding(btn, priority);
            this.hardwareButtons.Add(binding);
            this.hardwareButtons.Sort(binding.Comparison);
        }

        public void AddButton(DigitalButton negative, DigitalButton positive, short priority)
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
                if (!binding.IsAnalogBinding)
                {
                    if (binding.PositiveButton.IsDown(this.player))
                    {
                        rawValue += 1.0f;
                    }
                    if (binding.NegativeButton.IsDown(this.player))
                    {
                        rawValue -= 1.0f;
                    }

                    if (rawValue > 0.0f)
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
                    rawValue = binding.AnalogKey.GetValue(this.player);

                    if ((rawValue < this.deadZone) && (rawValue > -this.deadZone))
                    {
                        rawValue = 0.0f;
                    }

                    if (rawValue > 0.0f)
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

        public int PlayerIndex
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
