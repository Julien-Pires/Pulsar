using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics
{
    public enum ResizeRule
    {
        None,
        Lowest,
        Highest,
        Nearest
    }

    public sealed class Window : RenderTarget
    {
        #region Fields

        private bool _displayModeChanged;
        private bool _fullScreenChanged;
        private bool _deviceDirty;
        private readonly Dictionary<int, ReadOnlyCollection<DisplayMode>> _supportedDisplayMode = new Dictionary<int, ReadOnlyCollection<DisplayMode>>();

        #endregion

        #region Event

        private EventHandler<WindowDisplayModeChangedEventArgs> _onDisplayModeChanged;
        private EventHandler<WindowFullScreenSwitchedEventArgs> _onFullScreenSwitched;

        public event EventHandler<WindowDisplayModeChangedEventArgs> DisplayModeChanged
        {
            add { _onDisplayModeChanged += value; }
            remove { _onDisplayModeChanged -= value; }
        }

        public event EventHandler<WindowFullScreenSwitchedEventArgs> FullScreenSwitched
        {
            add { _onFullScreenSwitched += value; }
            remove { _onFullScreenSwitched -= value; }
        }

        #endregion

        #region Constructor

        internal Window(GraphicsDeviceManager deviceManager, Renderer renderer) : base(deviceManager, renderer)
        {
            PreferredResizeRule = ResizeRule.None;
            ExtractDeviceData();
            Initialize();
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            SetResolution(DeviceManager.PreferredBackBufferWidth, DeviceManager.PreferredBackBufferHeight);
            SetPixel(DeviceManager.PreferredBackBufferFormat);
            SetDepth(DeviceManager.PreferredDepthStencilFormat);
        }

        private void ExtractDeviceData()
        {
            ExtractDisplayMode();
            ExtractSupportedSurfaceFormat();
        }

        private void InvalidateDisplayMode()
        {
            _displayModeChanged = true;
            _deviceDirty = true;
        }

        private void InvalidateScreenMode()
        {
            _fullScreenChanged = true;
            _deviceDirty = true;
        }

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

        protected override void PostRender()
        {
            base.PostRender();

            Renderer.DrawFullQuad(Target);
        }

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
                if ((mode.Width == width) && (mode.Height == height))
                {
                    return true;
                }
            }

            return false;
        }

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

        public override void SetResolution(int width, int height)
        {
            base.SetResolution(width, height);
            DeviceManager.PreferredBackBufferWidth = width;
            DeviceManager.PreferredBackBufferHeight = height;
            InvalidateDisplayMode();
        }

        public override void SetPixel(SurfaceFormat pixel)
        {
            base.SetPixel(pixel);
            DeviceManager.PreferredBackBufferFormat = pixel;
            InvalidateScreenMode();
        }

        public override void SetDepth(DepthFormat depth)
        {
            base.SetDepth(depth);
            DeviceManager.PreferredDepthStencilFormat = depth;
            _deviceDirty = true;
        }

        public void SetFullScreen(bool isFullScreen)
        {
            if (isFullScreen)
            {
                if (PreferredResizeRule != ResizeRule.None)
                {
                    DisplayMode mode = FindFullScreenResolution(PreferredResizeRule, Width, Height);
                    if (mode == null)
                    {
                        throw new Exception("No valid resolution find for fullscreen mode");
                    }
                    SetResolution(mode.Width, mode.Height);
                }
            }
            DeviceManager.IsFullScreen = isFullScreen;
            InvalidateScreenMode();
        }

        private DisplayMode FindFullScreenResolution(ResizeRule rule, int width, int height)
        {
            ReadOnlyCollection<DisplayMode> availableModes = GetAvailableModes();
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
                case ResizeRule.Nearest: mode = FindNearestResolution(width, height, availableModes);
                    break;
            }

            return mode;
        }

        private static DisplayMode FindNearestResolution(int width, int height, ReadOnlyCollection<DisplayMode> availableModes)
        {
            if (availableModes.Count == 0)
            {
                return null;
            }

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

                if (currScreenDiff < screenDiff)
                {
                    isNearest = true;
                }
                else if ((currScreenDiff == screenDiff) && (currRatioDiff < ratioDiff))
                {
                    isNearest = true;
                }

                if (!isNearest) continue;
                nearest = current;
                screenDiff = currScreenDiff;
                ratioDiff = currRatioDiff;
            }

            return nearest;
        }

        private ReadOnlyCollection<DisplayMode> GetAvailableModes()
        {
            int currentFormat = (int)DeviceManager.PreferredBackBufferFormat;
            ReadOnlyCollection<DisplayMode> availableModes;

            return !_supportedDisplayMode.TryGetValue(currentFormat, out availableModes) ? null : availableModes;
        }

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

        private void ExtractSupportedSurfaceFormat()
        {
            List<SurfaceFormat> supported = new List<SurfaceFormat>();
            foreach (SurfaceFormat sf in _supportedDisplayMode.Keys)
            {
                supported.Add(sf);
            }
            SupportedSurfaceFormat = new ReadOnlyCollection<SurfaceFormat>(supported);
        }

        private static int CompareDisplayModeByResolution(DisplayMode first, DisplayMode second)
        {
            int result;
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
            if (_onDisplayModeChanged != null)
            {
                _onDisplayModeChanged(sender, args);
            }
        }

        private void OnFullScreenSwitched(object sender, WindowFullScreenSwitchedEventArgs args)
        {
            if (_onFullScreenSwitched != null)
            {
                _onFullScreenSwitched(sender, args);
            }
        }

        #endregion

        #region Properties

        public override bool MipMap
        {
            get { return false; }
            set { }
        }

        public bool IsFullScreen
        {
            get { return DeviceManager.IsFullScreen; }
        }

        public bool VSync
        {
            get { return DeviceManager.SynchronizeWithVerticalRetrace; }
            set
            {
                DeviceManager.SynchronizeWithVerticalRetrace = value;
                _deviceDirty = true;
            }
        }

        public ResizeRule PreferredResizeRule { get; set; }

        public ReadOnlyCollection<SurfaceFormat> SupportedSurfaceFormat { get; private set; }

        public ReadOnlyCollection<DisplayMode> AvailableDisplayMode
        {
            get { return GetAvailableModes(); }
        }

        #endregion
    }
}