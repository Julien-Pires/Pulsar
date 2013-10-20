## 0.3.3 (October 19, 2013)

New feature: Manual mesh

- **Core:**
  - Message struct use a timestamp instead of a GameTime instance
  - Services providers implements IDisposable
- **Extension:**
  - GetValues in EnumExtension class works now for both Windows and Xbox 360
  - ArrayExtension provides new methods to manipulate arrays
- **Math:**
  - GetAxes signature has changed in QuaternionHelpers class
- **Graphics:**
  - **New:** Mesh can be generated manually with helper methods (Begin/Update/End)
      - Begin/Update method create a new submesh or start editing an existing one
      - End validates and generates the submesh and all datas necessary to make it work
      - Position, Normal, Texture, Index, Triangle, Quad can be used to create new data for the SubMesh currently edited,
      those methods should be call after Begin/Update
  - **New:** BufferObject can now resize automatically, if a set of data is too large the buffer object will allocate a new buffer 
  with enough space and copy data from the old buffer to the new one, the old one will be destroyed
  - Mesh class has two new methods: UpdateMeshInfo to update Mesh informations (PrimitiveCount, ...) and
  UpdateBounds to update Mesh bounding volumes, those methods could be called when Mesh are not loaded from a file
  or generated with helper methods
  - BufferObject class provides new methods to manipulate data in a buffer
  - VertexBufferObject and IndexBufferObject class provides an event to listen for when a new block of memory has been allocated
  - VertexData and IndexData properties on Mesh and SubMesh class are now read-only
  - SubMesh class implements IDisposable
  - SubMesh uses its own VertexData and IndexData instance but VBO and IBO can be shared with parent Mesh
  - RenderingInfo class has a new method ComputePrimitiveCount to compute the number of primitive used by this instance
  - BoundingVolume class provides new methods to do intersection test
  - Camera class has new methods using ref/out parameters (Translate, Scale, ...)
  - Viewport property in Camera class has been renamed CurrentViewport
  - Camera class can override Viewport aspect ratio when rendering, AutomaticAspectRatio property tell the camera
  to use its own AspectRatio instead of Viewport one
  - Node class has new methods using ref/out parameters (Translate, Scale, ...)
  - Fields in Node class are now private
  - Node class has new properties to get specifics Matrix (ScaleTransform, OrientationTransform, ...)
  - GraphicsEngine class has a method to destroy a SceneGraph
  - NotifyCurrentCamera method in IMovable interface has been renamed to CheckVisibilityWithCamera
- **Input:**
  - **New:** InputEvent can now accepts more than one listeners, a boolean is passed as an out parameters to stop calling next methods
  - **New:** Player index and gamepad index can be different with the PlayerIndex class
  - InputAction class has been renamed InputEvent
  - PlayerInput class has been renamed Player
  - ButtonType enum and InputDevice enum are byte type
  - GamePad class has a static and a non-static methods to release all listeners of events on them
- **Miscellaneous:**
  - Some bug fixes

## 0.3.2 (August 11, 2013)

New feature: Window management

- **Graphics:**
  - RenderTarget class has been added. It's a base class for all classes that need render target rendering. Currently only Window class inherits it
  - RenderTarget can be split into multiple viewports with the new Viewport class
  - ViewportPosition is a new enum to allow creation of viewport with predefined dimension and position
  - Window class has been added. It allows to manage window parameters and create split-screen
  - A camera should be associated to a viewport of the Window class in order to render the scene associated to it
  - GraphicsEngine has two new properties: Window and FrameStatistics
  - Camera are now strongly linked to a scene graph and should be created with the CameraManager
- **Miscellaneous:**
  - Demo project has been removed

## 0.3.1 (June 11, 2013)

Bugfixe release

- **Asset:**
  - SubMesh can be rename with RenameSubMesh method
  - Mesh no longer expose VertexBuffer and IndexBuffer
  - Mesh and SubMesh can accept a VertexData instance for vertices data
  - Mesh and SubMesh can accept an IndexData instance for indices data
  - SubMesh has two new properties
  - ShareVertexBuffer : if set to true submesh will use parent VertexData otherwise it uses its own data
  - UseIndexes: if set to true submesh will use indices for draw call otherwise renderer will use non indexed draw call
- **Core:**
  - Mediator class is no longer a singleton
  - Message class has been changed to a struct
  - Message struct has a new property: Payload of type object
- **Graphics:**
  - New class available for buffer management
    - VertexData, VertexBufferObject
    - IndexData, IndexBufferObject
  - VertexBufferObject is a wrapper for a VertexBuffer or DynamicVertexBuffer, this class should be used instead of XNA buffer
  - IndexBufferObject is a wrapper for an IndexBuffer or DynamicIndexBuffer, this class should be used instead of XNA buffer
  - VertexData allows to bind multiple vertex buffer, this could be used to send multiple buffer when SetBuffer is called (eg: instancing, ...)
  - IndexData contains an IndexBufferObject
  - New class BufferManager allows to create buffer object and is available in GraphicsEngine
  - Renderer has now a method to draw non indexed mesh
  - Root class has been replaced by GraphicsEngine
  - GraphicsEngine is available via Services of GameApplication class
  - New class FrameInfo gathers information about the last frame (draw call count, vertices count, ...)
