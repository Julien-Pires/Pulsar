using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Provides data for the DisplayModeChanged event
    /// </summary>
    public sealed class WindowDisplayModeChangedEventArgs : EventArgs
    {
        #region Fields

        private readonly DisplayMode _mode;
        private readonly Window _window;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of WindowDisplayModeChangedEventArgs class
        /// </summary>
        /// <param name="mode">New display mode</param>
        /// <param name="win">Window</param>
        public WindowDisplayModeChangedEventArgs(DisplayMode mode, Window win)
        {
            _mode = mode;
            _window = win;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the new display mode
        /// </summary>
        public DisplayMode Mode
        {
            get { return _mode; }
        }

        /// <summary>
        /// Gets the window
        /// </summary>
        public Window Window
        {
            get { return _window; }
        }

        #endregion
    }
}
