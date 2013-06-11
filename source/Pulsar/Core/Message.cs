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

        /// <summary>
        /// Constructor of Message struct
        /// </summary>
        /// <param name="eventType">Type of event</param>
        /// <param name="time">Time at which the message was sent</param>
        /// <param name="sender">Sender of the message</param>
        public Message(EventType eventType, GameTime time, object sender)
        {
            this.eventType = eventType;
            this.sender = sender;
            this.time = time;
            this.payload = null;
        }

        /// <summary>
        /// Constructor of Message struct
        /// </summary>
        /// <param name="eventType">Type of event</param>
        /// <param name="time">Time at which the message was sent</param>
        /// <param name="sender">Sender of the message</param>
        /// <param name="payload">Payload data</param>
        public Message(EventType eventType, GameTime time, object sender, object payload)
        {
            this.eventType = eventType;
            this.sender = sender;
            this.time = time;
            this.payload = payload;
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
        public GameTime Time
        {
            get { return this.time; }
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
