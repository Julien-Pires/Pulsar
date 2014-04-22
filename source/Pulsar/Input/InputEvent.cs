using System.Collections.Generic;

namespace Pulsar.Input
{
    /// <summary>
    /// Represents an event that is triggered by a set of commands
    /// </summary>
    public sealed class InputEvent
    {
        #region Fields
        
        private readonly List<IInputCommand> _commands = new List<IInputCommand>();
        private readonly List<InputEventFired> _listeners = new List<InputEventFired>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of InputEvent class
        /// </summary>
        /// <param name="name">Name of the input event</param>
        /// <param name="owner">Owner of the input event</param>
        internal InputEvent(string name, VirtualInput owner)
        {
            Name = name;
            Owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the state of the event
        /// </summary>
        internal void Update()
        {
            bool isTriggered = true;
            for (int i = 0; i < _commands.Count; i++)
                isTriggered &= _commands[i].IsTriggered;

            if (!isTriggered) 
                return;

            bool propagate = true;
            for (int i = 0; i < _listeners.Count; i++)
            {
                _listeners[i](this, ref propagate);
                if(!propagate) 
                    break;
            }
        }

        /// <summary>
        /// Adds a listener for this event
        /// </summary>
        /// <param name="listener">Listener</param>
        public void AddListener(InputEventFired listener)
        {
            _listeners.Add(listener);
        }

        /// <summary>
        /// Removes a listener for this event
        /// </summary>
        /// <param name="listener">Listener</param>
        /// <returns>Returns true if the listener is removed otherwise false</returns>
        public bool RemoveListener(InputEventFired listener)
        {
            return _listeners.Remove(listener);
        }

        /// <summary>
        /// Adds a new command to check for
        /// </summary>
        /// <param name="buttonName">Name of the button associated with the command</param>
        /// <param name="btnEvent">Event to check for</param>
        public void CreateCommand(string buttonName, ButtonEventType btnEvent)
        {
            Button btn = Owner.GetButton(buttonName);
            ButtonCommand command = new ButtonCommand(btn, btnEvent);
            _commands.Add(command);
        }

        /// <summary>
        /// Adds a new command to check for
        /// </summary>
        /// <param name="axisName">Name of the axis associated with the command</param>
        /// <param name="axisEvent">Event to check for</param>
        public void CreateCommand(string axisName, AxisEventType axisEvent)
        {
            Axis axis = Owner.GetAxis(axisName);
            AxisCommand command = new AxisCommand(axis, axisEvent);
            _commands.Add(command);
        }

        /// <summary>
        /// Removes all commands
        /// </summary>
        public void Destroy()
        {
            _commands.Clear();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the owner
        /// </summary>
        public VirtualInput Owner { get; internal set; }

        #endregion
    }
}
