using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics
{
    public sealed class Viewport
    {
        #region Fields

        private float _width;
        private float _height;
        private float _topPosition;
        private float _leftPosition;
        private bool _isDirty;
        private readonly GraphicsDevice _device;
        private readonly Window _target;

        #endregion

        #region Constructor

        internal Viewport(Window target, GraphicsDevice device, float width, float height, float top, float left)
        {
            _target = target;
            _device = device;
            Width = width;
            Height = height;
            Top = top;
            Left = left;
        }

        #endregion

        #region Methods

        public void Render(GameTime time)
        {
            if (_isDirty)
            {
                UpdateSize();
            }
        }

        internal void UpdateSize()
        {
            int targetWidth = _target.Width;
            int targetHeight = _target.Height;
            int newWidth = (int)(targetWidth * _width);
            int newHeight = (int)(targetHeight * _height);

            if ((newWidth != RealWidth) || (newHeight != RealHeight))
            {
                if (newWidth > targetWidth) newWidth = targetWidth;
                if (newHeight > targetHeight) newHeight = targetHeight;
                RealWidth = newWidth;
                RealHeight = newHeight;
                AspectRatio = (float)newWidth/newHeight;

                CreateRenderTarget();
            }

            _isDirty = false;
        }

        private void CreateRenderTarget()
        {
            if (RenderTarget != null)
            {
                RenderTarget.Dispose();
                RenderTarget = null;
            }

            RenderTarget = new RenderTarget2D(_device, RealWidth, RealHeight, false,
                _target.Surface, _target.Depth);
        }

        #endregion

        #region Properties

        internal RenderTarget2D RenderTarget { get; private set; }

        public float AspectRatio { get; private set; }

        public ushort ZOrder { get; internal set; }

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