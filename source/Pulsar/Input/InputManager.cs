using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pulsar.Input
{
    /// <summary>
    /// Enumerate all input devices
    /// </summary>
    [Flags]
    public enum InputDevice 
    { 
        None = 0,
        Mouse = 1,
        Keyboard = 2,
        GamePad = 4,
        AllGamePad = 8
    }

    /// <summary>
    /// Manages all input device and virtual input
    /// </summary>
    public sealed class InputManager
    {
        #region Fields

        private Dictionary<short, PlayerInput> players = new Dictionary<short, PlayerInput>();
        private AbstractButton[] allButtons;

        #endregion

        #region Methods

        /// <summary>
        /// Create a new player
        /// </summary>
        /// <param name="player">Index of the player</param>
        /// <returns>Return a PlayerInput instance</returns>
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

        /// <summary>
        /// Remove a player
        /// </summary>
        /// <param name="player">Index of the player</param>
        /// <returns>Return true if the player is removed otherwise false</returns>
        public bool RemovePlayer(short player)
        {
            return this.players.Remove(player);
        }

        /// <summary>
        /// Remove all players
        /// </summary>
        public void RemoveAllPlayers()
        {
            this.players.Clear();
        }

        /// <summary>
        /// Get a player
        /// </summary>
        /// <param name="player">Index of the player</param>
        /// <returns>Return a PlayerInput instance</returns>
        public PlayerInput GetPlayer(short player)
        {
            return this.players[player];
        }

        /// <summary>
        /// Update the states of devices and players
        /// </summary>
        internal void Update()
        {
#if WINDOWS
            Mouse.Update();
#endif
            Keyboard.Update();
            GamePad.UpdatePads();

            foreach (PlayerInput plInput in this.players.Values)
            {
                plInput.Update();
            }
        }

        #endregion
    }
}
