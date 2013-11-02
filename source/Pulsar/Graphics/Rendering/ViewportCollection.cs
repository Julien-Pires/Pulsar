using System;
using System.Collections.Generic;

namespace Pulsar.Graphics.Rendering
{
    /// <summary>
    /// Represents a collection of viewports
    /// </summary>
    public sealed class ViewportCollection : IDisposable
    {
        #region Fields

        private bool _disposed;
        private readonly List<Viewport> _viewports = new List<Viewport>();
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

        #region Static method

        /// <summary>
        /// Compares two viewports by theirs Z-order
        /// </summary>
        /// <param name="vpOne">First viewport</param>
        /// <param name="vpTwo">Second viewport</param>
        /// <returns>Returns -1 if the first viewport is lower, 1 if higher otherwise 0 if equals</returns>
        public static int CompareByZOrder(Viewport vpOne, Viewport vpTwo)
        {
            if (vpOne.ZOrder < vpTwo.ZOrder) return -1;

            return (vpOne.ZOrder > vpTwo.ZOrder) ? 1 : 0;
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
            get { return _viewports[index]; }
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            for (int i = 0; i < _viewports.Count; i++)
                _viewports[i].Dispose();

            _disposed = true;
        }

        /// <summary>
        /// Creates a viewport that occupies entirely the render target
        /// </summary>
        /// <returns>Returns a viewport</returns>
        public Viewport CreateViewport(ushort zOrder)
        {
            return CreateViewport(1.0f, 1.0f, 0.0f, 0.0f, zOrder);
        }

        /// <summary>
        /// Creates a viewport with a ViewportPosition
        /// </summary>
        /// <param name="position">Position of the viewport</param>
        /// <param name="zOrder">Z-order of the viewport</param>    
        /// <returns>Returns a viewport</returns>
        public Viewport CreateViewport(ViewportPosition position, ushort zOrder)
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
                height -= 0.5f;

            if ((position & ViewportPosition.Right) == ViewportPosition.Right)
            {
                left += 0.5f;
                width -= 0.5f;
            }
            else if ((position & ViewportPosition.Left) == ViewportPosition.Left)
                width -= 0.5f;

            return CreateViewport(width, height, top, left, zOrder);
        }

        /// <summary>
        /// Creates a viewport with pixel resolution
        /// </summary>
        /// <param name="width">Pixel width</param>
        /// <param name="height">Pixel height</param>
        /// <param name="top">Top position</param>
        /// <param name="left">Left position</param>
        /// <param name="zOrder">Z-order of the viewport</param>
        /// <returns>Returns a viewport</returns>
        public Viewport CreateViewport(int width, int height, float top, float left, ushort zOrder)
        {
            float normWidth = (float)width/_owner.Width;
            float normHeight = (float)height/_owner.Height;

            return CreateViewport(normWidth, normHeight, top, left, zOrder);
        }

        /// <summary>
        /// Creates a viewport with normalized resolution
        /// </summary>
        /// <param name="width">Normalized width</param>
        /// <param name="height">Normalized height</param>
        /// <param name="top">Top position</param>
        /// <param name="left">Left position</param>
        /// <param name="zOrder">Z-order of the viewport</param>
        /// <returns>Returns a viewport</returns>
        public Viewport CreateViewport(float width, float height, float top, float left, ushort zOrder)
        {
            if(GetViewportIndex(zOrder) > -1)
                throw new ArgumentException(string.Format("Viewport with zOrder {0} already exist", zOrder));

            Viewport vp = new Viewport(_owner, width, height, top, left)
            {
                ZOrder = zOrder
            };
            _viewports.Add(vp);
            _viewports.Sort(CompareByZOrder);

            return vp;
        }

        /// <summary>
        /// Destroys a viewport from its Z-order
        /// </summary>
        /// <param name="zOrder">Z-order of the viewport</param>
        public void DestroyViewport(ushort zOrder)
        {
            int index = GetViewportIndex(zOrder);
            if(index == -1)
                throw new Exception(string.Format("No viewport found with zOrder {0}", zOrder));

            DestroyViewport(index);
        }

        /// <summary>
        /// Destroys a viewport at the specified index
        /// </summary>
        /// <param name="index">Index of the viewport</param>
        public void DestroyViewport(int index)
        {
            Viewport vp = _viewports[index];
            _viewports.RemoveAt(index);
            vp.Dispose();
        }

        /// <summary>
        /// Gets the index of a viewport from its Z-order
        /// </summary>
        /// <param name="zOrder">Z-order of the viewport</param>
        /// <returns>Returns the index of the viewport if it exists, otherwise -1</returns>
        private int GetViewportIndex(ushort zOrder)
        {
            int index = -1;
            for (int i = 0; i < _viewports.Count; i++)
            {
                if(_viewports[i].ZOrder != zOrder) continue;

                index = i;
                break;
            }

            return index;
        }

        /// <summary>
        /// Gets a viewport with a specified Z-order
        /// </summary>
        /// <param name="zOrder">Z-order of the viewport</param>
        /// <returns>Returns the viewport with the specified Z-order if it exists, otherwise null</returns>
        public Viewport GetViewport(ushort zOrder)
        {
            int index = GetViewportIndex(zOrder);

            return (index == -1) ? null : _viewports[index];
        }

        /// <summary>
        /// Sets a new Z-order for a viewport with a specified Z-order
        /// </summary>
        /// <param name="oldZOrder">Old Z-order of the viewport</param>
        /// <param name="newZOrder">New Z-order value</param>
        public void UpdateViewportZOrder(ushort oldZOrder, ushort newZOrder)
        {
            int index = GetViewportIndex(oldZOrder);
            if(index == -1)
                throw new Exception(string.Format("No viewport found with zOrder {0}", oldZOrder));

            _viewports[index].ZOrder = newZOrder;
            _viewports.Sort(CompareByZOrder);
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
