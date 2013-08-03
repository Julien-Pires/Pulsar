using System;
using System.Collections.Generic;

namespace Pulsar.Graphics.Rendering
{
    public sealed class ViewportCollection : IDisposable
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

        private bool _disposed;
        private int _viewportCounter;
        private readonly List<ViewportBinding> _viewports = new List<ViewportBinding>();
        private readonly RenderTarget _owner;

        #endregion

        #region Constructor

        internal ViewportCollection(RenderTarget owner)
        {
            _owner = owner;
        }

        #endregion

        #region Method

        public Viewport this[int index]
        {
            get { return _viewports[index].Viewport; }
        }

        public void Dispose()
        {
            if (_disposed) return;

            for (int i = 0; i < _viewports.Count; i++)
            {
                _viewports[i].Viewport.Dispose();
            }
            _disposed = true;
        }

        public int CreateViewport()
        {
            return CreateViewport(1.0f, 1.0f, 0.0f, 0.0f);
        }

        public int CreateViewport(ViewportPosition position)
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

            return CreateViewport(width, height, top, left);
        }

        public int CreateViewport(int width, int height, float top, float left)
        {
            float normWidth = (float)width/_owner.Width;
            float normHeight = (float)height/_owner.Height;

            return CreateViewport(normWidth, normHeight, top, left);
        }

        public int CreateViewport(float width, float height, float top, float left)
        {
            Viewport vp = new Viewport(_owner, width, height, top, left);
            ViewportBinding binding = new ViewportBinding(_viewportCounter++, vp);
            _viewports.Add(binding);
            _viewports.Sort(ViewportBinding.CompareByZOrder);

            return binding.Id;
        }

        public bool DestroyViewport(int id)
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
            ViewportBinding vpBind = _viewports[idx];
            _viewports.RemoveAt(idx);
            vpBind.Viewport.Dispose();

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

        public void SetZOrder(int id, ushort z)
        {
            ViewportBinding binding = GetViewportBinding(id);
            if (binding == null) return;
            binding.ZOrder = z;
            _viewports.Sort(ViewportBinding.CompareByZOrder);
        }

        #endregion

        #region Properties

        public int Count
        {
            get { return _viewports.Count; }   
        }

        #endregion
    }
}
