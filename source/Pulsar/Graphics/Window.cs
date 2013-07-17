using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Extension;

namespace Pulsar.Graphics
{
    public enum ResizeRule
    {
        None,
        Lowest,
        Highest,
        Nearest
    }

    public sealed class Window
    {
        #region Fields

        private GraphicsDeviceManager deviceManager;
        private GraphicsDevice device;
        private ResizeRule resizeRule = ResizeRule.None;
        private ReadOnlyCollection<SurfaceFormat> supportedFormat;
        private Dictionary<int, ReadOnlyCollection<DisplayMode>> supportedDisplayMode = new Dictionary<int, ReadOnlyCollection<DisplayMode>>();
        private bool isDirty = true;

        #endregion

        #region Event

        private event EventHandler<WindowDisplayModeChangedEventArgs> displayModeChanged;
        private event EventHandler<WindowFullScreenSwitchedEventArgs> fullScreenSwitched;

        public event EventHandler<WindowDisplayModeChangedEventArgs> DisplayModeChanged
        {
            add { this.displayModeChanged += value; }
            remove { this.displayModeChanged -= value; }
        }

        public event EventHandler<WindowFullScreenSwitchedEventArgs> FullScreenSwitched
        {
            add { this.fullScreenSwitched += value; }
            remove { this.fullScreenSwitched -= value; }
        }

        #endregion

        #region Constructor

        internal Window(GraphicsDeviceManager deviceManager)
        {
            if (deviceManager == null)
            {
                throw new ArgumentNullException("GraphicsDeviceManager cannot be null");
            }
            if (deviceManager.GraphicsDevice == null)
            {
                throw new ArgumentNullException("GraphicsDevice cannot be null");
            }
            this.deviceManager = deviceManager;
            this.device = deviceManager.GraphicsDevice;
            this.Initialize();
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            this.ExtractDisplayMode();
            this.ExtractSupportedSurfaceFormat();
        }

        public void Render()
        {
            if (this.isDirty)
            {
                this.ApplyChanges();
            }
        }

        private void ApplyChanges()
        {
            this.deviceManager.ApplyChanges();
            this.isDirty = false;
        }

        public void SetResolution(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentException("Width cannot be inferior or equal to zero");
            }
            if (height <= 0)
            {
                throw new ArgumentException("Height cannot be inferior or equal to zero");
            }

            this.deviceManager.PreferredBackBufferWidth = width;
            this.deviceManager.PreferredBackBufferHeight = height;
            this.isDirty = true;

            WindowDisplayModeChangedEventArgs args = new WindowDisplayModeChangedEventArgs(this.device.Adapter.CurrentDisplayMode, this);
            this.OnDisplayModeChanged(this, args);
        }

        public void SetFullScreen(bool isFullScreen)
        {
            if (isFullScreen)
            {
                int newWidth = this.deviceManager.PreferredBackBufferWidth;
                int newHeight = this.deviceManager.PreferredBackBufferHeight;
                DisplayMode mode = this.FindFullScreenResolution(this.resizeRule, newWidth, newHeight);
                if (this.resizeRule != ResizeRule.None)
                {
                    if (mode == null)
                    {
                        throw new Exception("No valid resolution find for fullscreen mode");
                    }
                    newWidth = mode.Width;
                    newHeight = mode.Height;
                }

                this.deviceManager.PreferredBackBufferWidth = newWidth;
                this.deviceManager.PreferredBackBufferHeight = newHeight;
            }

            this.deviceManager.IsFullScreen = isFullScreen;
            this.isDirty = true;

            WindowFullScreenSwitchedEventArgs args = new WindowFullScreenSwitchedEventArgs(isFullScreen, this);
            this.OnFullScreenSwitched(this, args);
        }

        public void SetSurfaceFormat(SurfaceFormat format)
        {
            SurfaceFormat supportedFormat;
            DepthFormat supportedDepth;
            int supportedSampling;
            bool supported = this.device.Adapter.QueryBackBufferFormat(this.deviceManager.GraphicsProfile, format,
                this.deviceManager.PreferredDepthStencilFormat, 0, out supportedFormat, out supportedDepth, out supportedSampling);
            if (supportedFormat != format)
            {
                throw new NotSupportedException(string.Format("SurfaceFormat {0} not supported for back buffer", format));
            }

            this.deviceManager.PreferredBackBufferFormat = format;
            this.isDirty = true;
        }

        public void SetDepthFormat(DepthFormat depth)
        {
            SurfaceFormat supportedFormat;
            DepthFormat supportedDepth;
            int supportedSampling;
            bool supported = this.device.Adapter.QueryBackBufferFormat(this.deviceManager.GraphicsProfile, 
                this.deviceManager.PreferredBackBufferFormat, depth, 0, out supportedFormat, out supportedDepth, out supportedSampling);
            if (supportedDepth != depth)
            {
                throw new NotSupportedException(string.Format("DepthFormat {0} not supported for back buffer", depth));
            }

            this.deviceManager.PreferredDepthStencilFormat = depth;
            this.isDirty = true;
        }

