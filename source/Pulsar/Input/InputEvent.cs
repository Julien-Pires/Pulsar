using System.Collections.Generic;

namespace Pulsar.Input
{
    /// <summary>
    /// Delegate used when an input action is triggerd
    /// </summary>
    /// <param name="inputEvent">InputEvent that trigger the event</param>
    /// <param name="propagate">Use to indicate that after the end of the method event should stop calling anothers</param>
    public delegate void InputEventFired(InputEvent inputEvent, ref bool propagate);

    /// <summary>
    /// Allows to trigger an action when input commands are triggered
    /// </summary>
    public sealed class InputEvent
    {
        #region Fields
        
        private List<IInputCommand> commands = new List<IInputCommand>();
        private readonly List<InputEventFired> _listeners = new List<InputEventFired>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of InputEvent class
        /// </summary>
        /// <param name="name">Name of the input action</param>
        /// <param name="owner">Owner of the input action</param>
        internal InputEvent(string name, VirtualInput owner)
        {
            this.Name = name;
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
                bool propagate = true;
                for (int i = 0; i < _listeners.Count; i++)
                {
                    _listeners[i](this, ref propagate);
                    if(!propagate) break;
                }
            }
        }

        public void AddListener(InputEventFired listener)
        {
            _listeners.Add(listener);
        }

        public bool RemoveListener(InputEventFired listener)
        {
            return _listeners.Remove(listener);
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

        #endregion
    }
}
