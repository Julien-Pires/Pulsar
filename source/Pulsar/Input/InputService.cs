using System;

using XnaGame = Microsoft.Xna.Framework.Game;

namespace Pulsar.Input
{
    /// <summary>
    /// Provides a input service
    /// </summary>
    public sealed class InputService : IInputService
    {
        #region Fields

        private InputManager input;
        private XnaGame game;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of InputService class
        /// </summary>
        /// <param name="game">Game instance</param>
        public InputService(XnaGame game)
        {
            if (game == null)
            {
                throw new ArgumentNullException("game", "game cannot be null");
            }
            this.game = game;
            if (this.game.Services.GetService(typeof(IInputService)) != null)
            {
                throw new ArgumentException("InputManager already present");
            }
            this.game.Services.AddService(typeof(IInputService), this);
            this.input = new InputManager();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get a InputManager instance
        /// </summary>
        public InputManager Input
        {
            get { return this.input; }
        }

        #endregion
    }
}
