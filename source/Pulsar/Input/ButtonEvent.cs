namespace Pulsar.Input
{
    /// <summary>
    /// Contains data about a button that have been triggered (Pressed or Released)
    /// </summary>
    public struct ButtonEvent
    {
        #region Fields

        /// <summary>
        /// Triggered button
        /// </summary>
        public readonly AbstractButton Button;

        /// <summary>
        /// Index of the device associated to the button
        /// </summary>
        public readonly short Index;

        /// <summary>
        /// Type of event
        /// </summary>
        public readonly ButtonEventType Event;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ButtonEvent struct
        /// </summary>
        /// <param name="button">AbstractButton instance</param>
        /// <param name="eventType">Type of event</param>
        public ButtonEvent(AbstractButton button, ButtonEventType eventType)
        {
            Button = button;
            Index = -1;
            Event = eventType;
        }

        /// <summary>
        /// Constructor of ButtonEvent struct
        /// </summary>
        /// <param name="button">AbstractButton instance</param>
        /// <param name="eventType">Type of event</param>
        /// <param name="index">Index of the device</param>
        public ButtonEvent(AbstractButton button, ButtonEventType eventType, short index)
        {
            Button = button;
            Index = index;
            Event = eventType;
        }

        #endregion
    }
}
