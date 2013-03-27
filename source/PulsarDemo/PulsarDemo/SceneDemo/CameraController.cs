using System;
using System.Text;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Pulsar.Graphics;
using Pulsar.Graphics.Graph;
using Pulsar.Graphics.Rendering;

namespace PulsarDemo.SceneDemo
{
    public enum CameraStyle { FREE, ORBIT };

    public sealed class CameraController
    {
        #region Fields

        private readonly float speed = 20.0f;
        private CameraStyle camStyle = CameraStyle.FREE;
        private Camera cam = null;
        private bool goForward = false;
        private bool goBack = false;
        private bool goRight = false;
        private bool goLeft = false;

        #endregion

        #region Constructors

        public CameraController(Camera cam)
        {
            this.cam = cam;
        }

        #endregion

        #region Methods

        public void Tick(GameTime time)
        {
            this.UpdateKey();
            this.ComputeFrame();
        }

        private void ComputeFrame()
        {
            this.ProcessMovement();
            this.ProcessMouse();   
        }

        private void ProcessMovement()
        {
            if (this.camStyle == CameraStyle.FREE)
            {
                Vector3 accel = Vector3.Zero;
                if (this.goForward) accel += this.cam.Direction;
                if (this.goBack) accel -= this.cam.Direction;
                if (this.goRight) accel += this.cam.Right;
                if (this.goLeft) accel -= this.cam.Right;

                this.cam.Translate(accel);
            }
        }

        private void ProcessMouse()
        {
            if (this.camStyle == CameraStyle.FREE)
            {
                Vector2 diff = Input.Instance.GetMousePositionDiff();
                this.cam.Yaw(MathHelper.ToRadians(diff.X) * 0.15f);
                this.cam.Pitch(MathHelper.ToRadians(diff.Y) * 0.15f);
            }

            //Input.Instance.ResetMouse();
        }

        private void UpdateKey()
        {
            if (this.camStyle == CameraStyle.FREE)
            {
                Input inHandler = Input.Instance;

                if ((inHandler.IsKeyDown(Keys.Up)) || (inHandler.IsKeyDown(Keys.Z))) this.goForward = true;
                else if ((inHandler.IsKeyUp(Keys.Up)) || (inHandler.IsKeyUp(Keys.Z))) this.goForward = false;

                if ((inHandler.IsKeyDown(Keys.Down)) || (inHandler.IsKeyDown(Keys.S))) this.goBack = true;
                else if ((inHandler.IsKeyUp(Keys.Up)) || (inHandler.IsKeyUp(Keys.S))) this.goBack = false;

                if ((inHandler.IsKeyDown(Keys.Right)) || (inHandler.IsKeyDown(Keys.D))) this.goRight = true;
                else if ((inHandler.IsKeyUp(Keys.Right)) || (inHandler.IsKeyUp(Keys.D))) this.goRight = false;

                if ((inHandler.IsKeyDown(Keys.Left)) || (inHandler.IsKeyDown(Keys.Q))) this.goLeft = true;
                else if ((inHandler.IsKeyUp(Keys.Left)) || (inHandler.IsKeyUp(Keys.Q))) this.goLeft = false;
            }
        }

        #endregion
    }
}
