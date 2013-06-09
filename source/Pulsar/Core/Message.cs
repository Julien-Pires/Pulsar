using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Core
{
    /// <summary>
    /// Struct for message used in event system
    /// A message is linked to a specific event, the mediator reads the event on the message
    /// in order to determine to which listener the message is send
    /// </summary>
    public struct Message
    {
        #region Fields

        private EventType eventType;

        private object payload;

        private GameTime time;

        private object sender;

        #endregion

        #region Constructors

        public Message(EventType eventType, GameTime time, object sender)
        {
            this.eventType = eventType;
            this.sender = sender;
            this.time = time;
            this.payload = null;
        }

        public Message(EventType eventType, GameTime time, object sender, object payload)
        {
            this.eventType = eventType;
            this.sender = sender;
            this.time = time;
            this.payload = payload;
        }

        #endregion

        #region Properties

        public EventType Event
        {
            get { return this.eventType; }
        }

        public object Payload
        {
            get { return this.payload; }
        }

        public GameTime Time
        {
            get { return this.time; }
        }

        public object Sender
        {
            get { return this.sender; }
        }

        #endregion
    }
}
