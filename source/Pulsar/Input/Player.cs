using System;
using System.Collections.Generic;

namespace Pulsar.Input
{
    /// <summary>
    /// Represents one player and contains all his input context
    /// </summary>
    public sealed class Player
    {
        #region Fields

        private readonly PlayerIndex _playerIndex = new PlayerIndex();
        private readonly Dictionary<string, VirtualInput> _contextMap = new Dictionary<string, VirtualInput>();
        private VirtualInput _currentContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Player class
        /// </summary>
        internal Player(short playerIndex)
        {
            _playerIndex.Index = playerIndex;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update player context
        /// </summary>
        internal void Update()
        {
            if (_currentContext != null)
            {
                _currentContext.Update();
                DispatchButtonEvent();
            }
        }

        /// <summary>
        /// Dispatch button event from device to the current context
        /// </summary>
        private void DispatchButtonEvent()
        {
            InputDevice device = _currentContext.AssociatedDevice;
#if WINDOWS
            if ((device & InputDevice.Mouse) == InputDevice.Mouse)
            {
                for (short i = 0; i < Mouse.InternalButtonPressed.Count; i++)
                {
                    _currentContext.InternalButtonPressed.Add(Mouse.InternalButtonPressed[i]);
                }
            }
#endif
            if ((device & InputDevice.Keyboard) == InputDevice.Keyboard)
            {
                for (short i = 0; i < Keyboard.InternalButtonPressed.Count; i++)
                {
                    _currentContext.InternalButtonPressed.Add(Keyboard.InternalButtonPressed[i]);
                }
            }
            if ((device & InputDevice.GamePad) == InputDevice.GamePad)
            {
                for(short i = 0; i < GamePad.InternalButtonPressed.Count; i++)
                {
                    ButtonEvent btnEvent = GamePad.InternalButtonPressed[i];
                    if (btnEvent.Index == _playerIndex.GamePadIndex)
                    {
                        _currentContext.InternalButtonPressed.Add(btnEvent);
                    }
                }
            }
            else if ((device & InputDevice.AllGamePad) == InputDevice.AllGamePad)
            {
                for (short i = 0; i < GamePad.InternalButtonPressed.Count; i++)
                {
                    _currentContext.InternalButtonPressed.Add(GamePad.InternalButtonPressed[i]);
                }
            }
        }

        /// <summary>
        /// Remove all context and reset the player
        /// </summary>
        public void Reset()
        {
            _contextMap.Clear();
            _currentContext = null;
        }

        /// <summary>
        /// Create a new context
        /// </summary>
        /// <param name="name">Name of the context</param>
        /// <returns>Return a VirtualInput instance</returns>
        public VirtualInput CreateContext(string name)
        {
            if (_contextMap.ContainsKey(name)) throw new Exception(string.Format("A context named {0} already exists", name));
            VirtualInput vInput = new VirtualInput(_playerIndex);
            _contextMap.Add(name, vInput);

            return vInput;
        }

        /// <summary>
        /// Remove a context
        /// </summary>
        /// <param name="name">Name of the context</param>
        /// <returns>Return true if the context is removed otherwise false</returns>
        public bool RemoveContext(string name)
        {
            return _contextMap.Remove(name);
        }

        /// <summary>
        /// Get a context
        /// </summary>
        /// <param name="name">Name of the player</param>
        /// <returns>Return a VirtualInput instance</returns>
        public VirtualInput GetContext(string name)
        {
            VirtualInput input;
            if (!_contextMap.TryGetValue(name, out input)) throw new Exception(string.Format("Failed to find a context named {0}", name));

            return input;
        }

        /// <summary>
        /// Set the current context
        /// </summary>
        /// <param name="name">Name of the context</param>
        public void SetCurrentContext(string name)
        {
            VirtualInput input;
            if (!_contextMap.TryGetValue(name, out input)) throw new Exception(string.Format("Failed to find a context named {0}", name));

            _currentContext = input;
        }

        /// <summary>
        /// Get a button from the current context
        /// </summary>
        /// <param name="name">Name of the button</param>
        /// <returns>Return a Button instance</returns>
        public Button GetButton(string name)
        {
            if (_currentContext == null) throw new Exception("No current context set");

            return _currentContext.GetButton(name);
        }

        /// <summary>
        /// Get an axis from the current context
        /// </summary>
        /// <param name="name">Name of the axis</param>
        /// <returns>Return an axis instance</returns>
        public Axis GetAxis(string name)
        {
            if (_currentContext == null) throw new Exception("No current context set");

            return _currentContext.GetAxis(name);
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Get the index of the player
        /// </summary>
        public short PlayerIndex
        {
            get { return _playerIndex.Index; }
        }

        public short GamePadIndex
        {
            get { return _playerIndex.GamePadIndex; }
            set
            {
                if(value > (GamePad.GamePadCount - 1) || (value < 0)) throw new ArgumentOutOfRangeException("value");
                _playerIndex.GamePadIndex = value;
            }
        }

        /// <summary>
        /// Get the current context
        /// </summary>
        public VirtualInput CurrentContext
        {
            get { return _currentContext; }
        }

        #endregion
    }
}
