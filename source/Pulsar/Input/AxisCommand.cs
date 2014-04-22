using System;

namespace Pulsar.Input
{
    /// <summary>
    /// Checks the state of an axis for a specific event
    /// </summary>
    internal sealed class AxisCommand : IInputCommand
    {
        #region Fields

        private readonly AxisEventType _axisEvent;
        private readonly Axis _axis;
        private Func<bool> _checkMethod;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of AxisCommand
        /// </summary>
        /// <param name="axis">Axis instance</param>
        /// <param name="axisEvent">Event to check for</param>
        internal AxisCommand(Axis axis, AxisEventType axisEvent)
        {
            _axis = axis;
            _axisEvent = axisEvent;
            AssignCheckMethod();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Assignes a method that check the state of the axis
        /// </summary>
        private void AssignCheckMethod()
        {
            switch (_axisEvent)
            {
                case AxisEventType.IsInactive: 
                    _checkMethod = IsInactive;
                    break;
                case AxisEventType.IsActive: 
                    _checkMethod = IsActive;
                    break;
                default: 
                    throw new Exception("Invalid command event code provided");
            }
        }

        /// <summary>
        /// Checks if an axis is inactive
        /// </summary>
        /// <returns>Return true if the axis is inactive othterwise false</returns>
        private bool IsInactive()
        {
            return !(_axis.Value > 0.0f);
        }

        /// <summary>
        /// Checks if an axis is active
        /// </summary>
        /// <returns>Return true if the axis is active otherwise false</returns>
        private bool IsActive()
        {
            return _axis.Value > 0.0f;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that indicates if the command is triggered
        /// </summary>
        public bool IsTriggered
        {
            get { return _checkMethod(); }
        }

        #endregion
    }
}
