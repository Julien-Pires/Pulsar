using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        #region Nested

        private sealed class ViewportBinding
        {
            #region Fields

            private readonly int _id;

            private ushort _zOrder;

            private readonly Viewport _viewport;

            #endregion

            #region Constructor

            public ViewportBinding(int id, Viewport vp)
            {
                _id = id;
                _viewport = vp;
                ZOrder = 0;
            }

            #endregion

            #region Methods

            public static int CompareByZOrder(ViewportBinding objOne, ViewportBinding objTwo)
            {
                if (objOne._zOrder > objTwo._zOrder) return -1;

                return (objOne._zOrder < objTwo._zOrder) ? 1 : 0;
            }

            #endregion

            #region Properties

            public int Id
            {
                get { return _id; }
            }

            public Viewport Viewport
            {
                get { return _viewport; }
            }

            public ushort ZOrder
            {
                get { return _zOrder; }
                set
                {
                    _zOrder = value;
                    _viewport.ZOrder = value;
                }
            }

            #endregion
        }

        #endregion

        #region Fields

        private int _viewportCounter;
        private readonly GraphicsDeviceManager _deviceManager;
        private readonly GraphicsDevice _device;
        private ResizeRule _resizeRule = ResizeRule.None;
        private ReadOnlyCollection<SurfaceFormat> _supportedFormat;
        private readonly Dictionary<int, ReadOnlyCollection<DisplayMode>> _supportedDisplayMode = new Dictionary<int, ReadOnlyCollection<DisplayMode>>();
        private readonly List<ViewportBinding> _viewports = new List<ViewportBinding>();
        private bool _isDirty = true;

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

        internal Window(GraphicsDeviceManager deviceManager)
        {
            if (deviceManager == null)
            {
                throw new ArgumentNullException("deviceManager");
            }
            if (deviceManager.GraphicsDevice == null)
            {
                throw new Exception("GraphicsDevice cannot be null");
            }
            _deviceManager = deviceManager;
            _device = deviceManager.GraphicsDevice;
            Initialize();
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            ExtractDisplayMode();
            ExtractSupportedSurfaceFormat();
        }

        public void Render()
        {
            if (_isDirty)
            {
                ApplyChanges();
            }
        }

        private void ApplyChanges()
        {
            _deviceManager.ApplyChanges();
            _isDirty = false;
        }

        public int AddViewport(float width, float height, float top, float left)
        {
            Viewport vp = new Viewport(this, _device, width, height, top, left);
            ViewportBinding binding = new ViewportBinding(_viewportCounter++, vp);
            _viewports.Add(binding);

            return binding.Id;
        }

        public bool RemoveViewport(int id)
        {
            int idx = -1;
            for (int i = 0; i < _viewports.Count; i++)
            {
                ViewportBinding current = _viewports[i];
                if(current.Id != id) continue;
                idx = i;
                break;
            }

            if (idx <= -1) return false;
            _viewports.RemoveAt(idx);

            return true;
        }

        public Viewport GetViewport(int id)
        {
            ViewportBinding binding = GetViewportBinding(id);

            return binding == null ? null : binding.Viewport;
        }

        private ViewportBinding GetViewportBinding(int id)
        {
            ViewportBinding binding = null;
            for (int i = 0; i < _viewports.Count; i++)
            {
                ViewportBinding current = _viewports[i];
                if (current.Id != id) continue;
                binding = current;
                break;
            }

            return binding;
        }

        public void SetViewportZOrder(int id, ushort z)
        {
            ViewportBinding binding = GetViewportBinding(id);
            if(binding == null) return;
            binding.ZOrder = z;
            _viewports.Sort(ViewportBinding.CompareByZOrder);
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

            _deviceManager.PreferredBackBufferWidth = width;
            _deviceManager.PreferredBackBufferHeight = height;
            _isDirty = true;

            WindowDisplayModeChangedEventArgs args = new WindowDisplayModeChangedEventArgs(_device.Adapter.CurrentDisplayMode, this);
            OnDisplayModeChanged(this, args);
        }

        public void SetFullScreen(bool isFullScreen)
        {
            if (isFullScreen)
            {
                int newWidth = _deviceManager.PreferredBackBufferWidth;
                int newHeight = _deviceManager.PreferredBackBufferHeight;
                DisplayMode mode = FindFullScreenResolution(_resizeRule, newWidth, newHeight);
                if (_resizeRule != ResizeRule.None)
                {
                    if (mode == null)
                    {
                        throw new Exception("No valid resolution find for fullscreen mode");
                    }
                    newWidth = mode.Width;
                    newHeight = mode.Height;
                }

                _deviceManager.PreferredBackBufferWidth = newWidth;
                _deviceManager.PreferredBackBufferHeight = newHeight;
            }

            _deviceManager.IsFullScreen = isFullScreen;
            _isDirty = true;

            WindowFullScreenSwitchedEventArgs args = new WindowFullScreenSwitchedEventArgs(isFullScreen, this);
            OnFullScreenSwitched(this, args);
        }

        public void SetSurfaceFormat(SurfaceFormat format)
        {
            SurfaceFormat selectedFormat;
            DepthFormat selectedDepthFormat;
            int selectedMultiSampleCount;
            _device.Adapter.QueryBackBufferFormat(_deviceManager.GraphicsProfile, format, _deviceManager.PreferredDepthStencilFormat, 
                0, out selectedFormat, out selectedDepthFormat, out selectedMultiSampleCount);
            if (selectedFormat != format)
            {
                throw new NotSupportedException(string.Format("SurfaceFormat {0} not supported for back buffer", format));
            }

            _deviceManager.PreferredBackBufferFormat = format;
            _isDirty = true;
        }

        public void SetDepthFormat(DepthFormat depth)
        {
            SurfaceFormat selectedFormat;
            DepthFormat selectedDepthFormat;
            int selectedMultiSampleCount;
            _device.Adapter.QueryBackBufferFormat(_deviceManager.GraphicsProfile, _deviceManager.PreferredBackBufferFormat, 
                depth, 0, out selectedFormat, out selectedDepthFormat, out selectedMultiSampleCount);
            if (selectedDepthFormat != depth)
            {
                throw new NotSupportedException(string.Format("DepthFormat {0} not supported for back buffer", depth));
            }

            _deviceManager.PreferredDepthStencilFormat = depth;
            _isDirty = true;
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

        public bool IsValidResolution(int width, int height)
        {
            if (!_deviceManager.IsFullScreen)
            {
                return true;
            }

            ReadOnlyCollection<DisplayMode> availableModes = GetAvailableModes();
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
            int currentFormat = (int)_deviceManager.PreferredBackBufferFormat;
            ReadOnlyCollection<DisplayMode> availableModes;

            return !_supportedDisplayMode.TryGetValue(currentFormat, out availableModes) ? null : availableModes;
        }

        private void ExtractDisplayMode()
        {
            DisplayModeCollection modes = _device.Adapter.SupportedDisplayModes;
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
            _supportedFormat = new ReadOnlyCollection<SurfaceFormat>(supported);
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

        public int Height
        {
            get { return _deviceManager.PreferredBackBufferHeight; }
        }

        public int Width
        {
            get { return _deviceManager.PreferredBackBufferWidth; }
        }

        public bool IsFullScreen
        {
            get { return _deviceManager.IsFullScreen; }
        }

        public SurfaceFormat Surface
        {
            get { return _deviceManager.PreferredBackBufferFormat; }
        }

        public DepthFormat Depth
        {
            get { return _deviceManager.PreferredDepthStencilFormat; }
        }

        public bool VSync
        {
            get { return _deviceManager.SynchronizeWithVerticalRetrace; }
            set
            {
                _deviceManager.SynchronizeWithVerticalRetrace = value;
                _isDirty = true;
            }
        }

        public ResizeRule PreferredResizeRule
        {
            get { return _resizeRule; }
            set { _resizeRule = value; }
        }

        public ReadOnlyCollection<SurfaceFormat> SupportedSurfaceFormat
        {
            get { return _supportedFormat; }
        }

        public ReadOnlyCollection<DisplayMode> AvailableDisplayMode
        {
            get { return GetAvailableModes(); }
        }

        #endregion
    }
}