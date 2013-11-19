using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Defines how the screen resolution is resolved when the game switch in full screen mode
    /// </summary>
    public enum ResizeRule
    {
        /// <summary>
        /// Doesn't change the resolution
        /// </summary>
        None,
        /// <summary>
        /// Use the lowest resolution
        /// </summary>
        Lowest,
        /// <summary>
        /// Use the highest resolution
        /// </summary>
        Highest,
        /// <summary>
        /// Use the nearest resolution compared to the actual 
        /// </summary>
        Nearest
    }

    /// <summary>
    /// Represents the window of the game and manages back buffer parameters
    /// </summary>
    public sealed class Window : RenderTarget
    {
        #region Fields

        private bool _displayModeChanged;
        private bool _fullScreenChanged;
        private bool _deviceDirty;
        private readonly Dictionary<int, ReadOnlyCollection<DisplayMode>> _supportedDisplayMode = new Dictionary<int, ReadOnlyCollection<DisplayMode>>();

        #endregion

        #region Event

        /// <summary>
        /// Occurs when display mode is changed
        /// </summary>
        public event EventHandler<WindowDisplayModeChangedEventArgs> DisplayModeChanged;

        /// <summary>
        /// Occurs when full screen mode is switched
        /// </summary>
        public event EventHandler<WindowFullScreenSwitchedEventArgs> FullScreenSwitched;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Window class
        /// </summary>
        /// <param name="deviceManager">Graphic device manager used by the window</param>
        /// <param name="renderer">Renderer</param>
        internal Window(GraphicsDeviceManager deviceManager, Renderer renderer) : base(deviceManager, renderer)
        {
            PreferredResizeRule = ResizeRule.None;
            Initialize();
            CreateViewport(0);
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Find the nearest resolution from a list of resolution
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="availableModes">List of display mode</param>
        /// <returns>Returns the nearest display mode if found, otherwise false</returns>
        private static DisplayMode FindNearestResolution(int width, int height, ReadOnlyCollection<DisplayMode> availableModes)
        {
            if (availableModes.Count == 0) return null;

            DisplayMode nearest = availableModes[0];
            int screenDiff = Math.Abs(nearest.Width - width) + Math.Abs(nearest.Height - height);
            float ratio = (float)width / height;
            float ratioDiff = Math.Abs(ratio - ((float)nearest.Width / nearest.Height));

            for (int i = 1; i < availableModes.Count; i++)
            {
                DisplayMode current = availableModes[i];
                int currScreenDiff = Math.Abs(current.Width - width) + Math.Abs(current.Height - height);
                float currRatioDiff = Math.Abs(ratio - ((float)current.Width / current.Height));

                bool isNearest = false;
                if (currScreenDiff < screenDiff) isNearest = true;
                else if ((currScreenDiff == screenDiff) && (currRatioDiff < ratioDiff)) isNearest = true;

                if (!isNearest) continue;

                nearest = current;
                screenDiff = currScreenDiff;
                ratioDiff = currRatioDiff;
            }

            return nearest;
        }

        /// <summary>
        /// Compares two display mode by their resolutions
        /// </summary>
        /// <param name="first">First display mode</param>
        /// <param name="second">Second display mode</param>
        /// <returns>Returns -1 if the first resolution is lower, 1 if higher and 0 if both are equals</returns>
        private static int CompareDisplayModeByResolution(DisplayMode first, DisplayMode second)
        {
            int result;
            if (first.Width < second.Width) result = -1;
            else if (first.Width > second.Width) result = 1;
            else
            {
                if (first.Height < second.Height) result = -1;
                else if (first.Height > second.Height) result = 1;
                else result = 0;
            }

            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the window
        /// </summary>
        private void Initialize()
        {
            ExtractDisplayMode();
            ExtractSupportedSurfaceFormat();

            SetResolution(DeviceManager.PreferredBackBufferWidth, DeviceManager.PreferredBackBufferHeight);
            SetPixel(DeviceManager.PreferredBackBufferFormat);
            SetDepth(DeviceManager.PreferredDepthStencilFormat);
        }

        /// <summary>
        /// Invalidates the display mode
        /// </summary>
        private void InvalidateDisplayMode()
        {
            _displayModeChanged = true;
            _deviceDirty = true;
        }

        /// <summary>
        /// Invalidates the full screen mode
        /// </summary>
        private void InvalidateScreenMode()
        {
            _fullScreenChanged = true;
            _deviceDirty = true;
        }

        /// <summary>
        /// Called before rendering
        /// </summary>
        protected override void PreRender()
        {
            base.PreRender();

            if (!_deviceDirty) return;

            DeviceManager.ApplyChanges();
            _deviceDirty = false;

            if (_displayModeChanged)
            {
                WindowDisplayModeChangedEventArgs args = new WindowDisplayModeChangedEventArgs(DeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode, this);
                OnDisplayModeChanged(this, args);
                _displayModeChanged = false;
            }
            if (_fullScreenChanged)
            {
                WindowFullScreenSwitchedEventArgs args = new WindowFullScreenSwitchedEventArgs(DeviceManager.IsFullScreen, this);
                OnFullScreenSwitched(this, args);
                _fullScreenChanged = false;
            }
        }

        /// <summary>
        /// Called after rendering
        /// </summary>
        protected override void PostRender()
        {
            base.PostRender();

            Renderer.DrawFullQuad(Target);
        }

        /// <summary>
        /// Checks if the specified resolution is valid
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns>Returns true if the resolution is valid otherwise false</returns>
        public override bool IsValidResolution(int width, int height)
        {
            if (width <= 0) return false;
            if (height <= 0) return false;
            if (!DeviceManager.IsFullScreen) return true;

            ReadOnlyCollection<DisplayMode> availableModes = GetAvailableModes();
            if (availableModes == null) return false;
            for (int i = 0; i < availableModes.Count; i++)
            {
                DisplayMode mode = availableModes[i];
                if ((mode.Width == width) && (mode.Height == height)) return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the pixel format is valid
        /// </summary>
        /// <param name="pixel">Pixel format</param>
        /// <returns>Returns true if the pixel format is valid otherwise false</returns>
        public override bool IsValidPixel(SurfaceFormat pixel)
        {
            SurfaceFormat selectedPixel;
            DepthFormat selectedDepth;
            int selectedMultiSample;
            DeviceManager.GraphicsDevice.Adapter.QueryBackBufferFormat(DeviceManager.GraphicsProfile, pixel,
                DeviceManager.PreferredDepthStencilFormat, 0, out selectedPixel, out selectedDepth,
                out selectedMultiSample);

            return selectedPixel == pixel;
        }

        /// <summary>
        /// Checks if the depth buffer format is valid
        /// </summary>
        /// <param name="depth">Depth buffer format</param>
        /// <returns>Returns true if the depth buffer format is valid otherwise false</returns>
        public override bool IsValidDepth(DepthFormat depth)
        {
            SurfaceFormat selectedPixel;
            DepthFormat selectedDepth;
            int selectedMultiSample;
            DeviceManager.GraphicsDevice.Adapter.QueryBackBufferFormat(DeviceManager.GraphicsProfile,
                DeviceManager.PreferredBackBufferFormat, depth, 0, out selectedPixel, out selectedDepth,
                out selectedMultiSample);

            return selectedDepth == depth;
        }

        /// <summary>
        /// Sets the resolution
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public override void SetResolution(int width, int height)
        {
            base.SetResolution(width, height);
            DeviceManager.PreferredBackBufferWidth = width;
            DeviceManager.PreferredBackBufferHeight = height;
            InvalidateDisplayMode();
        }

        /// <summary>
        /// Sets the pixel format
        /// </summary>
        /// <param name="pixel">Pixel format</param>
        public override void SetPixel(SurfaceFormat pixel)
        {
            base.SetPixel(pixel);
            DeviceManager.PreferredBackBufferFormat = pixel;
            InvalidateScreenMode();
        }

        /// <summary>
        /// Sets the depth buffer format
        /// </summary>
        /// <param name="depth">Depth buffer format</param>
        public override void SetDepth(DepthFormat depth)
        {
            base.SetDepth(depth);
            DeviceManager.PreferredDepthStencilFormat = depth;
            _deviceDirty = true;
        }

        /// <summary>
        /// Sets the full screen mode
        /// </summary>
        /// <param name="isFullScreen">Value which indicates full screen mode will be activated</param>
        public void SetFullScreen(bool isFullScreen)
        {
            if (isFullScreen)
            {
                if (PreferredResizeRule != ResizeRule.None)
                {
                    DisplayMode mode = FindFullScreenResolution(PreferredResizeRule, Width, Height);
                    if (mode == null) throw new Exception("No valid resolution find for fullscreen mode");
                    SetResolution(mode.Width, mode.Height);
                }
            }
            DeviceManager.IsFullScreen = isFullScreen;
            InvalidateScreenMode();
        }

        /// <summary>
        /// Find the most appropriate resoluton depending on the specified resize rule
        /// </summary>
        /// <param name="rule">Resize rule</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <returns>Returns the display mode if found, otherwise null</returns>
        private DisplayMode FindFullScreenResolution(ResizeRule rule, int width, int height)
        {
            ReadOnlyCollection<DisplayMode> availableModes = GetAvailableModes();
            if ((availableModes == null) || (availableModes.Count == 0)) return null;

            DisplayMode mode = null;
            switch (rule)
            {
                case ResizeRule.Lowest: mode = availableModes[0];
                    break;
                case ResizeRule.Highest: mode = availableModes[availableModes.Count - 1];
                    break;
                case ResizeRule.Nearest: mode = FindNearestResolution(width, height, availableModes);
                    break;
            }

            return mode;
        }

        /// <summary>
        /// Gets availables display mode for the current back buffer pixel format
        /// </summary>
        /// <returns>Returns a list of display mode if there is, otherwise null</returns>
        private ReadOnlyCollection<DisplayMode> GetAvailableModes()
        {
            int currentFormat = (int)DeviceManager.PreferredBackBufferFormat;
            ReadOnlyCollection<DisplayMode> availableModes;

            return !_supportedDisplayMode.TryGetValue(currentFormat, out availableModes) ? null : availableModes;
        }

        /// <summary>
        /// Extracts all availables display mode supported by the graphics device for the back buffer
        /// </summary>
        private void ExtractDisplayMode()
        {
            DisplayModeCollection modes = DeviceManager.GraphicsDevice.Adapter.SupportedDisplayModes;
            Dictionary<int, List<DisplayMode>> supported = new Dictionary<int, List<DisplayMode>>();
            foreach (DisplayMode dm in modes)
            {
                List<DisplayMode> formatList;
                SurfaceFormat currentFormat = dm.Format;
                if (!supported.TryGetValue((int)currentFormat, out formatList))
                {
                    formatList = new List<DisplayMode>();
                    supported.Add((int)currentFormat, formatList); 
                }
                formatList.Add(dm);
            }

            foreach (KeyValuePair<int, List<DisplayMode>> kvp in supported)
            {
                List<DisplayMode> modesList = kvp.Value;
                modesList.Sort(CompareDisplayModeByResolution);
                _supportedDisplayMode.Add(kvp.Key, new ReadOnlyCollection<DisplayMode>(modesList));
            }
        }

        /// <summary>
        /// Extracts all pixel format supported by the graphics device for the back buffer
        /// </summary>
        private void ExtractSupportedSurfaceFormat()
        {
            List<SurfaceFormat> supported = new List<SurfaceFormat>();
            foreach (SurfaceFormat sf in _supportedDisplayMode.Keys)
            {
                supported.Add(sf);
            }
            SupportedSurfaceFormat = new ReadOnlyCollection<SurfaceFormat>(supported);
        }

        /// <summary>
        /// Called when display mode has changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Arguments</param>
        private void OnDisplayModeChanged(object sender, WindowDisplayModeChangedEventArgs args)
        {
            if (DisplayModeChanged != null)
            {
                DisplayModeChanged(sender, args);
            }
        }

        /// <summary>
        /// Called when full screen mode has changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Arguments</param>
        private void OnFullScreenSwitched(object sender, WindowFullScreenSwitchedEventArgs args)
        {
            if (FullScreenSwitched != null)
            {
                FullScreenSwitched(sender, args);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that indicates whether mipmap is activated
        /// </summary>
        public override bool MipMap
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value that indicates whether full scree mode is activated
        /// </summary>
        public bool IsFullScreen
        {
            get { return DeviceManager.IsFullScreen; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether v-sync is activated
        /// </summary>
        public bool VSync
        {
            get { return DeviceManager.SynchronizeWithVerticalRetrace; }
            set
            {
                DeviceManager.SynchronizeWithVerticalRetrace = value;
                _deviceDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the resize rule
        /// </summary>
        public ResizeRule PreferredResizeRule { get; set; }

        /// <summary>
        /// Gets a list of supported pixel format for the back buffer
        /// </summary>
        public ReadOnlyCollection<SurfaceFormat> SupportedSurfaceFormat { get; private set; }

        /// <summary>
        /// Gets a list of available display mode for the current back buffer pixel format
        /// </summary>
        public ReadOnlyCollection<DisplayMode> AvailableDisplayMode
        {
            get { return GetAvailableModes(); }
        }

        #endregion
    }
}