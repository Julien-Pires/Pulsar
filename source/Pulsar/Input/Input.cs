using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pulsar.Input
{
    public enum InputDevice { Mouse, Keyboard, GamePad }

    public class Input
    {
        #region Fields

        private Dictionary<short, PlayerInput> players = new Dictionary<short, PlayerInput>();

        #endregion

        #region Methods

        public void CreatePlayer(short player)
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
        }

        public bool RemovePlayer(short player)
        {
            return this.players.Remove(player);
        }

        public PlayerInput GetPlayer(short player)
        {
            return this.players[player];
        }

        internal void Update()
        {
            Mouse.Update();
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
