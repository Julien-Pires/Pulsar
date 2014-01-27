using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents a part of a render target
    /// </summary>
    public sealed class Viewport : IDisposable
    {
        #region Fields

        public Camera Camera;

        private float _width;
        private float _height;
        private float _topPosition;
        private float _leftPosition;
        private bool _isDisposed;
        private bool _isDirty = true;
        private readonly RenderTarget _parent;
        private readonly FrameDetail _frameDetail = new FrameDetail();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of Viewport class
        /// </summary>
        /// <param name="parent">Render target that owns this viewport</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="top">Top position</param>
        /// <param name="left">Left position</param>
        internal Viewport(RenderTarget parent, float width, float height, float top, float left)
        {
            _parent = parent;
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
            if (_isDisposed) return;

            try
            {
                Target.Dispose();
            }
            finally
            {
                Target = null;
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Renders the scene from the associated camera
        /// </summary>
        public void Render()
        {
            if (_isDirty) 
                UpdateDimension();

            _frameDetail.Reset();
            if(Camera != null) 
                Camera.Render(this);
        }

        /// <summary>
        /// Updates the render target if parameters has changed
        /// </summary>
        private void UpdateDimension()
        {
            int newWidth = (int)(_parent.Width * _width);
            int newHeight = (int)(_parent.Height * _height);
            if ((newWidth == PixelWidth) && (newHeight == PixelHeight)) return;

            PixelWidth = newWidth;
            PixelHeight = newHeight;
            AspectRatio = (float)newWidth/newHeight;
            CreateRenderTarget();
            _isDirty = false;
        }

        /// <summary>
        /// Updates the position of the viewport
        /// </summary>
        private void UpdatePosition()
        {
            PixelTop = (int)(_topPosition * _parent.Height);
            PixelLeft = (int)(_leftPosition * _parent.Width);
        }

        /// <summary>
        /// Creates a new render target
        /// </summary>
        private void CreateRenderTarget()
        {
            if(_isDisposed)
                throw new Exception("Cannot use a disposed viewport");

            if (Target != null) 
                Target.Dispose();

            Target = new RenderTarget2D(_parent.GraphicsDevice, PixelWidth, PixelHeight, _parent.MipMap,
                _parent.Pixel, _parent.Depth);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the render target associated to this viewport
        /// </summary>
        internal RenderTarget2D Target { get; private set; }

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
            get { return _parent.AlwaysClear; }
        }

        /// <summary>
        /// Gets or sets the color used to clear the viewport
        /// </summary>
        public Color ClearColor { get; set; }

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
        public int PixelTop { get; private set; }

        /// <summary>
        /// Gets the left position in pixel
        /// </summary>
        public int PixelLeft { get; private set; }

        /// <summary>
        /// Gets the normalized top position
        /// </summary>
        public float Top
        {
            get { return _topPosition; }
            set
            {
                if (value < 0.0f)
                    value = 0.0f;
                else if (value > 1.0f)
                    value = 1.0f;

                _topPosition = value;
                UpdatePosition();
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
                    value = 0.0f;
                else if (value > 1.0f)
                    value = 1.0f;

                _leftPosition = value;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Gets the width in pixel
        /// </summary>
        public int PixelWidth { get; private set; }

        /// <summary>
        /// Gets the height in pixel
        /// </summary>
        public int PixelHeight { get; private set; }

        /// <summary>
        /// Gets the normalized width
        /// </summary>
        public float Width
        {
            get { return _width; }
            set
            {
                if (value < 0.0f)
                    value = 0.0f;
                else if (value > 1.0f)
                    value = 1.0f;

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
                    value = 0.0f;
                else if (value > 1.0f)
                    value = 1.0f;

                _height = value;
                _isDirty = true;
            }
        }

        #endregion
    }
}