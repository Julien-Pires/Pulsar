using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    [Flags]
    public enum ViewportPosition
    {
        Top = 0,
        Bottom = 1,
        Left = 2,
        Right = 4
    }

    public abstract class RenderTarget : IDisposable
    {
        #region Fields

        private bool _mipmap;
        private bool _disposed;
        private bool _isDirty = true;
        protected readonly Renderer Renderer;
        protected readonly GraphicsDeviceManager DeviceManager;
        private readonly FrameStatistics _frameStatistics = new FrameStatistics();

        #endregion

        #region Constructor

        internal RenderTarget(GraphicsDeviceManager deviceManager, Renderer renderer)
        {
            if (deviceManager == null)
            {
                throw new ArgumentNullException("deviceManager");
            }
            DeviceManager = deviceManager;
            Renderer = renderer;
            Viewports = new ViewportCollection(this);
            AlwaysClear = true;
            ClearColor = Color.CornflowerBlue;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_disposed) return;

            if (disposing)
            {
                Target.Dispose();
                Viewports.Dispose();
            }
            _disposed = true;
        }

        internal void Render(GameTime time)
        {
            PreRender();

            if (_isDirty) ApplyChanges();
            if (Viewports.Count > 0)
            {
                FrameDetail currentFrame = _frameStatistics.CurrentFrame;
                for (int i = 0; i < Viewports.Count; i++)
                {
                    Viewport vp = Viewports[i];
                    vp.Render();
                    currentFrame.Merge(vp.FrameDetail);
                }
            }
            if (Target != null) Renderer.RenderToTarget(this);

            PostRender();

            _frameStatistics.SaveCurrentFrame();
            _frameStatistics.Framecount++;
            _frameStatistics.ComputeFramerate(time);
        }

        protected virtual void PreRender()
        {
        }

        protected virtual void PostRender()
        {
        }

        private void ApplyChanges()
        {
            if (Target != null)
            {
                Target.Dispose();
            }
            Target = new RenderTarget2D(DeviceManager.GraphicsDevice, Width, Height, MipMap, Pixel, Depth);
            _isDirty = false;
        }

        public virtual void SetResolution(int width, int height)
        {
            if (!IsValidResolution(width, height))
            {
                throw new Exception("Invalid resolution provided");
            }
            Width = width;
            Height = height;
            _isDirty = true;
        }

        public virtual void SetDepth(DepthFormat depth)
        {
            if (!IsValidDepth(depth))
            {
                throw new Exception(string.Format("DepthFormat {0} is not supported", depth));
            }
            Depth = depth;
            _isDirty = true;
        }

        public virtual void SetPixel(SurfaceFormat pixel)
        {
            if (!IsValidPixel(pixel))
            {
                throw new Exception(string.Format("SurfaceFormat {0} is not supported", pixel));
            }
            Pixel = pixel;
            _isDirty = true;
        }

        public abstract bool IsValidResolution(int width, int height);

        public abstract bool IsValidDepth(DepthFormat depth);

        public abstract bool IsValidPixel(SurfaceFormat pixel);

        #endregion

        #region Properties

        internal RenderTarget2D Target { get; private set; }

        internal GraphicsDevice GraphicsDevice
        {
            get { return DeviceManager.GraphicsDevice; }
        }

        public FrameStatistics FrameStatistics
        {
            get { return _frameStatistics; }
        }

        public ViewportCollection Viewports { get; private set; }

        public Color ClearColor { get; set; }

        public bool AlwaysClear { get; set; }

        public int Height { get; private set; }

        public int Width { get; private set; }

        public virtual bool MipMap
        {
            get { return _mipmap; }
            set
            {
                _mipmap = value;
                _isDirty = true;
            }
        }

        public SurfaceFormat Pixel { get; private set; }

        public DepthFormat Depth { get; private set; }

        #endregion
    }
}