        private DisplayMode FindFullScreenResolution(ResizeRule rule, int width, int height)
        {
            ReadOnlyCollection<DisplayMode> availableModes = this.GetAvailableModes();
            if ((availableModes == null) || (availableModes.Count == 0))
            {
                return null;
            }

            DisplayMode mode = null;
            switch (rule)
            {
                case ResizeRule.Lowest: mode = availableModes[0];
                    break;
                case ResizeRule.Highest: mode = availableModes[availableModes.Count - 1];
                    break;
                case ResizeRule.Nearest: mode = this.FindNearestResolution(width, height, availableModes);
                    break;
            }

            return mode;
        }

        private DisplayMode FindNearestResolution(int width, int height, ReadOnlyCollection<DisplayMode> availableModes)
        {
            if (availableModes.Count == 0)
            {
                return null;
            }

            DisplayMode nearest = availableModes[0];
            int screenDiff = Math.Abs(nearest.Width - width) + Math.Abs(nearest.Height - height);
            float ratio = (float)width / (float)height;
            float ratioDiff = Math.Abs(ratio - ((float)nearest.Width / (float)nearest.Height));

            for (int i = 1; i < availableModes.Count; i++)
            {
                DisplayMode current = availableModes[i];
                int currScreenDiff = Math.Abs(current.Width - width) + Math.Abs(current.Height - height);
                float currRatioDiff = Math.Abs(ratio - ((float)current.Width / (float)current.Height));
                bool isNearest = false;

                if (currScreenDiff < screenDiff)
                {
                    isNearest = true;
                }
                else if ((currScreenDiff == screenDiff) && (currRatioDiff < ratioDiff))
                {
                    isNearest = true;
                }

                if (isNearest)
                {
                    nearest = current;
                    screenDiff = currScreenDiff;
                    ratioDiff = currRatioDiff;
                }
            }

            return nearest;
        }

        public bool IsValidResolution(int width, int height)
        {
            if (!this.deviceManager.IsFullScreen)
            {
                return true;
            }

            ReadOnlyCollection<DisplayMode> availableModes = this.GetAvailableModes();
            if (availableModes == null)
            {
                return false;
            }

            for (int i = 0; i < availableModes.Count; i++)
            {
                DisplayMode mode = availableModes[i];
                if ((mode.Width == width) && (mode.Height == height))
                {
                    return true;
                }
            }

            return false;
        }

        private ReadOnlyCollection<DisplayMode> GetAvailableModes()
        {
            int currentFormat = (int)this.deviceManager.PreferredBackBufferFormat;
            ReadOnlyCollection<DisplayMode> availableModes;
            if (!this.supportedDisplayMode.TryGetValue(currentFormat, out availableModes))
            {
                return null;
            }

            return availableModes;
        }

        private void ExtractDisplayMode()
        {
            DisplayModeCollection modes = this.device.Adapter.SupportedDisplayModes;
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
                modesList.Sort(this.CompareDisplayModeByResolution);
                this.supportedDisplayMode.Add(kvp.Key, new ReadOnlyCollection<DisplayMode>(modesList));
            }
        }

        private void ExtractSupportedSurfaceFormat()
        {
            List<SurfaceFormat> supported = new List<SurfaceFormat>();
            foreach (SurfaceFormat sf in this.supportedDisplayMode.Keys)
            {
                supported.Add(sf);
            }
            this.supportedFormat = new ReadOnlyCollection<SurfaceFormat>(supported);
        }

        private int CompareDisplayModeByResolution(DisplayMode first, DisplayMode second)
        {
            int result = 0;
            if (first.Width < second.Width)
            {
                result = -1;
            }
            else if (first.Width > second.Width)
            {
                result = 1;
            }
            else
            {
                if (first.Height < second.Height)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
            }

            return result;
        }

        private void OnDisplayModeChanged(object sender, WindowDisplayModeChangedEventArgs args)
        {
            if (this.displayModeChanged != null)
            {
                this.displayModeChanged(sender, args);
            }
        }

        private void OnFullScreenSwitched(object sender, WindowFullScreenSwitchedEventArgs args)
        {
            if (this.fullScreenSwitched != null)
            {
                this.fullScreenSwitched(sender, args);
            }
        }

        #endregion

        #region Properties

        public int Height
        {
            get { return this.deviceManager.PreferredBackBufferHeight; }
        }

        public int Width
        {
            get { return this.deviceManager.PreferredBackBufferWidth; }
        }

        public bool IsFullScreen
        {
            get { return this.deviceManager.IsFullScreen; }
        }

        public SurfaceFormat Surface
        {
            get { return this.deviceManager.PreferredBackBufferFormat; }
        }

        public DepthFormat Depth
        {
            get { return this.deviceManager.PreferredDepthStencilFormat; }
        }

        public bool VSync
        {
            get { return this.deviceManager.SynchronizeWithVerticalRetrace; }
            set
            {
                this.deviceManager.SynchronizeWithVerticalRetrace = value;
                this.isDirty = true;
            }
        }

        public ResizeRule PreferredResizeRule
        {
            get { return this.resizeRule; }
            set { this.resizeRule = value; }
        }

        public ReadOnlyCollection<SurfaceFormat> SupportedSurfaceFormat
        {
            get { return this.supportedFormat; }
        }

        public ReadOnlyCollection<DisplayMode> AvailableDisplayMode
        {
            get { return this.GetAvailableModes(); }
        }

        #endregion
    }
}