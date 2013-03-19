using System;
using System.Text;

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics
{
    /// <summary>
    /// Camera manager used to keep track of all the cameras
    /// </summary>
    public sealed class CameraManager
    {
        #region Fields

        private Dictionary<string, Camera> cameras = new Dictionary<string, Camera>();
        private Camera current = null;

        #endregion

        #region Constructors

        internal CameraManager()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a camera to the manager
        /// </summary>
        /// <param name="camera">The camera to add</param>
        public void AddCamera(Camera camera)
        {
            if (!this.cameras.Keys.Contains(camera.Name))
            {
                this.cameras.Add(camera.Name, camera);
            }
        }

        /// <summary>
        /// Remove a camera from the manager
        /// </summary>
        /// <param name="camera">Camera to remove</param>
        /// <returns>Return true if the camera was removed, else false</returns>
        public bool RemoveCamera(Camera camera)
        {
            if (this.current != null)
            {
                if (string.Equals(this.current.Name, camera.Name))
                    this.current = null;

                if (this.cameras.Remove(camera.Name))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Set the current camera to the one specified
        /// </summary>
        /// <param name="camera">Camera to set as current</param>
        public void UseCamera(Camera camera)
        {
            if (!this.cameras.Keys.Contains(camera.Name))
                this.AddCamera(camera);

            this.InternalUseCamera(camera);
        }

        /// <summary>
        /// Set the current camera to the one specified
        /// </summary>
        /// <param name="cameraName">Name of the camera to set as current</param>
        public void UseCamera(string cameraName)
        {
            if (!this.cameras.Keys.Contains(cameraName))
                throw new Exception(string.Format("No camera with name {0} exist.", cameraName));

            this.InternalUseCamera(this.cameras[cameraName]);
        }

        /// <summary>
        /// Set the current camera
        /// </summary>
        /// <param name="camera">Camera to set as current</param>
        private void InternalUseCamera(Camera camera)
        {
            this.current = null;
            this.current = camera;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the current camera
        /// </summary>
        public Camera Current
        {
            get { return this.current; }
        }

        #endregion
    }
}
