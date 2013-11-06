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

        public readonly ReadOnlyCollection<ButtonEvent> ButtonPressed;

        internal readonly List<ButtonEvent> InternalButtonPressed = new List<ButtonEvent>();
        internal readonly PlayerIndex PlayerIndex;

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
            ButtonPressed = new ReadOnlyCollection<ButtonEvent>(InternalButtonPressed);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates virtual input states
        /// </summary>
        internal void Update()
        {
            InternalButtonPressed.Clear();
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
            return InternalButtonPressed.Count > 0;
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
            if (!_eventsMap.TryGetValue(name, out idx)) return false;

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
        /// Adds a button
        /// </summary>
        /// <param name="btn">Button instance</param>
        public void AddButton(Button btn)
        {
            if (string.IsNullOrEmpty(btn.Name)) 
                throw new Exception("Failed to add the button, his name is null or empty");

            if (_buttonsMap.ContainsKey(btn.Name)) 
                throw new Exception(string.Format("A button named {0} already exists in this virtual input", btn.Name)); 

            if (btn.Owner != null)
                btn.Owner.RemoveButton(btn.Name);
            _buttons.Add(btn);
            _buttonsMap.Add(btn.Name, _buttons.Count - 1);
            btn.Owner = this;
        }

        /// <summary>
        /// Adds an axis
        /// </summary>
        /// <param name="axis">Axis instance</param>
        public void AddAxis(Axis axis)
        {
            if(string.IsNullOrEmpty(axis.Name)) 
                throw new Exception("Failed to add the axis, his name is null or empty");

            if (_axesMap.ContainsKey(axis.Name)) 
                throw new Exception(string.Format("An axis named {0} already exists in this virtual input", axis.Name));

            if (axis.Owner != null)
                axis.Owner.RemoveAxis(axis.Name);
            _axes.Add(axis);
            _axesMap.Add(axis.Name, _axes.Count - 1);
            axis.Owner = this;
        }

        /// <summary>
        /// Removes a button
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <returns>Return true if the button is removed otherwise false</returns>
        public bool RemoveButton(string name)
        {
            int idx;
            if (!_buttonsMap.TryGetValue(name, out idx)) return false;

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
        public bool RemoveAxis(string name)
        {
            int idx;
            if (!_axesMap.TryGetValue(name, out idx)) return false;

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
        /// Gets the device enum that the virtual input is listening to
        /// </summary>
        public InputDevice AssociatedDevice { get; set; }

        #endregion
    }
}
