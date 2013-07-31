using System.Collections.Generic;

namespace Pulsar.Graphics.SceneGraph
{
    /// <summary>
    /// Camera manager used to keep track of all the cameras
    /// </summary>
    public sealed class CameraManager
    {
        #region Fields

        private Dictionary<string, Camera> cameras = new Dictionary<string, Camera>();
        private SceneTree owner;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of CameraManager class
        /// </summary>
        internal CameraManager(SceneTree owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a camera to the manager
        /// </summary>
        /// <param name="camera">The camera to add</param>
        public Camera CreateCamera(string name)
        {
            Camera cam;
            if (this.cameras.TryGetValue(name, out cam)) return cam;

            cam = new Camera(name, this.owner);
            this.cameras.Add(name, cam);

            return cam;
        }

        /// <summary>
        /// Remove a camera from the manager
        /// </summary>
        /// <param name="camera">Camera to remove</param>
        /// <returns>Return true if the camera was removed, else false</returns>
        public bool RemoveCamera(Camera camera)
        {
            return this.cameras.Remove(camera.Name);
        }

        public bool RemoveCamera(string name)
        {
            return this.cameras.Remove(name);
        }

        #endregion
    }
}
