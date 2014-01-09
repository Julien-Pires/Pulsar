using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pulsar.Extension;
using Pulsar.Graphics.Rendering;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Represents a 3D mesh
    /// A mesh can be created in three ways:
    ///     - From a file, using the XNA Content Pipeline (Easy)
    ///     - With helper function (Begin/Update/End) of the mesh class (Easy - Intermediate)
    ///     - From scratch, all buffer(Vertex/Index) must be created manually (Hard)
    /// </summary>
    public sealed class Mesh : IDisposable
    {
        #region Nested

        /// <summary>
        /// Contains informations about a submesh buffer
        /// </summary>
        private struct SubMeshBufferData
        {
            #region Fields

            /// <summary>
            /// A buffer used by a submesh
            /// </summary>
            public BufferObject Buffer;

            /// <summary>
            /// Starting offset in the buffer
            /// </summary>
            public int Offset;

            /// <summary>
            /// Number of element used in the buffer
            /// </summary>
            public int Count;

            /// <summary>
            /// Indicates that the buffer is shared with others submesh
            /// </summary>
            public readonly bool Shared;

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor of SubMeshBufferData struct
            /// </summary>
            /// <param name="buffer">Buffer object</param>
            /// <param name="offset">Starting offset</param>
            /// <param name="count">Number of elements used</param>
            /// <param name="shared">Indicates that buffer is shared</param>
            public SubMeshBufferData(BufferObject buffer, int offset, int count, bool shared)
            {
                Buffer = buffer;
                Offset = offset;
                Count = count;
                Shared = shared;
            }

            #endregion
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Represents the method that returns informations about a submesh buffer
        /// </summary>
        /// <param name="sub">Submesh to get informations from</param>
        /// <returns>Return a SubMeshBufferData with informations about a buffer</returns>
        private delegate SubMeshBufferData GetSubBufferData(SubMesh sub);

        #endregion

        #region Fields

        private bool _isUpdating;
        private bool _appendToBuffer;
        private bool _pendingVertex;
        private int _currentSub = -1;
        private VertexPositionNormalTexture _currentPoint;
        private List<VertexPositionNormalTexture> _currentVertices;
        private List<int> _currentIndices;
        private BufferType _currentIndexBufferType = BufferType.Static;
        private BufferType _currentVertexBufferType = BufferType.Static;
        private int _estimatedVertexCount;
        private int _estimatedIndexCount;

        private bool _isDisposed;
        private VertexData _vertexData = new VertexData();
        private IndexData _indexData = new IndexData();
        private readonly List<SubMesh> _subMeshes = new List<SubMesh>();
        private readonly Dictionary<string, int> _subMeshNamesMap = new Dictionary<string, int>();
        private BoundingBox _aabb;
        private BoundingSphere _boundingSphere;
        private readonly BufferManager _bufferManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the mesh class
        /// </summary>
        /// <param name="name">Name of the mesh</param>
        /// <param name="bufferManager">Buffer manager</param>
        internal Mesh(string name, BufferManager bufferManager)
        {
            _bufferManager = bufferManager;
            Name = name;
            Bones = new[] { Matrix.Identity };
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Add data to a buffer depending on the available space
        /// </summary>
        /// <typeparam name="T">Type of data contained in the buffer</typeparam>
        /// <param name="buffer">Buffer object</param>
        /// <param name="source">Source of data to add</param>
        /// <param name="offset">Offset from where to start adding data</param>
        /// <param name="length">Length used by existing data in case of a replacement</param>
        /// <returns>Returns the offset where the added data has been set</returns>
        private static int AddToBuffer<T>(BufferObject buffer, T[] source, int offset, int length) where T : struct
        {
            int newOffset;
            if (source.Length <= length)
            {
                newOffset = offset;
                buffer.SetData(offset, source, 0, source.Length, SetDataOptions.None);
            }
            else
            {
                T[] bufferData = new T[buffer.ElementCount];
                buffer.GetData(bufferData);
                int size = (buffer.ElementCount - length) + source.Length;
                T[] newData = new T[size];

                bufferData.SlicedCopyTo(offset, length, newData);
                newOffset = bufferData.Length - length;
                Array.Copy(source, 0, newData, newOffset, source.Length);
                buffer.SetData(newData);
            }

            return newOffset;
        }

        /// <summary>
        /// Shift data of buffer
        /// </summary>
        /// <typeparam name="T">Type of data contained in the buffer</typeparam>
        /// <param name="buffer">Buffer object</param>
        /// <param name="offset">Offset where the shift begin</param>
        /// <param name="length">Length of the shift</param>
        private static void ShiftBufferData<T>(BufferObject buffer, int offset, int length) where T : struct 
        {
            T[] bufferData = new T[buffer.ElementCount];
            buffer.GetData(bufferData);
            ArrayExtension.SlicedCopy(bufferData, offset, length, bufferData);

            int freeOffset = bufferData.Length - length;
            for (int i = freeOffset; i < bufferData.Length; i++) bufferData[i] = default (T);
            buffer.SetData(bufferData);
        }

        /// <summary>
        /// Get informations about submesh vertex buffer
        /// </summary>
        /// <param name="sub">Submesh from which informations are extracted</param>
        /// <returns>Return a SubMeshBufferData with informations about vertex buffer</returns>
        private static SubMeshBufferData GetSubVertexData(SubMesh sub)
        {
            if(sub.VertexData.BufferCount == 0) return new SubMeshBufferData();

            VertexData vertexData = sub.VertexData;

            return new SubMeshBufferData(vertexData.GetBuffer(0), vertexData.GetVertexOffset(0), 
                sub.RenderInfo.VertexCount, sub.ShareVertexBuffer);
        }

        /// <summary>
        /// Get informations about submesh index buffer
        /// </summary>
        /// <param name="sub">Submesh from which informations are extracted</param>
        /// <returns>Return a SubMeshBufferData with informations about index buffer</returns>
        private static SubMeshBufferData GetSubIndexData(SubMesh sub)
        {
            if(sub.IndexData.IndexBuffer == null) return new SubMeshBufferData();

            IndexData indexData = sub.IndexData;

            return new SubMeshBufferData(indexData.IndexBuffer, indexData.StartIndex, indexData.IndexCount, 
                sub.ShareIndexBuffer);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            if(_isDisposed) return;

            try
            {
                for(int i = 0; i < _subMeshes.Count; i++)
                    _subMeshes[i].Dispose();

                _vertexData.Dispose();
                _indexData.Dispose();
            }
            finally
            {
                _vertexData = null;
                _indexData = null;
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Begin to edit a new submesh
        /// </summary>
        public void Begin()
        {
            Begin(string.Empty, PrimitiveType.TriangleList, false, true, true);
        }

        /// <summary>
        /// Begin to edit a new submesh
        /// </summary>
        /// <param name="type">Defines how primitives are rendered</param>
        public void Begin(PrimitiveType type)
        {
            Begin(string.Empty, type, false, true, true);
        }

        /// <summary>
        /// Begin to edit a new submesh
        /// </summary>
        /// <param name="type">Defines how primitives are rendered</param>
        /// <param name="shareBuffer">Indicates that the submesh should use a shared buffer</param>
        /// <param name="staticVertex">Indicates that the submesh should use a static buffer</param>
        /// /// <param name="staticIndex">Indicates that the submesh should use a static index buffer</param>
        public void Begin(PrimitiveType type, bool shareBuffer, bool staticVertex, bool staticIndex)
        {
            Begin(string.Empty, type, shareBuffer, staticVertex, staticIndex);
        }

        /// <summary>
        /// Begin to edit a new submesh
        /// </summary>
        /// <param name="name">Name of the submesh</param>
        public void Begin(string name)
        {
            Begin(name, PrimitiveType.TriangleList, false, true, true);
        }

        /// <summary>
        /// Begin to edit a new submesh
        /// </summary>
        /// <param name="name">Name of the submesh</param>
        /// <param name="type">Defines how primitives are rendered</param>
        public void Begin(string name, PrimitiveType type)
        {
            Begin(name, type, false, true, true);
        }

        /// <summary>
        /// Begin to edit a new submesh
        /// </summary>
        /// <param name="name">Name of the submesh</param>
        /// <param name="type">Defines how primitives are rendered</param>
        /// <param name="shareBuffer">Indicates that the submesh should use a shared buffer</param>
        /// <param name="staticVertex">Indicates that the submesh should use a static vertex buffer</param>
        /// <param name="staticIndex">Indicates that the submesh should use a static index buffer</param>
        public void Begin(string name, PrimitiveType type, bool shareBuffer, bool staticVertex, bool staticIndex)
        {
            SubMesh sub = string.IsNullOrEmpty(name) ? CreateSubMesh() : CreateSubMesh(name);
            sub.RenderInfo.PrimitiveType = type;
            sub.ShareVertexBuffer = shareBuffer;
            sub.ShareIndexBuffer = shareBuffer;

            _currentVertices = new List<VertexPositionNormalTexture>();
            _currentIndices = new List<int>();
            _currentSub = _subMeshes.Count - 1;
            _currentVertexBufferType = staticVertex ? BufferType.Static : BufferType.Dynamic;
            _currentIndexBufferType = staticIndex ? BufferType.Static : BufferType.Dynamic;
        }

        /// <summary>
        /// Begin to update an existing submesh
        /// </summary>
        /// <param name="index">Index of the submesh</param>
        public void Update(int index)
        {
            Update(index, false);
        }

        /// <summary>
        /// Begin to update an existing submesh
        /// </summary>
        /// <param name="index">Index of the submesh</param>
        /// <param name="append">Indicates that data will be append to existing data instead of replaced</param>
        public void Update(int index, bool append)
        {
            GetSubMesh(index);

            _currentVertices = new List<VertexPositionNormalTexture>();
            _currentIndices = new List<int>();
            _isUpdating = true;
            _currentSub = index;
            _appendToBuffer = append;
        }

        /// <summary>
        /// Used to estimate the size of buffers
        /// </summary>
        /// <param name="vertexCount">Size of the vertex buffer</param>
        /// <param name="indexCount">Size of the index buffer</param>
        public void EstimateBufferSize(int vertexCount, int indexCount)
        {
            _estimatedVertexCount = vertexCount;
            _estimatedIndexCount = indexCount;
        }

        /// <summary>
        /// Stops to edit a submesh
        /// </summary>
        public void End()
        {
            if (_currentSub == -1) 
                throw new Exception("Begin or Update must be called first");

            if (_pendingVertex) 
                _currentVertices.Add(_currentPoint);

            if (_currentVertices.Count > 0)
            {
                _estimatedVertexCount = Math.Max(_estimatedVertexCount, _currentVertices.Count);
                _estimatedIndexCount = Math.Max(_estimatedIndexCount, _currentIndices.Count);

                SubMesh sub = _subMeshes[_currentSub];
                RenderingInfo renderingInfo = sub.RenderInfo;
                VertexPositionNormalTexture[] vertexSource = _currentVertices.ToArray();
                int[] indexSource = _currentIndices.ToArray();

                CopyVertexToBuffer(vertexSource);
                if (_currentIndices.Count > 0)
                {
                    CopyIndexToBuffer(indexSource);
                    renderingInfo.UseIndexes = true;
                }
                else 
                    renderingInfo.UseIndexes = false;
                renderingInfo.ComputePrimitiveCount();

                GenerateCurrentBoundingVolume(vertexSource);
                UpdateBounds();
                UpdateMeshInfo();
            }
            else 
                RemoveSubMesh(_currentSub);

            ResetManualConfig();
        }

        /// <summary>
        /// Starts a new vertex
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        public void Position(float x, float y, float z)
        {
            Vector3 position = new Vector3(x, y, z);
            Position(ref position);
        }

        /// <summary>
        /// Starts a new vertex
        /// </summary>
        /// <param name="position">Position</param>
        public void Position(Vector3 position)
        {
            Position(ref position);
        }

        /// <summary>
        /// Adds a new vertex and sets it as the current
        /// </summary>
        /// <param name="position">Position</param>
        public void Position(ref Vector3 position)
        {
            if(_currentSub == -1) throw new Exception("Begin or Update must be called first");

            if (_pendingVertex) _currentVertices.Add(_currentPoint);

            VertexPositionNormalTexture vertex = new VertexPositionNormalTexture
            {
                Position = position
            };
            _currentPoint = vertex;
            _pendingVertex = true;
        }

        /// <summary>
        /// Sets the normal for the current vertex
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="z">Z</param>
        public void Normal(float x, float y, float z)
        {
            Vector3 normal = new Vector3(x, y, z);
            Normal(ref normal);
        }

        /// <summary>
        /// Sets the normal for the current vertex
        /// </summary>
        /// <param name="normal">Normal</param>
        public void Normal(Vector3 normal)
        {
            Normal(ref normal);
        }

        /// <summary>
        /// Sets the normal for the current vertex
        /// </summary>
        /// <param name="normal">Normal</param>
        public void Normal(ref Vector3 normal)
        {
            if (_currentSub == -1) throw new Exception("Begin or Update must be called first");
            _currentPoint.Normal = normal;
        }

        /// <summary>
        /// Sets the texture coordinate for the current vertex
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void Texture(float x, float y)
        {
            Vector2 texture = new Vector2(x, y);
            Texture(ref texture);
        }

        /// <summary>
        /// Sets the texture coordinate for the current vertex
        /// </summary>
        /// <param name="texture">Texture coordinate</param>
        public void Texture(Vector2 texture)
        {
            Texture(ref texture);
        }

        /// <summary>
        /// Sets the texture coordinate for the current vertex
        /// </summary>
        /// <param name="texture">Texture coordinate</param>
        public void Texture(ref Vector2 texture)
        {
            if (_currentSub == -1) throw new Exception("Begin or Update must be called first");
            _currentPoint.TextureCoordinate = texture;
        }

        /// <summary>
        /// Adds a new index
        /// </summary>
        /// <param name="index">Index</param>
        public void Index(int index)
        {
            if (_currentSub == -1) throw new Exception("Begin or Update must be called first");
            _currentIndices.Add(index);
        }

        /// <summary>
        /// Create a new primitive from three indices
        /// </summary>
        /// <param name="index1">Index 1</param>
        /// <param name="index2">Index 2</param>
        /// <param name="index3">Index 3</param>
        public void Triangle(int index1, int index2, int index3)
        {
            Index(index1);
            Index(index2);
            Index(index3);
        }

        /// <summary>
        /// Create a new quad from four indices
        /// </summary>
        /// <param name="index1">Index 1</param>
        /// <param name="index2">Index 2</param>
        /// <param name="index3">Index 3</param>
        /// <param name="index4">Index 4</param>
        public void Quad(int index1, int index2, int index3, int index4)
        {
            Triangle(index1, index2, index3);
            Triangle(index3, index4, index1);
        }

        /// <summary>
        /// Resets all variables used to create and edit submesh
        /// </summary>
        private void ResetManualConfig()
        {
            _pendingVertex = false;
            _currentVertices = null;
            _currentIndices = null;
            _currentSub = -1;
            _appendToBuffer = false;
            _isUpdating = false;
            _currentVertexBufferType = BufferType.Static;
            _currentIndexBufferType = BufferType.Static;
        }

        /// <summary>
        /// Computes bounding volumes for the submesh that is being edited
        /// </summary>
        /// <param name="vertex">Array of vertices that composed the submesh</param>
        private void GenerateCurrentBoundingVolume(VertexPositionNormalTexture[] vertex)
        {
            SubMesh sub = _subMeshes[_currentSub];
            Vector3[] positions = new Vector3[vertex.Length];
            for (int i = 0; i < vertex.Length; i++) 
                positions[i] = vertex[i].Position;

            BoundingBox aabb = BoundingBox.CreateFromPoints(positions);
            BoundingSphere sphere = BoundingSphere.CreateFromPoints(positions);
            if (_appendToBuffer)
            {
                aabb = BoundingBox.CreateMerged(aabb, sub.AxisAlignedBoundingBox);
                sphere = BoundingSphere.CreateMerged(sphere, sub.BoundingSphere);
            }
            sub.AxisAlignedBoundingBox = aabb;
            sub.BoundingSphere = sphere;
        }

        /// <summary>
        /// Adds vertices to the vertex buffer of the submesh that is being edited
        /// </summary>
        /// <param name="source">Array of vertices</param>
        private void CopyVertexToBuffer(VertexPositionNormalTexture[] source)
        {
            SubMesh sub = _subMeshes[_currentSub];
            VertexData subVertexData = sub.VertexData;
            SubMeshBufferData subBufferData = GetSubVertexData(sub);
            VertexBufferObject buffer;

            int newOffset = 0;
            if (sub.ShareVertexBuffer)
            {
                buffer = EnsureVertexBuffer();
                if (!_isUpdating)
                {
                    subBufferData.Buffer = buffer;
                    subBufferData.Offset = FindNextBufferOffset(GetSubVertexData);
                    subBufferData.Count = source.Length;
                }
                newOffset = UpdateSharedVertex(source, subBufferData);
            }
            else
            {
                if (!_isUpdating)
                {
                    buffer = _bufferManager.CreateVertexBuffer(_currentVertexBufferType, typeof(VertexPositionNormalTexture),
                        _estimatedVertexCount);
                    subBufferData.Buffer = buffer;
                }
                else buffer = subVertexData.GetBuffer(0);

                UpdatePrivateBuffer(source, subBufferData);
            }
            
            subVertexData.UnsetBinding(0);
            subVertexData.SetBinding(buffer, newOffset, 0, 0);
            sub.RenderInfo.VertexCount = _appendToBuffer ? (sub.RenderInfo.VertexCount + source.Length) : source.Length;
        }

        /// <summary>
        /// Adds indices to the index buffer of the submesh that is being edited
        /// </summary>
        /// <param name="source">Array of indices</param>
        private void CopyIndexToBuffer(int[] source)
        {
            SubMesh sub = _subMeshes[_currentSub];
            IndexData subIndexData = sub.IndexData;
            SubMeshBufferData subBufferData = GetSubIndexData(sub);
            IndexBufferObject buffer;

            int newOffset = 0;
            if (sub.ShareIndexBuffer)
            {
                buffer = EnsureIndexBuffer();
                if (!_isUpdating)
                {
                    subBufferData.Buffer = buffer;
                    subBufferData.Offset = FindNextBufferOffset(GetSubIndexData);
                    subBufferData.Count = source.Length;
                }
                newOffset = UpdateSharedIndex(source, subBufferData);
            }
            else
            {
                if (!_isUpdating)
                {
                    buffer = _bufferManager.CreateIndexBuffer(_currentIndexBufferType, IndexElementSize.ThirtyTwoBits,
                        _estimatedIndexCount);
                    subBufferData.Buffer = buffer;
                }
                else buffer = subIndexData.IndexBuffer;

                UpdatePrivateBuffer(source, subBufferData);
            }

            subIndexData.IndexBuffer = buffer;
            subIndexData.StartIndex = newOffset;
            subIndexData.IndexCount = _appendToBuffer ? (subIndexData.IndexCount + source.Length) : source.Length;
        }

        /// <summary>
        /// Updates a non-shared buffer with new data
        /// </summary>
        /// <typeparam name="T">Type of data contained in the buffer</typeparam>
        /// <param name="source">Data to update the buffer with</param>
        /// <param name="subBufferData">Informations about the buffer</param>
        private void UpdatePrivateBuffer<T>(T[] source, SubMeshBufferData subBufferData) where T : struct
        {
            int offset = subBufferData.Offset;
            int length = subBufferData.Count;
            if (_isUpdating)
            {
                if (_appendToBuffer)
                {
                    offset = length;
                    length = source.Length;
                }
            }
            else length = source.Length;

            AddToBuffer(subBufferData.Buffer, source, offset, length);
        }

        /// <summary>
        /// Updates a shared buffer with new data
        /// </summary>
        /// <typeparam name="T">Type of data contained in the buffer</typeparam>
        /// <param name="source">Data to update the buffer with</param>
        /// <param name="subBufferData">Informations about the buffer</param>
        /// <param name="getSubBufferData">Method used to retrieve informations about buffer</param>
        /// <param name="newOffset">Starting offset in the buffer where data had been set</param>
        /// <param name="lengthUsed">Size of memory block used in the buffer</param>
        private void UpdateSharedBuffer<T>(T[] source, SubMeshBufferData subBufferData, 
            GetSubBufferData getSubBufferData, out int newOffset, out int lengthUsed) where T : struct
        {
            bool offsetChanged = true;
            int offset = subBufferData.Offset;
            int length = subBufferData.Count;
            if (_isUpdating)
            {
                int freeOffset, freeLength;
                bool spaceAvailable = CheckBufferFreeSpace(subBufferData.Offset, subBufferData.Count, 
                    getSubBufferData, out freeOffset, out freeLength);
                if (_appendToBuffer)
                {
                    if (spaceAvailable && (freeLength >= source.Length))
                    {
                        offsetChanged = false;
                        offset = freeOffset;
                        length = source.Length;
                    }
                    else
                    {
                        T[] bufferData = new T[length + source.Length];
                        subBufferData.Buffer.GetData(offset, bufferData, 0, length);
                        Array.Copy(source, 0, bufferData, length, source.Length);
                        source = bufferData;
                        if (spaceAvailable) length += freeLength; // Avoid memory fragmentation
                    }
                }
                else
                {
                    if (spaceAvailable) length += freeLength; // Avoid memory fragmentation
                }
            }

            int bufferOffset = AddToBuffer(subBufferData.Buffer, source, offset, length);
            newOffset = offsetChanged ? bufferOffset : subBufferData.Offset;
            lengthUsed = length;
        }

        /// <summary>
        /// Updates the shared vertex buffer with new data
        /// </summary>
        /// <param name="source">Array of vertices to update the buffer with</param>
        /// <param name="subBufferData">Informations about the buffer</param>
        /// <returns>Return the starting offset in the buffer where data had been set</returns>
        private int UpdateSharedVertex(VertexPositionNormalTexture[] source, SubMeshBufferData subBufferData)
        {
            int newOffset, length;
            UpdateSharedBuffer(source, subBufferData, GetSubVertexData, out newOffset, out length);
            if ((newOffset != subBufferData.Offset) & (length > 0))
                ShiftSubMeshVertexOffset(subBufferData.Offset, length);

            return newOffset;
        }

        /// <summary>
        /// Updates the shared index buffer with new data
        /// </summary>
        /// <param name="source">Array of indices to update the buffer with</param>
        /// <param name="subBufferData">Informations about the buffer</param>
        /// <returns>Return the starting offset in the buffer where data had been set</returns>
        private int UpdateSharedIndex(int[] source, SubMeshBufferData subBufferData)
        {
            int newOffset, length;
            UpdateSharedBuffer(source, subBufferData, GetSubIndexData, out newOffset, out length);
            if ((newOffset != subBufferData.Offset) & (length > 0))
                ShiftSubMeshIndexOffset(subBufferData.Offset, length);

            return newOffset;
        }

        /// <summary>
        /// Removes data from the shared vertex buffer
        /// </summary>
        /// <param name="offset">Starting offset</param>
        /// <param name="length">Number of element to remove</param>
        private void RemoveSharedVertex(int offset, int length)
        {
            VertexBufferObject buffer = EnsureVertexBuffer();
            ShiftBufferData<VertexPositionNormalTexture>(buffer, offset, length);
            ShiftSubMeshVertexOffset(offset, length);
        }

        /// <summary>
        /// Removes data from the shared index buffer
        /// </summary>
        /// <param name="offset">Starting offset</param>
        /// <param name="length">Number of element to remove</param>
        private void RemoveSharedIndex(int offset, int length)
        {
            IndexBufferObject buffer = EnsureIndexBuffer();
            ShiftBufferData<int>(buffer, offset, length);
            ShiftSubMeshIndexOffset(offset, length);
        }

        /// <summary>
        /// Finds the next starting offset of a free block of memory in a shared buffer
        /// </summary>
        /// <param name="getData">Method used to find informations from a submesh about the buffer</param>
        /// <returns>Returns the starting offset of a free block of memory</returns>
        private int FindNextBufferOffset(GetSubBufferData getData)
        {
            int nextOffset = 0;
            for (int i = 0; i < _subMeshes.Count; i++)
            {
                SubMeshBufferData subBufferData = getData(_subMeshes[i]);
                if (!subBufferData.Shared) continue;
                int subOffset = subBufferData.Offset;
                if (subOffset == -1) continue;
                if(subOffset >= nextOffset) nextOffset = subOffset + subBufferData.Count;
            }

            return nextOffset;
        }

        /// <summary>
        /// Checks for available space between used blocks of memory in a shared buffer
        /// </summary>
        /// <param name="offset">Starting offset of the first block of memory</param>
        /// <param name="length">Size of the first block of memory</param>
        /// <param name="getBufferData">Method used to find informations from a submesh about the buffer</param>
        /// <param name="startOffset">Starting offset of a free block of memory</param>
        /// <param name="freeLength">Size of the free block of memory</param>
        /// <returns>Returns true if a free block of memory is found otherwise false</returns>
        private bool CheckBufferFreeSpace(int offset, int length, GetSubBufferData getBufferData,
            out int startOffset, out int freeLength)
        {
            startOffset = freeLength = -1;
            int nearestOffset = int.MaxValue;
            for (int i = 0; i < _subMeshes.Count; i++)
            {
                SubMeshBufferData subBufferData = getBufferData(_subMeshes[i]);
                if (!subBufferData.Shared) continue;

                int subOffset = subBufferData.Offset;
                if(subOffset == -1) continue;
                if ((subOffset <= offset) || (subOffset >= nearestOffset)) continue;
                nearestOffset = subOffset;
            }

            int usedSpace = offset + length;
            if (usedSpace == nearestOffset) return false;

            startOffset = usedSpace;
            freeLength = nearestOffset - usedSpace;

            return true;
        }

        /// <summary>
        /// Shifts starting offset in the vertex buffer for each submesh using a shared buffer
        /// </summary>
        /// <param name="startOffset">Offset where the shift occurred</param>
        /// <param name="shiftLength">Size of the shift</param>
        private void ShiftSubMeshVertexOffset(int startOffset, int shiftLength)
        {
            if(shiftLength == 0) return;
            for (int i = 0; i < _subMeshes.Count; i++)
            {
                if(!_subMeshes[i].ShareVertexBuffer) continue;

                VertexData v = _subMeshes[i].VertexData;
                if(v.BufferCount == 0) continue;

                int currentOffset = v.GetVertexOffset(0);
                if(currentOffset > startOffset) v.SetVertexOffset((currentOffset - shiftLength), 0);
            }
        }

        /// <summary>
        /// Shifts starting offset in the index buffer for each submesh using a shared buffer
        /// </summary>
        /// <param name="startOffset">Offset where the shift occurred</param>
        /// <param name="shiftLength">Size of the shift</param>
        private void ShiftSubMeshIndexOffset(int startOffset, int shiftLength)
        {
            if(shiftLength == 0) return;
            for (int i = 0; i < _subMeshes.Count; i++)
            {
                if(!_subMeshes[i].ShareIndexBuffer) continue;

                IndexData idx = _subMeshes[i].IndexData;
                if(idx.IndexBuffer == null) continue;

                int currentOffset = idx.StartIndex;
                if (currentOffset > startOffset) idx.StartIndex = (currentOffset - shiftLength);
            }
        }

        /// <summary>
        /// Gets the shared vertex buffer if it exists otherwise it is created first
        /// </summary>
        /// <returns>Returns the shared vertex buffer</returns>
        private VertexBufferObject EnsureVertexBuffer()
        {
            if (_vertexData.BufferCount > 0) return _vertexData.GetBuffer(0);

            VertexBufferObject vbo = _bufferManager.CreateVertexBuffer(_currentVertexBufferType,
                typeof (VertexPositionNormalTexture), _estimatedVertexCount);
            _vertexData.SetBinding(vbo, 0, 0, 0);

            return vbo;
        }

        /// <summary>
        /// Gets the shared index buffer if it exists otherwise it is created first
        /// </summary>
        /// <returns>Returns the shared index buffer</returns>
        private IndexBufferObject EnsureIndexBuffer()
        {
            if (_indexData.IndexBuffer != null) return _indexData.IndexBuffer;

            IndexBufferObject ibo = _bufferManager.CreateIndexBuffer(_currentIndexBufferType, 
                IndexElementSize.ThirtyTwoBits, _estimatedIndexCount);
            _indexData.IndexBuffer = ibo;

            return ibo;
        }

        /// <summary>
        /// Creates a submesh
        /// </summary>
        /// <returns>Returns a new submesh</returns>
        public SubMesh CreateSubMesh()
        {
            SubMesh sub = new SubMesh();
            _subMeshes.Add(sub);

            return sub;
        }

        /// <summary>
        /// Creates a submesh with a name associated
        /// </summary>
        /// <param name="name">Name of the submesh</param>
        /// <returns>Returns a new submesh</returns>
        public SubMesh CreateSubMesh(string name)
        {
            if (_subMeshNamesMap.ContainsKey(name))
            {
                throw new Exception(string.Format("A submesh with the name {0} already exists", name));
            }
            SubMesh sub = CreateSubMesh();
            _subMeshNamesMap.Add(name, (ushort)(_subMeshes.Count - 1));

            return sub;
        }

        /// <summary>
        /// Removes a submesh
        /// </summary>
        /// <param name="index">Index of the submesh to remove</param>
        public void RemoveSubMesh(int index)
        {
            SubMesh sub = _subMeshes[index];
            _subMeshes.RemoveAt(index);
            UpdateNameMap(index);

            if (sub.ShareVertexBuffer)
            {
                SubMeshBufferData subVertexData = GetSubVertexData(sub);
                RemoveSharedVertex(subVertexData.Offset, subVertexData.Count);
            }
            if (sub.ShareIndexBuffer)
            {
                SubMeshBufferData subIndexData = GetSubIndexData(sub);
                RemoveSharedIndex(subIndexData.Offset, subIndexData.Count);
            }
            sub.Dispose();

            UpdateBounds();
            UpdateMeshInfo();
        }

        /// <summary>
        /// Removes a submesh with a specific name
        /// </summary>
        /// <param name="name">Name of the submesh to remove</param>
        public void RemoveSubMesh(string name)
        {
            int idx = GetSubMeshIndex(name);
            RemoveSubMesh(idx);
        }

        /// <summary>
        /// Updates the map of submesh name from a specific index
        /// </summary>
        /// <param name="index">Index used to update the map</param>
        private void UpdateNameMap(int index)
        {
            foreach (KeyValuePair<string, int> mapKvp in _subMeshNamesMap)
            {
                if (mapKvp.Value == index) _subMeshNamesMap.Remove(mapKvp.Key);
                else if (mapKvp.Value > index) _subMeshNamesMap[mapKvp.Key] = mapKvp.Value + 1;
            }
        }

        /// <summary>
        /// Gets a submesh
        /// </summary>
        /// <param name="index">Index of the submesh</param>
        /// <returns>Returns a submesh instance</returns>
        public SubMesh GetSubMesh(int index)
        {
            return _subMeshes[index];
        }

        /// <summary>
        /// Gets a submesh with a specific name
        /// </summary>
        /// <param name="name">Name of the submesh</param>
        /// <returns>Returns a submesh instance</returns>
        public SubMesh GetSubMesh(string name)
        {
            int idx = GetSubMeshIndex(name);
            
            return GetSubMesh(idx);
        }

        /// <summary>
        /// Renames a submesh
        /// </summary>
        /// <param name="name">Old name to replace</param>
        /// <param name="newName">New name to use</param>
        public void RenameSubMesh(string name, string newName)
        {
            int idx = GetSubMeshIndex(name);
            _subMeshNamesMap.Remove(name);
            _subMeshNamesMap.Add(newName, idx);
        }

        /// <summary>
        /// Gets the index of a named submesh
        /// </summary>
        /// <param name="name">Name of the submesh</param>
        /// <returns>Returns the index</returns>
        public int GetSubMeshIndex(string name)
        {
            int idx;
            if (!_subMeshNamesMap.TryGetValue(name, out idx))
                throw new Exception(string.Format("No submesh with a name {0} exists", name));

            return idx;
        }

        /// <summary>
        /// Updates mesh data, can be called by childs submesh to notify changes on them
        /// </summary>
        public void UpdateMeshInfo()
        {
            VerticesCount = 0;
            PrimitiveCount = 0;
            for (int i = 0; i < _subMeshes.Count; i++)
            {
                RenderingInfo renderingInfo = _subMeshes[i].RenderInfo;
                VerticesCount += renderingInfo.VertexCount;
                PrimitiveCount += renderingInfo.PrimitiveCount;
            }
        }

        /// <summary>
        /// Updates the mesh bounding volumes
        /// </summary>
        public void UpdateBounds()
        {
            _aabb = new BoundingBox();
            _boundingSphere = new BoundingSphere();
            for (int i = 0; i < _subMeshes.Count; i++)
            {
                BoundingBox subAabb = _subMeshes[i].AxisAlignedBoundingBox;
                BoundingBox.CreateMerged(ref _aabb, ref subAabb, out _aabb);
                BoundingSphere subSphere = _subMeshes[i].BoundingSphere;
                BoundingSphere.CreateMerged(ref _boundingSphere, ref subSphere, out _boundingSphere);
            }
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        /// <summary>
        /// Gets the number of vertices
        /// </summary>
        public int VerticesCount { get; internal set; }

        /// <summary>
        /// Gets the number of primitive
        /// </summary>
        public int PrimitiveCount { get; internal set; }

        /// <summary>
        /// Gets the number of submesh
        /// </summary>
        public int SubMeshCount
        {
            get { return _subMeshes.Count; }
        }

        /// <summary>
        /// Gets the list of sub mesh
        /// </summary>
        internal List<SubMesh> SubMeshes
        {
            get { return _subMeshes; }
        }

        /// <summary>
        /// Gets the aabb of the mesh
        /// </summary>
        public BoundingBox AxisAlignedBoundingBox
        {
            get { return _aabb; }
        }

        /// <summary>
        /// Gets the bounding sphere of the mesh
        /// </summary>
        public BoundingSphere BoundingSphere
        {
            get { return _boundingSphere; }
        }

        /// <summary>
        /// Gets the bones of the sub mesh
        /// </summary>
        public Matrix[] Bones { get; set; }

        /// <summary>
        /// Gets the vertex buffer of the mesh
        /// </summary>
        public VertexData VertexData
        {
            get { return _vertexData; }
        }

        /// <summary>
        /// Gets the index buffer of the mesh
        /// </summary>
        public IndexData IndexData
        {
            get { return _indexData; }
        }

        #endregion
    }
}