using System;
using System.Collections.Generic;

namespace Pulsar.Input
{
    /// <summary>
    /// Enumerate all input devices
    /// </summary>
    [Flags]
    public enum InputDevice : byte
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

        private readonly Dictionary<short, Player> _players = new Dictionary<short, Player>();

        #endregion

        #region Methods

        /// <summary>
        /// Create a new player
        /// </summary>
        /// <param name="player">Index of the player</param>
        /// <returns>Return a Player instance</returns>
        public Player CreatePlayer(short player)
        {
            if (_players.ContainsKey(player)) throw new Exception(string.Format("Failed to create the player {0}, he already exists", player));

            Player input = new Player(player);
            _players.Add(player, input);

            return input;
        }

        /// <summary>
        /// Remove a player
        /// </summary>
        /// <param name="player">Index of the player</param>
        /// <returns>Return true if the player is removed otherwise false</returns>
        public bool RemovePlayer(short player)
        {
            return _players.Remove(player);
        }

        /// <summary>
        /// Remove all players
        /// </summary>
        public void RemoveAllPlayers()
        {
            _players.Clear();
        }

        /// <summary>
        /// Get a player
        /// </summary>
        /// <param name="player">Index of the player</param>
        /// <returns>Return a Player instance</returns>
        public Player GetPlayer(short player)
        {
            return _players[player];
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

            foreach (Player plInput in _players.Values)
            {
                plInput.Update();
            }
        }

        #endregion
    }
}
