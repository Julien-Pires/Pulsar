using System;
using System.Collections.Generic;

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

        internal readonly List<Viewport> Viewports = new List<Viewport>();

        protected readonly Renderer Renderer;
        protected readonly GraphicsDeviceManager DeviceManager;
        
        private bool _mipmap;
        private bool _disposed;
        private bool _isDirty = true;
        private readonly FrameDetail _frameDetail = new FrameDetail();

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
            AlwaysClear = true;
            ClearColor = Color.CornflowerBlue;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Gets the viewport at the specified index
        /// </summary>
        /// <param name="index">Index of the viewport</param>
        /// <returns>Returns a viewport</returns>
        public Viewport this[int index]
        {
            get { return Viewports[index]; }
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

        #region Methods

        /// <summary>
        /// Dispose resources
        /// </summary>
        /// <param name="disposing">Indicate if the method is called from dispose</param>
        protected virtual void Dispose(bool disposing)
        {
            if(_disposed) return;

            if (disposing)
            {
                for (int i = 0; i < Viewports.Count; i++)
                    Viewports[i].Dispose();

                Target.Dispose();
            }
            _disposed = true;
        }

        /// <summary>
        /// Fills the render target by rendering each viewports
        /// </summary>
        /// <param name="time">Time since last render</param>
        internal void Render(GameTime time)
        {
            _frameDetail.Reset();
            Renderer.FrameDetail.Reset();

            PreRender();

            if (_isDirty) 
                ApplyChanges();

            if (Target != null) 
                Renderer.RenderToTarget(this);

            PostRender();

            _frameDetail.Merge(Renderer.FrameDetail);
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
            if (Target != null) 
                Target.Dispose();

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
            if (!IsValidResolution(width, height)) 
                throw new Exception("Invalid resolution provided");

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
            if (!IsValidDepth(depth)) 
                throw new Exception(string.Format("DepthFormat {0} is not supported", depth));

            Depth = depth;
            _isDirty = true;
        }

        /// <summary>
        /// Set the pixel format of the render target
        /// </summary>
        /// <param name="pixel">Pixel format</param>
        public virtual void SetPixel(SurfaceFormat pixel)
        {
            if (!IsValidPixel(pixel)) 
                throw new Exception(string.Format("SurfaceFormat {0} is not supported", pixel));

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

        /// <summary>
        /// Disposes resources
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            for (int i = 0; i < Viewports.Count; i++)
                Viewports[i].Dispose();

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
            float normWidth = (float)width / Width;
            float normHeight = (float)height / Height;

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
            if (GetViewportIndex(zOrder) > -1)
                throw new ArgumentException(string.Format("Viewport with zOrder {0} already exist", zOrder));

            Viewport vp = new Viewport(this, width, height, top, left)
            {
                ZOrder = zOrder
            };
            Viewports.Add(vp);
            Viewports.Sort(CompareByZOrder);

            return vp;
        }

        /// <summary>
        /// Destroys a viewport from its Z-order
        /// </summary>
        /// <param name="zOrder">Z-order of the viewport</param>
        public void DestroyViewport(ushort zOrder)
        {
            int index = GetViewportIndex(zOrder);
            if (index == -1)
                throw new Exception(string.Format("No viewport found with zOrder {0}", zOrder));

            DestroyViewport(index);
        }

        /// <summary>
        /// Destroys a viewport at the specified index
        /// </summary>
        /// <param name="index">Index of the viewport</param>
        public void DestroyViewport(int index)
        {
            Viewport vp = Viewports[index];
            Viewports.RemoveAt(index);
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
            for (int i = 0; i < Viewports.Count; i++)
            {
                if (Viewports[i].ZOrder != zOrder) continue;

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

            return (index == -1) ? null : Viewports[index];
        }

        /// <summary>
        /// Sets a new Z-order for a viewport with a specified Z-order
        /// </summary>
        /// <param name="oldZOrder">Old Z-order of the viewport</param>
        /// <param name="newZOrder">New Z-order value</param>
        public void UpdateViewportZOrder(ushort oldZOrder, ushort newZOrder)
        {
            int index = GetViewportIndex(oldZOrder);
            if (index == -1)
                throw new Exception(string.Format("No viewport found with zOrder {0}", oldZOrder));

            Viewports[index].ZOrder = newZOrder;
            Viewports.Sort(CompareByZOrder);
        }

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
        public FrameDetail FrameDetail
        {
            get { return _frameDetail; }
        }

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

        /// <summary>
        /// Gets the number of viewports
        /// </summary>
        public int Count
        {
            get { return Viewports.Count; }
        }

        #endregion
    }
}
