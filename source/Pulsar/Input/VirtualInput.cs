using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pulsar.Input
{
    /// <summary>
    /// Manages virtual button and axis
    /// </summary>
    public sealed class VirtualInput
    {
        #region Fields

        internal readonly PlayerIndex PlayerIndex;

        private readonly List<ButtonEvent> _internalButtonPressed = new List<ButtonEvent>(8);
        private readonly ReadOnlyCollection<ButtonEvent> _buttonPressed;
        private readonly Dictionary<string, int> _buttonsMap = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _axesMap = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _eventsMap = new Dictionary<string, int>();
        private readonly List<Button> _buttons = new List<Button>();
        private readonly List<Axis> _axes = new List<Axis>();
        private readonly List<InputEvent> _events = new List<InputEvent>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of VirtualInput class
        /// </summary>
        internal VirtualInput(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
            _buttonPressed = new ReadOnlyCollection<ButtonEvent>(_internalButtonPressed);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates virtual input states
        /// </summary>
        internal void Update()
        {
            for (int i = 0; i < _buttons.Count; i++)
                _buttons[i].Update();

            for (int i = 0; i < _axes.Count; i++)
                _axes[i].Update();

            for (int i = 0; i < _events.Count; i++)
                _events[i].Update();
        }

        /// <summary>
        /// Checks if any key has been pressed
        /// </summary>
        /// <returns></returns>
        public bool AnyKeyPressed()
        {
            return _internalButtonPressed.Count > 0;
        }

        /// <summary>
        /// Creates a new event
        /// </summary>
        /// <param name="name">Name of the event</param>
        /// <returns>Return an InputEvent instance</returns>
        public InputEvent CreateEvent(string name)
        {
            if (_eventsMap.ContainsKey(name))
                throw new Exception(string.Format("An event named {0} already exists", name));

            InputEvent newEvent = new InputEvent(name, this);
            _events.Add(newEvent);
            _eventsMap.Add(name, _events.Count - 1);

            return newEvent;
        }

        /// <summary>
        /// Removes an event
        /// </summary>
        /// <param name="name">Name of the event</param>
        /// <returns>Return true if the event is removed otherwise false</returns>
        public bool DestroyEvent(string name)
        {
            int idx;
            if (!_eventsMap.TryGetValue(name, out idx)) 
                return false;

            _events.RemoveAt(idx);
            _eventsMap.Remove(name);
            UpdateIndexMap(idx, _eventsMap);

            return true;
        }

        /// <summary>
        /// Gets an event
        /// </summary>
        /// <param name="name">Name of the event</param>
        /// <returns>Return an InputEvent instance</returns>
        public InputEvent GetEvent(string name)
        {
            int idx;
            if (!_eventsMap.TryGetValue(name, out idx)) 
                throw new Exception(string.Format("Failed to find an event named {0}", name));

            return _events[idx];
        }

        /// <summary>
        /// Adds a button with a specified name
        /// </summary>
        /// <param name="name">Button name</param>
        public Button CreateButton(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (_buttonsMap.ContainsKey(name))
                throw new Exception(string.Format("A button named {0} already exists in this virtual input", name));

            Button btn = new Button();
            _buttons.Add(btn);
            _buttonsMap.Add(name, _buttons.Count - 1);
            btn.Owner = this;

            return btn;
        }

        /// <summary>
        /// Adds an axis with a specified name
        /// </summary>
        /// <param name="name">Axis name</param>
        public Axis CreateAxis(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (_axesMap.ContainsKey(name)) 
                throw new Exception(string.Format("An axis named {0} already exists in this virtual input", name));

            Axis axis = new Axis();
            _axes.Add(axis);
            _axesMap.Add(name, _axes.Count - 1);
            axis.Owner = this;

            return axis;
        }

        /// <summary>
        /// Removes a button
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <returns>Return true if the button is removed otherwise false</returns>
        public bool DestroyButton(string name)
        {
            int idx;
            if (!_buttonsMap.TryGetValue(name, out idx)) 
                return false;

            Button btn = _buttons[idx];
            btn.Owner = null;
            _buttons.RemoveAt(idx);
            _buttonsMap.Remove(name);
            UpdateIndexMap(idx, _buttonsMap);

            return true;
        }

        /// <summary>
        /// Removes an axis
        /// </summary>
        /// <param name="name">Name of the axis</param>
        /// <returns>Return true if the axis is removed otherwise false</returns>
        public bool DestroyAxis(string name)
        {
            int idx;
            if (!_axesMap.TryGetValue(name, out idx)) 
                return false;

            Axis axis = _axes[idx];
            axis.Owner = null;
            _axes.RemoveAt(idx);
            _axesMap.Remove(name);
            UpdateIndexMap(idx, _axesMap);

            return true;
        }

        /// <summary>
        /// Gets a button
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <returns>Return a button instance</returns>
        public Button GetButton(string name)
        {
            int idx;
            if (!_buttonsMap.TryGetValue(name, out idx)) 
                throw new Exception(string.Format("Failed to find a button named {0}", name));

            return _buttons[idx];
        }

        /// <summary>
        /// Gets an axis
        /// </summary>
        /// <param name="name">Name of the axis</param>
        /// <returns>Return an axis instance</returns>
        public Axis GetAxis(string name)
        {
            int idx;
            if (!_axesMap.TryGetValue(name, out idx)) 
                throw new Exception(string.Format("Failed to find an axis named {0}", name));

            return _axes[idx];
        }

        /// <summary>
        /// Clears the pressed buttons list
        /// </summary>
        internal void ClearPressedButtons()
        {
            _internalButtonPressed.Clear();
        }

        /// <summary>
        /// Adds pressed buttons to this virtual input
        /// </summary>
        /// <param name="pressedButtons">List of pressed buttons</param>
        internal void AddPressedButtons(List<ButtonEvent> pressedButtons)
        {
            for(int i = 0; i < pressedButtons.Count; i++)
                _internalButtonPressed.Add(pressedButtons[i]);
        }

        /// <summary>
        /// Updates a dictionary used as an index dictionary when an index is removed
        /// </summary>
        /// <param name="idx">Removed index</param>
        /// <param name="map">Dictionary to update</param>
        private void UpdateIndexMap(int idx, Dictionary<string, int> map)
        {
            foreach (KeyValuePair<string, int> kvp in map)
            {
                if (kvp.Value > idx) 
                    map[kvp.Key] = kvp.Value - 1;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets all buttons that are currently being pressed for the device associated to this
        /// virtual input
        /// </summary>
        public ReadOnlyCollection<ButtonEvent> ButtonPressed
        {
            get { return _buttonPressed; }
        }

        /// <summary>
        /// Gets the device enum that the virtual input is listening to
        /// </summary>
        public InputDevice AssociatedDevice { get; set; }

        #endregion
    }
}
