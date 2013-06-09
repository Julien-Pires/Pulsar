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
            public readonly AbstractButton Button;

            #endregion

            #region Constructors

            internal ButtonBinding(AbstractButton btn, short priority)
            {
                this.Button = btn;
                this.Priority = priority;
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
        private short player = 0;

        #endregion

        #region Methods

        public void AddButton(AbstractButton btn, short priority)
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
                AbstractButton btn = this.hardwareButtons[i].Button;
                if (btn.Type == ButtonType.Digital)
                {
                    this.pressedValue = btn.GetValue(this.player);
                    this.down = this.pressedValue > 0.0f;
                }
                else
                {
                    this.pressedValue = btn.GetValue(this.player);
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
