using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pulsar.Input
{
    public enum InputDevice 
    { 
        None,
        Mouse,
        Keyboard,
        GamePad
    }

    public sealed class InputManager
    {
        #region Fields

        private Dictionary<short, PlayerInput> players = new Dictionary<short, PlayerInput>();

        #endregion

        #region Methods

        public PlayerInput CreatePlayer(short player)
        {
            if (this.players.ContainsKey(player))
            {
                throw new Exception(string.Format("Failed to create the player {0}, he already exists", player));
            }

            PlayerInput input = new PlayerInput()
            {
                PlayerIndex = player
            };
            this.players.Add(player, input);

            return input;
        }

        public bool RemovePlayer(short player)
        {
            return this.players.Remove(player);
        }

        public void RemoveAllPlayers()
        {
            this.players.Clear();
        }

        public PlayerInput GetPlayer(short player)
        {
            return this.players[player];
        }

        internal void Update()
        {
#if WINDOWS
            Mouse.Update();
#endif
            Keyboard.Update();
            GamePad.Update();

            Dictionary<short, PlayerInput>.ValueCollection values = this.players.Values;
            foreach (PlayerInput plInput in values)
            {
                VirtualInput current = plInput.CurrentContext;
                if (current != null)
                {
                    current.Update();
                }
            }
        }

        #endregion
    }
}
