using System;
using System.Collections.Generic;

namespace Pulsar.Input
{
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
        /// Creates a new player
        /// </summary>
        /// <param name="player">Index of the player</param>
        /// <returns>Returns a Player instance</returns>
        public Player CreatePlayer(short player)
        {
            if (_players.ContainsKey(player))
                throw new Exception(string.Format("Failed to create the player {0}, he already exists", player));

            Player input = new Player(player);
            _players.Add(player, input);

            return input;
        }

        /// <summary>
        /// Removes a player
        /// </summary>
        /// <param name="player">Index of the player</param>
        /// <returns>Returns true if the player is removed otherwise false</returns>
        public bool RemovePlayer(short player)
        {
            return _players.Remove(player);
        }

        /// <summary>
        /// Removes all players
        /// </summary>
        public void RemoveAllPlayers()
        {
            _players.Clear();
        }

        /// <summary>
        /// Gets a player
        /// </summary>
        /// <param name="player">Index of the player</param>
        /// <returns>Returns a Player instance</returns>
        public Player GetPlayer(short player)
        {
            return _players[player];
        }

        /// <summary>
        /// Updates the states of devices and players
        /// </summary>
        public void Update()
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
