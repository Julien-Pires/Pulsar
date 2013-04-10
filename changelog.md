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

* Added resources management
* Added graphic system
* Added component system
