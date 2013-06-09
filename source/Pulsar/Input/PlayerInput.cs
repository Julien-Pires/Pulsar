using System;

using System.Collections.Generic;

namespace Pulsar.Input
{
    public sealed class PlayerInput
    {
        #region Fields

        private Dictionary<string, VirtualInput> contextMap = new Dictionary<string, VirtualInput>();
        private VirtualInput currentContext;

        #endregion

        #region Constructors

        internal PlayerInput()
        {
        }

        #endregion

        #region Methods

        internal void Update()
        {
            if (this.currentContext != null)
            {
                this.currentContext.Update();
                this.DispatchButtonEvent();
            }
        }

        private void DispatchButtonEvent()
        {
            InputDevice device = this.currentContext.AssociatedDevice;
#if WINDOWS
            if ((device & InputDevice.Mouse) == InputDevice.Mouse)
            {
                for (short i = 0; i < Mouse.ButtonPressed.Count; i++)
                {
                    this.currentContext.ButtonPressed.Add(Mouse.ButtonPressed[i]);
                }
            }
#endif
            if ((device & InputDevice.Keyboard) == InputDevice.Keyboard)
            {
                for (short i = 0; i < Keyboard.ButtonPressed.Count; i++)
                {
                    this.currentContext.ButtonPressed.Add(Keyboard.ButtonPressed[i]);
                }
            }
            if ((device & InputDevice.GamePad) == InputDevice.GamePad)
            {
                for(short i = 0; i < GamePad.ButtonPressed.Count; i++)
                {
                    ButtonEvent btnEvent = GamePad.ButtonPressed[i];
                    if (btnEvent.Index == this.PlayerIndex)
                    {
                        this.currentContext.ButtonPressed.Add(btnEvent);
                    }
                }
            }
            else if ((device & InputDevice.AllGamePad) == InputDevice.AllGamePad)
            {
                for (short i = 0; i < GamePad.ButtonPressed.Count; i++)
                {
                    this.currentContext.ButtonPressed.Add(GamePad.ButtonPressed[i]);
                }
            }
        }

        public void Reset()
        {
            this.contextMap.Clear();
            this.currentContext = null;
        }

        public VirtualInput CreateContext(string name)
        {
            if (this.contextMap.ContainsKey(name))
            {
                throw new Exception(string.Format("A context named {0} already exists", name));
            }
            VirtualInput vInput = new VirtualInput();
            this.contextMap.Add(name, vInput);

            return vInput;
        }

        public bool RemoveContext(string name)
        {
            return this.contextMap.Remove(name);
        }

        public VirtualInput GetContext(string name)
        {
            VirtualInput input;
            if (!this.contextMap.TryGetValue(name, out input))
            {
                throw new Exception(string.Format("Failed to find a context named {0}", name));
            }

            return input;
        }

        public void SetCurrentContext(string name)
        {
            VirtualInput input;
            if (!this.contextMap.TryGetValue(name, out input))
            {
                throw new Exception(string.Format("Failed to find a context named {0}", name));
            }

            this.currentContext = input;
        }

        public Button GetButton(string name)
        {
            if (this.currentContext == null)
            {
                throw new Exception("No current context set");
            }

            return this.currentContext.GetButton(name);
        }

        public Axis GetAxis(string name)
        {
            if (this.currentContext == null)
            {
                throw new Exception("No current context set");
            }

            return this.currentContext.GetAxis(name);
        }
        
        #endregion

        #region Properties

        public short PlayerIndex { get; internal set; }

        public VirtualInput CurrentContext
        {
            get { return this.currentContext; }
        }

        #endregion
    }
}
