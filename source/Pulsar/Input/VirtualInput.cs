using System;

using System.Collections.Generic;

namespace Pulsar.Input
{
    /// <summary>
    /// Manages virtual button and axis
    /// </summary>
    public sealed class VirtualInput
    {
        #region Fields

        internal List<ButtonEvent> ButtonPressed = new List<ButtonEvent>();
        private Dictionary<string, int> buttonsMap = new Dictionary<string, int>();
        private Dictionary<string, int> axesMap = new Dictionary<string, int>();
        private Dictionary<string, int> actionsMap = new Dictionary<string, int>();
        private List<Button> buttons = new List<Button>();
        private List<Axis> axes = new List<Axis>();
        private List<InputEvent> actions = new List<InputEvent>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of VirtualInput class
        /// </summary>
        internal VirtualInput()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update virtual input states
        /// </summary>
        internal void Update()
        {
            this.ButtonPressed.Clear();
            for (int i = 0; i < buttons.Count; i++)
            {
                this.buttons[i].Update();
            }

            for (int i = 0; i < axes.Count; i++)
            {
                this.axes[i].Update();
            }

            for (int i = 0; i < actions.Count; i++)
            {
                this.actions[i].Update();
            }
        }

        /// <summary>
        /// Check if any key has been pressed
        /// </summary>
        /// <returns></returns>
        public bool AnyKeyPressed()
        {
            return this.ButtonPressed.Count > 0;
        }

        /// <summary>
        /// Create a new action
        /// </summary>
        /// <param name="name">Name of the action</param>
        /// <param name="actionDelegate">Delegate to call when the action is triggered</param>
        /// <returns>Return an InputEvent instance</returns>
        public InputEvent CreateAction(string name)
        {
            if (this.actionsMap.ContainsKey(name))
            {
                throw new Exception(string.Format("An action named {0} already exists", name));
            }

            InputEvent action = new InputEvent(name, this);
            this.actions.Add(action);
            this.actionsMap.Add(name, this.actions.Count - 1);

            return action;
        }

        /// <summary>
        /// Remove an action
        /// </summary>
        /// <param name="name">Name of the action</param>
        /// <returns>Return true if the action is removed otherwise false</returns>
        public bool DeleteAction(string name)
        {
            int idx;
            if (!this.actionsMap.TryGetValue(name, out idx))
            {
                return false;
            }
            this.actions.RemoveAt(idx);
            this.actionsMap.Remove(name);
            this.UpdateIndexMap(idx, this.actionsMap);

            return true;
        }

        /// <summary>
        /// Get an action
        /// </summary>
        /// <param name="name">Name of the action</param>
        /// <returns>Return an InputEvent instance</returns>
        public InputEvent GetAction(string name)
        {
            int idx;
            if (!this.actionsMap.TryGetValue(name, out idx))
            {
                throw new Exception(string.Format("Failed to find an action named {0}", name));
            }

            return this.actions[idx];
        }

        /// <summary>
        /// Add a button
        /// </summary>
        /// <param name="btn">Button instance</param>
        public void AddButton(Button btn)
        {
            if (string.IsNullOrEmpty(btn.Name))
            {
                throw new Exception("Failed to add the button, his name is null or empty");
            }
            if (this.buttonsMap.ContainsKey(btn.Name))
            {
                throw new Exception(string.Format("A button named {0} already exists in this virtual input", btn.Name)); 
            }

            if (btn.Owner != null)
            {
                btn.Owner.RemoveButton(btn.Name);
            }
            this.buttons.Add(btn);
            this.buttonsMap.Add(btn.Name, this.buttons.Count - 1);
            btn.Owner = this;
        }

        /// <summary>
        /// Add an axis
        /// </summary>
        /// <param name="axis">Axis instance</param>
        public void AddAxis(Axis axis)
        {
            if(string.IsNullOrEmpty(axis.Name))
            {
                throw new Exception("Failed to add the axis, his name is null or empty");
            }
            if (this.axesMap.ContainsKey(axis.Name))
            {
                throw new Exception(string.Format("An axis named {0} already exists in this virtual input", axis.Name));
            }

            if (axis.Owner != null)
            {
                axis.Owner.RemoveAxis(axis.Name);
            }
            this.axes.Add(axis);
            this.axesMap.Add(axis.Name, this.axes.Count - 1);
            axis.Owner = this;
        }

        /// <summary>
        /// Remove a button
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <returns>Return true if the button is removed otherwise false</returns>
        public bool RemoveButton(string name)
        {
            int idx;
            if (!this.buttonsMap.TryGetValue(name, out idx))
            {
                return false;
            }
            Button btn = this.buttons[idx];
            btn.Owner = null;
            this.buttons.RemoveAt(idx);
            this.buttonsMap.Remove(name);
            this.UpdateIndexMap(idx, this.buttonsMap);

            return true;
        }

        /// <summary>
        /// Remove an axis
        /// </summary>
        /// <param name="name">Name of the axis</param>
        /// <returns>Return true if the axis is removed otherwise false</returns>
        public bool RemoveAxis(string name)
        {
            int idx;
            if (!this.axesMap.TryGetValue(name, out idx))
            {
                return false;
            }
            Axis axis = this.axes[idx];
            axis.Owner = null;
            this.axes.RemoveAt(idx);
            this.axesMap.Remove(name);
            this.UpdateIndexMap(idx, this.axesMap);

            return true;
        }

        /// <summary>
        /// Get a button
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <returns>Return a button instance</returns>
        public Button GetButton(string name)
        {
            int idx;
            if (!this.buttonsMap.TryGetValue(name, out idx))
            {
                throw new Exception(string.Format("Failed to find a button named {0}", name));
            }

            return this.buttons[idx];
        }

        /// <summary>
        /// Get an axis
        /// </summary>
        /// <param name="name">Name of the axis</param>
        /// <returns>Return an axis instance</returns>
        public Axis GetAxis(string name)
        {
            int idx;
            if (!this.axesMap.TryGetValue(name, out idx))
            {
                throw new Exception(string.Format("Failed to find an axis named {0}", name));
            }

            return this.axes[idx];
        }

        /// <summary>
        /// Update a dictionary used as an index dictionary when an index is removed
        /// </summary>
        /// <param name="idx">Removed index</param>
        /// <param name="map">Dictionary to update</param>
        private void UpdateIndexMap(int idx, Dictionary<string, int> map)
        {
            foreach (KeyValuePair<string, int> kvp in map)
            {
                if (kvp.Value > idx)
                {
                    map[kvp.Key] = kvp.Value - 1;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the device enum that the virtual input is listening to
        /// </summary>
        public InputDevice AssociatedDevice { get; set; }

        #endregion
    }
}
