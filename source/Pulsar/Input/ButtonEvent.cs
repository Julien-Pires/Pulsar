namespace Pulsar.Input
{
    /// <summary>
    /// Contains data about a button that have been triggered
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

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of ButtonEvent struct
        /// </summary>
        /// <param name="button">AbstractButton instance</param>
        public ButtonEvent(AbstractButton button)
        {
            Button = button;
            Index = -1;
        }

        /// <summary>
        /// Constructor of ButtonEvent struct
        /// </summary>
        /// <param name="button">AbstractButton instance</param>
        /// <param name="index">Index of the device</param>
        public ButtonEvent(AbstractButton button, short index)
        {
            Button = button;
            Index = index;
        }

        #endregion
    }
}
