using System.Collections.Generic;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Camera manager used to keep track of all the cameras
    /// </summary>
    public sealed class CameraManager
    {
        #region Fields

        private readonly Dictionary<string, Camera> _cameras = new Dictionary<string, Camera>();
        private readonly SceneTree _owner;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of CameraManager class
        /// </summary>
        internal CameraManager(SceneTree owner)
        {
            _owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a camera to the manager
        /// </summary>
        /// <param name="name">Name of the camera</param>
        public Camera CreateCamera(string name)
        {
            Camera cam;
            if (_cameras.TryGetValue(name, out cam)) return cam;

            cam = new Camera(name, _owner);
            _cameras.Add(name, cam);

            return cam;
        }

        /// <summary>
        /// Remove a camera
        /// </summary>
        /// <param name="camera">Camera to remove</param>
        /// <returns>Return true if the camera was removed, otherwise false</returns>
        public bool RemoveCamera(Camera camera)
        {
            return _cameras.Remove(camera.Name);
        }

        /// <summary>
        /// Remove a camera with its name
        /// </summary>
        /// <param name="name">Name of the camera</param>
        /// <returns>Return true if the camera was removed, otherwise false</returns>
        public bool RemoveCamera(string name)
        {
            return _cameras.Remove(name);
        }

        #endregion
    }
}
