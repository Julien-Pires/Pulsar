using System;

using System.Collections.Generic;

namespace Pulsar.Input
{
    public sealed class VirtualInput
    {
        #region Fields

        internal List<ButtonEvent> ButtonPressed = new List<ButtonEvent>();
        private Dictionary<string, int> buttonsMap = new Dictionary<string, int>();
        private Dictionary<string, int> axesMap = new Dictionary<string, int>();
        private Dictionary<string, int> actionsMap = new Dictionary<string, int>();
        private List<Button> buttons = new List<Button>();
        private List<Axis> axes = new List<Axis>();
        private List<InputAction> actions = new List<InputAction>();

        #endregion

        #region Constructors

        internal VirtualInput()
        {
        }

        #endregion

        #region Methods

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

        public bool AnyKeyPressed()
        {
            return this.ButtonPressed.Count > 0;
        }

        public InputAction CreateAction(string name, InputActionFired actionDelegate)
        {
            if (this.actionsMap.ContainsKey(name))
            {
                throw new Exception(string.Format("An action named {0} already exists", name));
            }

            InputAction action = new InputAction(name, actionDelegate, this);
            this.actions.Add(action);
            this.actionsMap.Add(name, this.actions.Count - 1);

            return action;
        }

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

        public InputAction GetAction(string name)
        {
            int idx;
            if (!this.actionsMap.TryGetValue(name, out idx))
            {
                throw new Exception(string.Format("Failed to find an action named {0}", name));
            }

            return this.actions[idx];
        }

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

        public Button GetButton(string name)
        {
            int idx;
            if (!this.buttonsMap.TryGetValue(name, out idx))
            {
                throw new Exception(string.Format("Failed to find a button named {0}", name));
            }

            return this.buttons[idx];
        }

        public Axis GetAxis(string name)
        {
            int idx;
            if (!this.axesMap.TryGetValue(name, out idx))
            {
                throw new Exception(string.Format("Failed to find an axis named {0}", name));
            }

            return this.axes[idx];
        }

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

        public InputDevice AssociatedDevice { get; set; }

        #endregion
    }
}
