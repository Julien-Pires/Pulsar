using System;

using System.Collections.Generic;

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

        private Dictionary<short, Player> players = new Dictionary<short, Player>();
        private AbstractButton[] allButtons;

        #endregion

        #region Methods

        /// <summary>
        /// Create a new player
        /// </summary>
        /// <param name="player">Index of the player</param>
        /// <returns>Return a Player instance</returns>
        public Player CreatePlayer(short player)
        {
            if (this.players.ContainsKey(player))
            {
                throw new Exception(string.Format("Failed to create the player {0}, he already exists", player));
            }

            Player input = new Player(player);
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
        /// <returns>Return a Player instance</returns>
        public Player GetPlayer(short player)
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

            foreach (Player plInput in this.players.Values)
            {
                plInput.Update();
            }
        }

        #endregion
    }
}
