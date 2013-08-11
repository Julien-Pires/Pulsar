using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Represents a part of a render target
    /// </summary>
    public sealed class Viewport : IDisposable
    {
        #region Fields

        private float _width;
        private float _height;
        private float _topPosition;
        private float _leftPosition;
        private bool _disposed;
        private bool _isDirty = true;
        private readonly RenderTarget _parentTarget;
        public Camera Camera;
        private readonly FrameDetail _frameDetail = new FrameDetail();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Viewport class
        /// </summary>
        /// <param name="parentTarget">Render target that owns this viewport</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="top">Top position</param>
        /// <param name="left">Left position</param>
        internal Viewport(RenderTarget parentTarget, float width, float height, float top, float left)
        {
            _parentTarget = parentTarget;
            Width = width;
            Height = height;
            Top = top;
            Left = left;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">Indicates wether the method is called from IDisposable.Dispose</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing) RenderTarget.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Renders the scene from the associated camera
        /// </summary>
        public void Render()
        {
            if (_isDirty) UpdateDimension();
            _frameDetail.Reset();
            if(Camera != null) Camera.Render(this);
        }

        /// <summary>
        /// Updates the render target if parameters has changed
        /// </summary>
        private void UpdateDimension()
        {
            int targetWidth = _parentTarget.Width;
            int targetHeight = _parentTarget.Height;
            int newWidth = (int)(targetWidth * _width);
            int newHeight = (int)(targetHeight * _height);
            if ((newWidth != RealWidth) || (newHeight != RealHeight))
            {
                RealWidth = newWidth;
                RealHeight = newHeight;
                AspectRatio = (float)newWidth/newHeight;
                CreateRenderTarget();
            }
            RealTop = (int)(_topPosition * targetHeight);
            RealLeft = (int) (_leftPosition * targetWidth);

            _isDirty = false;
        }

        /// <summary>
        /// Creates a new render target
        /// </summary>
        private void CreateRenderTarget()
        {
            if (RenderTarget != null) RenderTarget.Dispose();

            RenderTarget = new RenderTarget2D(_parentTarget.GraphicsDevice, RealWidth, RealHeight, _parentTarget.MipMap,
                _parentTarget.Pixel, _parentTarget.Depth);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the render target associated to this viewport
        /// </summary>
        internal RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// Gets the statistics about the last rendered scene
        /// </summary>
        internal FrameDetail FrameDetail
        {
            get { return _frameDetail; }
        }

        /// <summary>
        /// Gets a value that indicates if the viewport is always cleared before rendering
        /// </summary>
        public bool AlwaysClear
        {
            get { return _parentTarget.AlwaysClear; }
        }

        /// <summary>
        /// Gets the color used to clear the viewport
        /// </summary>
        public Color ClearColor
        {
            get { return _parentTarget.ClearColor; }
        }

        /// <summary>
        /// Gets the aspect ratio
        /// </summary>
        public float AspectRatio { get; private set; }

        /// <summary>
        /// Gets the z-order
        /// </summary>
        public ushort ZOrder { get; internal set; }

        /// <summary>
        /// Gets the top position in pixel
        /// </summary>
        public int RealTop { get; private set; }

        /// <summary>
        /// Gets the left position in pixel
        /// </summary>
        public int RealLeft { get; private set; }

        /// <summary>
        /// Gets the normalized top position
        /// </summary>
        public float Top
        {
            get { return _topPosition; }
            set
            {
                if (value < 0.0f)
                {
                    value = 0.0f;
                }
                else if (value > 1.0f)
                {
                    value = 1.0f;
                }
                _topPosition = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets the normalized left position
        /// </summary>
        public float Left
        {
            get { return _leftPosition; }
            set
            {
                if (value < 0.0f)
                {
                    value = 0.0f;
                }
                else if (value > 1.0f)
                {
                    value = 1.0f;
                }
                _leftPosition = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets the width in pixel
        /// </summary>
        public int RealWidth { get; private set; }

        /// <summary>
        /// Gets the height in pixel
        /// </summary>
        public int RealHeight { get; private set; }

        /// <summary>
        /// Gets the normalized width
        /// </summary>
        public float Width
        {
            get { return _width; }
            set
            {
                if (value < 0.0f)
                {
                    value = 0.0f;
                }
                else if (value > 1.0f)
                {
                    value = 1.0f;   
                }
                _width = value;
                _isDirty = true;
            }
        }

        /// <summary>
        /// Gets the normalized height
        /// </summary>
        public float Height
        {
            get { return _width; }
            set
            {
                if (value < 0.0f)
                {
                    value = 0.0f;
                }
                else if (value > 1.0f)
                {
                    value = 1.0f;
                }
                _height = value;
                _isDirty = true;
            }
        }

        #endregion
    }
}