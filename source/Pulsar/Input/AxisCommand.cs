using System;

namespace Pulsar.Input
{
    public enum AxisEventType : byte
    {
        IsInactive,
        IsActive
    }

    internal sealed class AxisCommand : IInputCommand
    {
        #region Fields

        private AxisEventType axisEvent;
        private Axis axis;
        private CheckCommandState checkMethod;

        #endregion

        #region Constructors

        internal AxisCommand(Axis axis, AxisEventType axisEvent)
        {
            this.axis = axis;
            this.axisEvent = axisEvent;
            this.AssignCheckMethod();
        }

        #endregion

        #region Methods

        private void AssignCheckMethod()
        {
            switch (this.axisEvent)
            {
                case AxisEventType.IsInactive: this.checkMethod = this.IsInactive;
                    break;
                case AxisEventType.IsActive: this.checkMethod = this.IsActive;
                    break;
                default: throw new Exception("Invalid command event code provided");
                    break;
            }
        }

        private bool IsInactive()
        {
            return this.axis.Value == 0.0f;
        }

        private bool IsActive()
        {
            return this.axis.Value > 0.0f;
        }

        #endregion

        #region Properties

        public bool IsTriggered
        {
            get { return this.checkMethod(); }
        }

        #endregion
    }
}
