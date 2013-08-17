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

        private long _timestamp;

        private object sender;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Message struct
        /// </summary>
        /// <param name="eventType">Type of event</param>
        /// <param name="sender">Sender of the message</param>
        public Message(EventType eventType, object sender)
        {
            this.eventType = eventType;
            this.sender = sender;
            payload = null;
            _timestamp = 0;
        }

        /// <summary>
        /// Constructor of Message struct
        /// </summary>
        /// <param name="eventType">Type of event</param>
        /// <param name="sender">Sender of the message</param>
        /// <param name="payload">Payload data</param>
        public Message(EventType eventType, object sender, object payload)
        {
            this.eventType = eventType;
            this.sender = sender;
            this.payload = payload;
            _timestamp = 0;
            
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the type of event
        /// </summary>
        public EventType Event
        {
            get { return this.eventType; }
        }

        /// <summary>
        /// Get the payload data
        /// </summary>
        public object Payload
        {
            get { return this.payload; }
        }

        /// <summary>
        /// Get the time at which the message was sent
        /// </summary>
        public long Timestamp
        {
            get { return _timestamp; }
            internal set { _timestamp = value; }
        }

        /// <summary>
        /// Get the sender of the message
        /// </summary>
        public object Sender
        {
            get { return this.sender; }
        }

        #endregion
    }
}
