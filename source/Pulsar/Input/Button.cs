using System;
using System.Collections.Generic;

namespace Pulsar.Input
{
    public sealed class Button
    {
        #region Nested

        private class ButtonBinding
        {
            #region Fields

            public readonly short Priority;
            public readonly bool IsAnalogBinding;
            public readonly AnalogButton AnalogKey;
            public readonly DigitalButton DigitalKey;

            #endregion

            #region Constructors

            internal ButtonBinding(AnalogButton btn, short priority)
            {
                this.Priority = priority;
                this.IsAnalogBinding = true;
                this.AnalogKey = btn;
            }

            internal ButtonBinding(DigitalButton btn, short priority)
            {
                this.Priority = priority;
                this.DigitalKey = btn;
            }

            #endregion

            #region Methods

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
        private int player = 0;

        #endregion

        #region Methods

        public void AddButton(AnalogButton btn, short priority)
        {
            ButtonBinding binding = new ButtonBinding(btn, priority);
            this.hardwareButtons.Add(binding);
            this.hardwareButtons.Sort(binding.Comparison);
        }

        public void AddButton(DigitalButton btn, short priority)
        {
            ButtonBinding binding = new ButtonBinding(btn, priority);
            this.hardwareButtons.Add(binding);
            this.hardwareButtons.Sort(binding.Comparison);
        }

        public void RemoveAllButtons()
        {
            this.hardwareButtons.Clear();
        }

        internal void Update()
        {
            this.previousDown = this.down;

            bool activated = false;
            for (int i = 0; i < this.hardwareButtons.Count; i++)
            {
                ButtonBinding binding = this.hardwareButtons[i];
                if (!binding.IsAnalogBinding)
                {
                    this.down = binding.DigitalKey.IsDown(this.player);
                    if (this.down)
                    {
                        this.pressedValue = 1.0f;
                        activated = true;
                    }
                    else
                    {
                        this.pressedValue = 0.0f;
                    }
                }
                else
                {
                    this.pressedValue = binding.AnalogKey.GetValue(this.player);
                    this.down = this.pressedValue > this.deadZone;
                    if (this.down)
                    {
                        activated = true;
                    }
                }

                if (activated)
                {
                    break;
                }
            }
        }

        #endregion

        #region Properties

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

        public float DeadZone
        {
            get { return this.deadZone; }
            set { this.deadZone = value; }
        }

        public bool IsDown
        {
            get { return this.down; }
        }

        public bool IsUp
        {
            get { return !this.down; }
        }

        public bool IsPressed
        {
            get { return !this.previousDown && this.down; }
        }

        public bool IsReleased
        {
            get { return this.previousDown && !this.down; }
        }

        public float Value
        {
            get { return this.pressedValue; }
        }

        #endregion
    }
}
