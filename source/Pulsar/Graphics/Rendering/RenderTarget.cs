using System;
using System.Collections.Generic;

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
        #region Nested

        internal sealed class ViewportBinding
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
        private readonly List<ViewportBinding> _viewports = new List<ViewportBinding>();
        private bool _mipmap;
        private bool _disposed;
        private bool _isDirty = true;
        protected readonly Renderer Renderer;
        protected readonly GraphicsDeviceManager DeviceManager;

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
            AlwaysClear = true;
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
                for (int i = 0; i < _viewports.Count; i++)
                {
                    _viewports[i].Viewport.Dispose();
                }
            }
            _disposed = true;
        }

        internal void Render()
        {
            PreRender();

            if (_isDirty) ApplyChanges();
            if (Target != null) Renderer.RenderToTarget(this);

            PostRender();
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

        public int AddViewport()
        {
            return AddViewport(1.0f, 1.0f, 0.0f, 0.0f);
        }

        public int AddViewport(ViewportPosition position)
        {
            float width = 1.0f;
            float height = 1.0f;
            float top = 0.0f;
            float left = 0.0f;

            if ((position & ViewportPosition.Bottom) == ViewportPosition.Bottom)
            {
                top += 0.5f;
                height -= 0.5f;
            }
            else if ((position & ViewportPosition.Top) == ViewportPosition.Top)
            {
                height -= 0.5f;
            }

            if ((position & ViewportPosition.Right) == ViewportPosition.Right)
            {
                left += 0.5f;
                width -= 0.5f;
            }
            else if ((position & ViewportPosition.Left) == ViewportPosition.Left)
            {
                width -= 0.5f;
            }

            return AddViewport(width, height, top, left);
        }

        public int AddViewport(float width, float height, float top, float left)
        {
            Viewport vp = new Viewport(this, DeviceManager.GraphicsDevice, width, height, top, left);
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
                if (current.Id != id) continue;
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
            if (binding == null) return;
            binding.ZOrder = z;
            _viewports.Sort(ViewportBinding.CompareByZOrder);
        }

        #endregion

        #region Properties

        internal List<ViewportBinding> Viewports
        {
            get { return _viewports; }
        }

        internal RenderTarget2D Target { get; private set; }

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
