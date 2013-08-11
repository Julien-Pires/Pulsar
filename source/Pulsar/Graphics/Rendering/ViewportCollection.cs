using System;
using System.Collections.Generic;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Represents a collection of viewports
    /// </summary>
    public sealed class ViewportCollection : IDisposable
    {
        #region Nested

        /// <summary>
        /// Used to order the viewports
        /// </summary>
        internal sealed class ViewportBinding
        {
            #region Fields

            private readonly int _id;
            private ushort _zOrder;
            private readonly Viewport _viewport;

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor of ViewportBinding class
            /// </summary>
            /// <param name="id">Id of the viewport</param>
            /// <param name="vp">Viewport</param>
            public ViewportBinding(int id, Viewport vp)
            {
                _id = id;
                _viewport = vp;
                ZOrder = 0;
            }

            #endregion

            #region Static methods

            /// <summary>
            /// Compares the order of two viewports
            /// </summary>
            /// <param name="objOne">First ViewportBinding instance</param>
            /// <param name="objTwo">Second ViewportBinding instance</param>
            /// <returns>Returns -1 if the first viewport is on top of the second, 1 if it is under and 0 if they have the same z-order</returns>
            public static int CompareByZOrder(ViewportBinding objOne, ViewportBinding objTwo)
            {
                if (objOne._zOrder < objTwo._zOrder) return -1;

                return (objOne._zOrder > objTwo._zOrder) ? 1 : 0;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the Id
            /// </summary>
            public int Id
            {
                get { return _id; }
            }

            /// <summary>
            /// Gets the viewport
            /// </summary>
            public Viewport Viewport
            {
                get { return _viewport; }
            }

            /// <summary>
            /// Sets the z-order
            /// </summary>
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

        /// <summary>
        /// Constructor of ViewportCollection class
        /// </summary>
        /// <param name="owner">RenderTarget that owns this collection</param>
        internal ViewportCollection(RenderTarget owner)
        {
            _owner = owner;
        }

        #endregion

        #region Method

        /// <summary>
        /// Gets the viewport at the specified index
        /// </summary>
        /// <param name="index">Index of the viewport</param>
        /// <returns>Returns a viewport</returns>
        public Viewport this[int index]
        {
            get { return _viewports[index].Viewport; }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            for (int i = 0; i < _viewports.Count; i++)
            {
                _viewports[i].Viewport.Dispose();
            }
            _disposed = true;
        }

        /// <summary>
        /// Creates a viewport that occupies entirely the render target
        /// </summary>
        /// <returns>Returns a viewport</returns>
        public int CreateViewport()
        {
            return CreateViewport(1.0f, 1.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Creates a viewport with a ViewportPosition
        /// </summary>
        /// <param name="position">Position of the viewport</param>
        /// <returns>Returns a viewport</returns>
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

        /// <summary>
        /// Creates a viewport with pixel resolution
        /// </summary>
        /// <param name="width">Pixel width</param>
        /// <param name="height">Pixel height</param>
        /// <param name="top">Top position</param>
        /// <param name="left">Left position</param>
        /// <returns>Returns a viewport</returns>
        public int CreateViewport(int width, int height, float top, float left)
        {
            float normWidth = (float)width/_owner.Width;
            float normHeight = (float)height/_owner.Height;

            return CreateViewport(normWidth, normHeight, top, left);
        }

        /// <summary>
        /// Creates a viewport with normalized resolution
        /// </summary>
        /// <param name="width">Normalized width</param>
        /// <param name="height">Normalized height</param>
        /// <param name="top">Top position</param>
        /// <param name="left">Left position</param>
        /// <returns>Returns a viewport</returns>
        public int CreateViewport(float width, float height, float top, float left)
        {
            Viewport vp = new Viewport(_owner, width, height, top, left);
            ViewportBinding binding = new ViewportBinding(_viewportCounter++, vp);
            _viewports.Add(binding);
            _viewports.Sort(ViewportBinding.CompareByZOrder);

            return binding.Id;
        }

        /// <summary>
        /// Destroy a viewport from this collection
        /// </summary>
        /// <param name="id">Id of the viewport</param>
        /// <returns>Returns true if the viewport is destroyed otherwise false</returns>
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

        /// <summary>
        /// Gets a viewport with a specified Id
        /// </summary>
        /// <param name="id">Id of the viewport</param>
        /// <returns>Returns the viewport with the specified Id if it exists, otherwise null</returns>
        public Viewport GetViewport(int id)
        {
            ViewportBinding binding = GetViewportBinding(id);

            return binding == null ? null : binding.Viewport;
        }

        /// <summary>
        /// Gets a ViewportBinding with a specified Id
        /// </summary>
        /// <param name="id">Id of the viewport</param>
        /// <returns>Returns a ViewportBinding</returns>
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

        /// <summary>
        /// Sets the z-order of a viewport with a specified Id
        /// </summary>
        /// <param name="id">Id of the viewport</param>
        /// <param name="z">Z-order</param>
        public void SetZOrder(int id, ushort z)
        {
            ViewportBinding binding = GetViewportBinding(id);
            if (binding == null) return;
            binding.ZOrder = z;
            _viewports.Sort(ViewportBinding.CompareByZOrder);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of viewports
        /// </summary>
        public int Count
        {
            get { return _viewports.Count; }   
        }

        #endregion
    }
}
