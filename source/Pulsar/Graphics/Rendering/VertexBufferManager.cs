using System;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Graphics.Rendering
{
    public enum BufferType
    {
        Static,
        StaticWriteOnly,
        Dynamic,
        DynamicWriteOnly
    }

    public sealed class VertexBufferManager
    {
        #region Fields

        private GraphicsEngine engine;

        #endregion

        #region Constructors

        internal VertexBufferManager(GraphicsEngine engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException("GraphicsEngine cannot be null");
            }
            this.engine = engine;
        }

        #endregion

        #region Methods

        public VertexBufferObject CreateBuffer(BufferType bufferType, Type vertexType, int vertexCount)
        {
            VertexBuffer buffer = null;
            switch (bufferType)
            {
                case BufferType.Static: buffer = new VertexBuffer(engine.Renderer.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.None);
                    break;
                case BufferType.StaticWriteOnly: buffer = new VertexBuffer(engine.Renderer.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.WriteOnly);
                    break;
                case BufferType.Dynamic: buffer = new DynamicVertexBuffer(engine.Renderer.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.None);
                    break;
                case BufferType.DynamicWriteOnly: buffer = new DynamicVertexBuffer(engine.Renderer.GraphicsDevice, vertexType,
                    vertexCount, BufferUsage.WriteOnly);
                    break;
            }
            if (buffer == null)
            {
                throw new Exception("Failed to create buffer, wrong buffer type provided");
            }

            VertexDeclaration declaration = this.GetVertexDeclaration(vertexType);
            VertexBufferObject vbo = new VertexBufferObject(buffer, declaration);

            return vbo;
        }

        internal VertexBufferObject CreateBuffer(VertexBuffer buffer)
        {
            VertexDeclaration declaration = buffer.VertexDeclaration;
            VertexBufferObject vbo = new VertexBufferObject(buffer, declaration);

            return vbo;
        }

        private VertexDeclaration GetVertexDeclaration(Type vertexType)
        {
            if (vertexType == null)
            {
                throw new ArgumentNullException("Vertex type cannot be null");
            }
            IVertexType vertex = Activator.CreateInstance(vertexType) as IVertexType;
            if (vertex == null)
            {
                throw new ArgumentException("Vertex type not a IVertexType");
            }
            VertexDeclaration declaration = vertex.VertexDeclaration;
            if (declaration == null)
            {
                throw new ArgumentException("Vertex type doesn't have VertexDeclaration");
            }
            if (Marshal.SizeOf(vertexType) != declaration.VertexStride)
            {
                throw new ArgumentException("Vertex type has a wrong size");
            }

            return declaration;
        }

        #endregion
    }
}
