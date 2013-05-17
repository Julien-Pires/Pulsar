using System;

using System.Collections.Generic;

namespace Pulsar.Input
{
    public delegate void InputActionFired(InputAction action);

    public sealed class InputAction
    {
        #region Fields
        
        private List<IInputCommand> commands = new List<IInputCommand>();
        private InputActionFired actionMethod;

        #endregion

        #region Methods

        internal void Update()
        {
            bool isTriggered = true;
            for (int i = 0; i < this.commands.Count; i++)
            {
                isTriggered &= this.commands[i].IsTriggered;
            }

            if (isTriggered)
            {
                this.actionMethod(this);
            }
        }

        public void AddCommand(string buttonName, ButtonEvent btnEvent)
        {
            Button btn = this.Owner.GetButton(buttonName);
            ButtonCommand command = new ButtonCommand(btn, btnEvent);
            this.commands.Add(command);
        }

        public void AddCommand(string axisName, AxisEvent axisEvent)
        {
            Axis axis = this.Owner.GetAxis(axisName);
            AxisCommand command = new AxisCommand(axis, axisEvent);
            this.commands.Add(command);
        }

        public void Clear()
        {
            this.commands.Clear();
        }

        #endregion

        #region Properties

        public string Name { get; internal set; }

        public VirtualInput Owner { get; internal set; }

        public InputActionFired ActionMethod
        {
            get { return this.actionMethod; }
            internal set { this.actionMethod = value; }
        }

        #endregion
    }
}
