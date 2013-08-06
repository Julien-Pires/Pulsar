using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Graphics.Rendering;
using Pulsar.Graphics.SceneGraph;

namespace Pulsar.Graphics
{
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

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                RenderTarget.Dispose();
            }
            _disposed = true;
        }

        public void Render()
        {
            if (_isDirty) UpdateDimension();
            _frameDetail.Reset();
            if(Camera != null) Camera.Render(this);
        }

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

        private void CreateRenderTarget()
        {
            if (RenderTarget != null) RenderTarget.Dispose();

            RenderTarget = new RenderTarget2D(_parentTarget.GraphicsDevice, RealWidth, RealHeight, _parentTarget.MipMap,
                _parentTarget.Pixel, _parentTarget.Depth);
        }

        #endregion

        #region Properties

        internal RenderTarget2D RenderTarget { get; private set; }

        internal FrameDetail FrameDetail
        {
            get { return _frameDetail; }
        }

        public bool AlwaysClear
        {
            get { return _parentTarget.AlwaysClear; }
        }

        public Color ClearColor
        {
            get { return _parentTarget.ClearColor; }
        }

        public float AspectRatio { get; private set; }

        public ushort ZOrder { get; internal set; }

        public int RealTop { get; private set; }

        public int RealLeft { get; private set; }

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

        public int RealWidth { get; private set; }

        public int RealHeight { get; private set; }

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