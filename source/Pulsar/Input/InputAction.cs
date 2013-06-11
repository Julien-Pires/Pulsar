using System;

using System.Collections.Generic;

namespace Pulsar.Input
{
    /// <summary>
    /// Delegate used when an input action is triggerd
    /// </summary>
    /// <param name="action"></param>
    public delegate void InputActionFired(InputAction action);

    /// <summary>
    /// Allows to trigger an action when input commands are triggered
    /// </summary>
    public sealed class InputAction
    {
        #region Fields
        
        private List<IInputCommand> commands = new List<IInputCommand>();
        private InputActionFired actionMethod;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of InputAction class
        /// </summary>
        /// <param name="name">Name of the input action</param>
        /// <param name="action">Action to trigger</param>
        /// <param name="owner">Owner of the input action</param>
        internal InputAction(string name, InputActionFired action, VirtualInput owner)
        {
            this.Name = name;
            this.actionMethod = action;
            this.Owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the state
        /// </summary>
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

        /// <summary>
        /// Add a new command to check for
        /// </summary>
        /// <param name="buttonName">Name of the button associated with the command</param>
        /// <param name="btnEvent">Event to check for</param>
        public void AddCommand(string buttonName, ButtonEventType btnEvent)
        {
            Button btn = this.Owner.GetButton(buttonName);
            ButtonCommand command = new ButtonCommand(btn, btnEvent);
            this.commands.Add(command);
        }

        /// <summary>
        /// Add new command to check for
        /// </summary>
        /// <param name="axisName">Name of the axis associated with the command</param>
        /// <param name="axisEvent">Event to check for</param>
        public void AddCommand(string axisName, AxisEventType axisEvent)
        {
            Axis axis = this.Owner.GetAxis(axisName);
            AxisCommand command = new AxisCommand(axis, axisEvent);
            this.commands.Add(command);
        }

        /// <summary>
        /// Remove all commands
        /// </summary>
        public void Clear()
        {
            this.commands.Clear();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Get the owner
        /// </summary>
        public VirtualInput Owner { get; internal set; }

        /// <summary>
        /// Get the action delegate
        /// </summary>
        public InputActionFired ActionMethod
        {
            get { return this.actionMethod; }
        }

        #endregion
    }
}
