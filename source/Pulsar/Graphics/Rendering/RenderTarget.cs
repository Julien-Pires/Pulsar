using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Specifies the position of a viewport, can be combined
    /// </summary>
    [Flags]
    public enum ViewportPosition
    {
        /// <summary>
        /// Occupies the half top
        /// </summary>
        Top = 0,
        /// <summary>
        /// Occupies the half bottom
        /// </summary>
        Bottom = 1,
        /// <summary>
        /// Occupies the half left
        /// </summary>
        Left = 2,
        /// <summary>
        /// Occupies the half right
        /// </summary>
        Right = 4
    }

    /// <summary>
    /// Used as a render target for multiple rendering
    /// </summary>
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

        /// <summary>
        /// Constructor of RenderTarget class
        /// </summary>
        /// <param name="deviceManager"></param>
        /// <param name="renderer"></param>
        internal RenderTarget(GraphicsDeviceManager deviceManager, Renderer renderer)
        {
            if (deviceManager == null) throw new ArgumentNullException("deviceManager");
            if(renderer == null) throw new ArgumentNullException("renderer");
            DeviceManager = deviceManager;
            Renderer = renderer;
            Viewports = new ViewportCollection(this);
            AlwaysClear = true;
            ClearColor = Color.CornflowerBlue;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="disposing">Indicate if the method is called from dispose</param>
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

        /// <summary>
        /// Fills the render target by rendering each viewports
        /// </summary>
        /// <param name="time">Time since last render</param>
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

        /// <summary>
        /// Called before rendering
        /// </summary>
        protected virtual void PreRender()
        {
        }

        /// <summary>
        /// Called after rendering
        /// </summary>
        protected virtual void PostRender()
        {
        }

        /// <summary>
        /// Recreate resources if render target parameters has changed
        /// </summary>
        private void ApplyChanges()
        {
            if (Target != null) Target.Dispose();
            Target = new RenderTarget2D(DeviceManager.GraphicsDevice, Width, Height, MipMap, Pixel, Depth);
            _isDirty = false;
        }

        /// <summary>
        /// Set the resolution of the render target
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public virtual void SetResolution(int width, int height)
        {
            if (!IsValidResolution(width, height)) throw new Exception("Invalid resolution provided");
            Width = width;
            Height = height;
            _isDirty = true;
        }

        /// <summary>
        /// Set the depth buffer format
        /// </summary>
        /// <param name="depth">Depth buffer format</param>
        public virtual void SetDepth(DepthFormat depth)
        {
            if (!IsValidDepth(depth)) throw new Exception(string.Format("DepthFormat {0} is not supported", depth));
            Depth = depth;
            _isDirty = true;
        }

        /// <summary>
        /// Set the pixel format of the render target
        /// </summary>
        /// <param name="pixel">Pixel format</param>
        public virtual void SetPixel(SurfaceFormat pixel)
        {
            if (!IsValidPixel(pixel)) throw new Exception(string.Format("SurfaceFormat {0} is not supported", pixel));
            Pixel = pixel;
            _isDirty = true;
        }

        /// <summary>
        /// Indicates whether the specified resolution is valid
        /// </summary>
        /// <param name="width">With</param>
        /// <param name="height">Height</param>
        /// <returns>Returns true if the resolution is valid otherwise false</returns>
        public abstract bool IsValidResolution(int width, int height);

        /// <summary>
        /// Indicates whether the specified depth buffer format is valid
        /// </summary>
        /// <param name="depth">Depth buffer format</param>
        /// <returns>Returns true if the format is valid otherwise false</returns>
        public abstract bool IsValidDepth(DepthFormat depth);

        /// <summary>
        /// Indicates whether the specified pixel format is valid
        /// </summary>
        /// <param name="pixel">Pixel format</param>
        /// <returns>Returns true if the format is valid otherwise false</returns>
        public abstract bool IsValidPixel(SurfaceFormat pixel);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the RenderTarget2D instance of this render target
        /// </summary>
        internal RenderTarget2D Target { get; private set; }

        /// <summary>
        /// Gets the graphic device associated to this render target
        /// </summary>
        internal GraphicsDevice GraphicsDevice
        {
            get { return DeviceManager.GraphicsDevice; }
        }

        /// <summary>
        /// Gets frame statistics
        /// </summary>
        public FrameStatistics FrameStatistics
        {
            get { return _frameStatistics; }
        }

        /// <summary>
        /// Gets the collection of viewports
        /// </summary>
        public ViewportCollection Viewports { get; private set; }

        /// <summary>
        /// Gets or sets the color used to clear the render target
        /// </summary>
        public Color ClearColor { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the render target should always be cleared before rendering
        /// </summary>
        public bool AlwaysClear { get; set; }

        /// <summary>
        /// Gets the height
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets the width
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets or sets a value that indicates whether mipmap is activated
        /// </summary>
        public virtual bool MipMap
        {
            get { return _mipmap; }
            set
            {
                _mipmap = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets the pixel format
        /// </summary>
        public SurfaceFormat Pixel { get; private set; }

        /// <summary>
        /// Gets the depth buffer format
        /// </summary>
        public DepthFormat Depth { get; private set; }

        #endregion
    }
}
