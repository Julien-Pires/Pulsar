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

        private readonly InputManager _input;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of InputService class
        /// </summary>
        /// <param name="game">Game instance</param>
        public InputService(XnaGame game)
        {
            if (game == null) throw new ArgumentNullException("game", "Cannot be null");
            if (game.Services.GetService(typeof(IInputService)) != null) throw new ArgumentException("InputManager already present");
            game.Services.AddService(typeof(IInputService), this);
            _input = new InputManager();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get a InputManager instance
        /// </summary>
        public InputManager Input
        {
            get { return _input; }
        }

        #endregion
    }
}
