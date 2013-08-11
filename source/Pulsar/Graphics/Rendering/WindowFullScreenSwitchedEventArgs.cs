using System;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Provides data for the FullScreenSwitched event
    /// </summary>
    public sealed class WindowFullScreenSwitchedEventArgs : EventArgs
    {
        #region Fields

        private readonly bool _isFullScreen;
        private readonly Window _window;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of WindowFullScreenSwitchedEventArgs class
        /// </summary>
        /// <param name="fullScreen">Fullscreen mode state</param>
        /// <param name="win">Window</param>
        public WindowFullScreenSwitchedEventArgs(bool fullScreen, Window win)
        {
            _isFullScreen = fullScreen;
            _window = win;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that indicates whether the full screen mode is activated
        /// </summary>
        public bool IsFullScreen
        {
            get { return _isFullScreen; }
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
