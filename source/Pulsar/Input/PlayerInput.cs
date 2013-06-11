using System;

using System.Collections.Generic;

namespace Pulsar.Input
{
    /// <summary>
    /// Represents one player and contains all his input context
    /// </summary>
    public sealed class PlayerInput
    {
        #region Fields

        private Dictionary<string, VirtualInput> contextMap = new Dictionary<string, VirtualInput>();
        private VirtualInput currentContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of PlayerInput class
        /// </summary>
        internal PlayerInput()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update player context
        /// </summary>
        internal void Update()
        {
            if (this.currentContext != null)
            {
                this.currentContext.Update();
                this.DispatchButtonEvent();
            }
        }

        /// <summary>
        /// Dispatch button event from device to the current context
        /// </summary>
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

        /// <summary>
        /// Remove all context and reset the player
        /// </summary>
        public void Reset()
        {
            this.contextMap.Clear();
            this.currentContext = null;
        }

        /// <summary>
        /// Create a new context
        /// </summary>
        /// <param name="name">Name of the context</param>
        /// <returns>Return a VirtualInput instance</returns>
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

        /// <summary>
        /// Remove a context
        /// </summary>
        /// <param name="name">Name of the context</param>
        /// <returns>Return true if the context is removed otherwise false</returns>
        public bool RemoveContext(string name)
        {
            return this.contextMap.Remove(name);
        }

        /// <summary>
        /// Get a context
        /// </summary>
        /// <param name="name">Name of the player</param>
        /// <returns>Return a VirtualInput instance</returns>
        public VirtualInput GetContext(string name)
        {
            VirtualInput input;
            if (!this.contextMap.TryGetValue(name, out input))
            {
                throw new Exception(string.Format("Failed to find a context named {0}", name));
            }

            return input;
        }

        /// <summary>
        /// Set the current context
        /// </summary>
        /// <param name="name">Name of the context</param>
        public void SetCurrentContext(string name)
        {
            VirtualInput input;
            if (!this.contextMap.TryGetValue(name, out input))
            {
                throw new Exception(string.Format("Failed to find a context named {0}", name));
            }

            this.currentContext = input;
        }

        /// <summary>
        /// Get a button from the current context
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <returns>Return a Button instance</returns>
        public Button GetButton(string name)
        {
            if (this.currentContext == null)
            {
                throw new Exception("No current context set");
            }

            return this.currentContext.GetButton(name);
        }

        /// <summary>
        /// Get an axis from the current context
        /// </summary>
        /// <param name="name">Name of the axis</param>
        /// <returns>Return an axis instance</returns>
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

        /// <summary>
        /// Get the index of the player
        /// </summary>
        public short PlayerIndex { get; internal set; }

        /// <summary>
        /// Get the current context
        /// </summary>
        public VirtualInput CurrentContext
        {
            get { return this.currentContext; }
        }

        #endregion
    }
}