- **Input:**
  - DigitalButton and AnalogButton structs have been replaced by AbstractButton struct
  - InputDevice enum is now a flag enum and add a new value : AllGamePad
  - Mouse, Keyboard and GamePad classes have a new method AnyKeyPressed
  - VirtualInput also has a AnyKeyPressed method and a new property AssociatedDevice which determine which device to listen for pressed key
  - InputManager can remove all players at once with RemoveAllPlayers method
  - CreatePlayer method of InputManager returns the created instance
  - CreateContext method of PlayerInput returns the created instance

## 0.3.0 (May 17, 2013)

New feature: Input management

- **Input:**
  - Virtual input system. This virtual input allow to map a button (digital or analog) to a virtual one.
    There are two kind of virtual button: Button and Axis. Button represents a simple button and Axis represents
    an axis with his value between a range of value positive and negative
  - Two way to check for virtual input, by calling GetAxis or GetButton methods of VirtualInput class to check by 
    yourself the state of these input or use InputAction class to create an action. An action is composed of one or 
    more command and when all commands are triggered, action invoke a delegate
  - Context for each players. A context is a way to link a VirtualInput to a specific name
  - Three new classes to manage input peripheric: Mouse, Keyboard and GamePad. Those classes come on the top of 
    the existing one in the XNA Framework, they keep tracks of the current state and the previous
  - GamePad class provides two events to notify when a GamePad is connected or disconnected
  - InputService is available via Services property when GameApplication class is used
- **Core:**
  - Singleton class has been moved to Core namespace
- **Miscellaneous:**
  - MIT licence added

## 0.2.6 (April 25, 2013)

- **Graphics:**
  - Graph namespace have been renamed to SceneGraph
  - SceneGraph class has been renamed to SceneTree
  - GeometryBatch class has been renamed to InstanceBatch
  - GeometryBatchManager class has been renamed to InstanceBatchManager
  - Finding a specific instance of InstanceBatch has been improved
  - InstanceBatch and InstanceBatchManager have been moved to Graphics.Rendering namespace
  - Processing instanced entity occurred now during the rendering stage and not at the queue processing as before
  - RenderingInfo constructor is now internal
  - Added a function to find a SubEntity instance in an entity
  - ID for IRenderable instance are now generated by RenderingInfo instance. This ensures integrity in the generation of ID
  - MeshBoundingBox reuse existing vertices instead of recreated them. Only the Position field is updated
- **Assets**
  - RenderingInfo of a SubMesh can be modified using SetRenderingInfo method
  - Added a function to find a SubMesh instance in a mesh
  - Material can be modified on SubMesh
  - SubMesh instance in a mesh can have an unique name
  - SubMesh can now be removed of mesh
  - Mesh and SubMesh class are now in separated file
- **Component:**
  - ComponentHandler can be associated to one or more component type. With this mecanism ComponentHandler are only notified for concerned component
  - Owner property of ComponentHandler can only be modified by ComponentHandlerSystem class
- **Miscellaneous:**
  - Demo project is now separated from Pulsar library and is located in demo folder with its own VS solution
  - Source code has been commented

## 0.2.5 (April 9, 2013)

Bugfix release

- **New:**
  - Added Xbox 360 version of Pulsar
- **Graphics:**
  - Added a virtual remove function on the Node class to implement specific remove task for specific node implementation
  - Root class doesn't longer need a ContentManager instance in his constructor
  - Remove BaseEffect class (use Shader class instead)
  - IsVisible property was replaced by IsRendered in IMovable interface to avoid misunderstanding with Visible property
  - Added a new class BoundingVolume to manage bounding shapes and their updates
  - Fixed a bug in which AABB doesn't fit meshes entirely
  - MeshBoundingBox is now displayed with full line
- **Asset:**
  - Added a function to add submesh in a mesh
  - Mesh informations are now updated when a submesh is added
  - Add a vertex buffer and an index buffer when an empty mesh is created by MeshManager. Manual mesh should use those buffers for submeshes.
  - Moved CreateMaterial function from MeshManager to MaterialManager
  - Added parameters to create more personnalized checkerboard texture in CreateMissingTexture functions
  - Removed IsSolid property in Material class. Now to determine if a material is solid or transparent use IsTransparent property
- **Component:**
  - Removed owner parameter of type GameObjectManager in the constructor of GameObject class
  - Fixed a bug in which a game object with components added to a GameObjectManager doesn't notify component handler of the existing components
- **Miscellaneous:**
  - Removed out modifiers
  - Removed default parameters

## 0.2 (March 27, 2013)

  - Added resources management
  - Added graphic system
  - Added component system
