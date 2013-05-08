using System;

using System.Collections.Generic;

namespace Pulsar.Input
{
    public sealed class VirtualInput
    {
        #region Fields

        private Dictionary<string, int> buttonsMap = new Dictionary<string, int>();
        private Dictionary<string, int> axesMap = new Dictionary<string, int>();
        private List<Button> buttons = new List<Button>();
        private List<Axis> axes = new List<Axis>();

        #endregion

        #region Methods

        internal void Update()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                this.buttons[i].Update();
            }

            for (int i = 0; i < axes.Count; i++)
            {
                this.axes[i].Update();
            }
        }

        public void AddButton(string name, Button btn)
        {
            int idx;
            if(this.buttonsMap.TryGetValue(name, out idx))
            {
                throw new Exception(string.Format("A button named {0} already exists in this virtual input", name)); 
            }
            this.buttons.Add(btn);
            this.buttonsMap.Add(name, this.buttons.Count - 1);
        }

        public void AddAxis(string name, Axis axis)
        {
            int idx;
            if (this.axesMap.TryGetValue(name, out idx))
            {
                throw new Exception(string.Format("An axis named {0} already exists in this virtual input", name));
            }
            this.axes.Add(axis);
            this.axesMap.Add(name, this.axes.Count - 1);
        }

        public bool RemoveButton(string name)
        {
            int idx;
            if (!this.buttonsMap.TryGetValue(name, out idx))
            {
                return false;
            }
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
    }
}
