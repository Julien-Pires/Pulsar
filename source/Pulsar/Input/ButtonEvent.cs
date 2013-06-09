using System;

namespace Pulsar.Input
{
    public struct ButtonEvent
    {
        #region Fields

        public readonly AbstractButton Button;

        public readonly short Index;

        public readonly ButtonEventType Event;

        #endregion

        #region Constructors

        public ButtonEvent(AbstractButton button, ButtonEventType eventType)
        {
            this.Button = button;
            this.Index = -1;
            this.Event = eventType;
        }

        public ButtonEvent(AbstractButton button, ButtonEventType eventType, short index)
        {
            this.Button = button;
            this.Index = index;
            this.Event = eventType;
        }

        #endregion
    }
}
