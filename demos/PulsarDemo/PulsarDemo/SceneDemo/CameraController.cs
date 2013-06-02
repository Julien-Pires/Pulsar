using System;
using System.Text;

using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Pulsar.Input;
using Pulsar.Graphics;
using Pulsar.Graphics.SceneGraph;
using Pulsar.Graphics.Rendering;

namespace PulsarDemo.SceneDemo
{
    public enum CameraStyle { FREE, ORBIT };

    public sealed class CameraController
    {
        #region Fields

        private const string solarController = "SolarController";
        private const string forwardName = "Forward";
        private const string backName = "Back";
        private const string rightName = "Right";
        private const string leftName = "Left";
        private const string horizontalName = "Horizontal";
        private const string verticalName = "Vertical";

        private readonly float speed = 20.0f;
        private readonly float lookSpeed = 75.0f;
        private CameraStyle camStyle = CameraStyle.FREE;
        private InputManager input;
        private PlayerInput player;
        private VirtualInput currentInput;
        private Camera cam;
        private bool goForward;
        private bool goBack;
        private bool goRight;
        private bool goLeft;

        #endregion

        #region Constructors

        public CameraController(InputManager input)
        {
            this.input = input;
            this.input.CreatePlayer(0);
            this.player = this.input.GetPlayer(0);
            this.player.CreateContext(CameraController.solarController);
            this.player.SetCurrentContext(CameraController.solarController);
            this.currentInput = this.player.GetContext(CameraController.solarController);
            this.CreateInputMapping();
        }

        #endregion

        #region Methods

        public void CreateInputMapping()
        {
            Button forward = new Button() { Name = CameraController.forwardName };
            Button back = new Button() { Name = CameraController.backName };
            Button right = new Button() { Name = CameraController.rightName };
            Button left = new Button() { Name = CameraController.leftName };
            Axis horizontal = new Axis() { Name = CameraController.horizontalName, Inverse = true };
            Axis vertical = new Axis() { Name = CameraController.verticalName, Inverse = true };
            
#if WINDOWS
            forward.AddButton(new DigitalButton(Keys.Z), 0);
            forward.AddButton(new DigitalButton(Keys.Up), 1);
            back.AddButton(new DigitalButton(Keys.S), 0);
            back.AddButton(new DigitalButton(Keys.Down), 1);
            right.AddButton(new DigitalButton(Keys.D), 0);
            right.AddButton(new DigitalButton(Keys.Right), 1);
            left.AddButton(new DigitalButton(Keys.Q), 0);
            left.AddButton(new DigitalButton(Keys.Left), 1);
            horizontal.AddButton(new AnalogButton(MouseAnalogButtons.MouseX), 0);
            vertical.AddButton(new AnalogButton(MouseAnalogButtons.MouseY), 0);
#endif
            forward.AddButton(new DigitalButton(Buttons.LeftThumbstickUp), 2);
            back.AddButton(new DigitalButton(Buttons.LeftThumbstickDown), 2);
            right.AddButton(new DigitalButton(Buttons.LeftThumbstickRight), 2);
            left.AddButton(new DigitalButton(Buttons.LeftThumbstickLeft), 2);
            horizontal.AddButton(new AnalogButton(GamePadAnalogButtons.RightThumbStickX), 1);
            vertical.AddButton(new AnalogButton(GamePadAnalogButtons.RightThumbStickY), 1);

            this.currentInput.AddButton(forward);
            this.currentInput.AddButton(back);
            this.currentInput.AddButton(right);
            this.currentInput.AddButton(left);
            this.currentInput.AddAxis(horizontal);
            this.currentInput.AddAxis(vertical);
        }

        public void Tick(GameTime time)
        {
            this.UpdateKey();
            this.ComputeFrame(time);
        }

        private void ComputeFrame(GameTime time)
        {
            this.ProcessMovement();
            this.ProcessMouse(time);   
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

        private void ProcessMouse(GameTime time)
        {
            if (this.camStyle == CameraStyle.FREE)
            {
                float xValue = this.currentInput.GetAxis(CameraController.horizontalName).Value;
                float yValue = this.currentInput.GetAxis(CameraController.verticalName).Value;
                float yaw = MathHelper.Clamp(xValue, -1.0f, 1.0f) * (this.lookSpeed * (float)time.ElapsedGameTime.TotalSeconds);
                float pitch = MathHelper.Clamp(yValue, -1.0f, 1.0f) * (this.lookSpeed * (float)time.ElapsedGameTime.TotalSeconds);
                this.cam.Yaw(MathHelper.ToRadians(yaw));
                this.cam.Pitch(MathHelper.ToRadians(pitch));
            }
        }

        private void UpdateKey()
        {
            if (this.camStyle == CameraStyle.FREE)
            {
                if (this.currentInput.GetButton(CameraController.forwardName).IsDown) this.goForward = true;
                else if (this.currentInput.GetButton(CameraController.forwardName).IsUp) this.goForward = false;

                if (this.currentInput.GetButton(CameraController.backName).IsDown) this.goBack = true;
                else if (this.currentInput.GetButton(CameraController.backName).IsUp) this.goBack = false;

                if (this.currentInput.GetButton(CameraController.rightName).IsDown) this.goRight = true;
                else if (this.currentInput.GetButton(CameraController.rightName).IsUp) this.goRight = false;

                if (this.currentInput.GetButton(CameraController.leftName).IsDown) this.goLeft = true;
                else if (this.currentInput.GetButton(CameraController.leftName).IsUp) this.goLeft = false;
            }
        }

        #endregion

        #region Properties

        public Camera Camera
        {
            get { return this.cam; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Camera cannot be null");
                }
                this.cam = value;
            }
        }

        #endregion
    }
}
