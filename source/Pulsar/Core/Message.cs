using System;

using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Pulsar.Core
{
    /// <summary>
    /// Base class for message used in event system
    /// A message is linked to a specific event, the mediator reads the event on the message
    /// in order to determine to which listener the message is send
    /// </summary>
    public class Message
    {
        #region Fields

        /// <summary>
        /// Event used by this message
        /// </summary>
        public readonly EventType Event;
        
        protected object sender;
        protected GameTime time;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Message class
        /// </summary>
        public Message()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Recycle the message to reuse it
        /// </summary>
        /// <param name="s">Sender of the message</param>
        /// <param name="ti">Time at wich the message was sent</param>
        public virtual void Recycle(object s, GameTime ti)
        {
            this.sender = s;
            this.time = ti;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the sender of the message
        /// </summary>
        public object Sender
        {
            get { return this.sender; }
        }

        /// <summary>
        /// Get the time at wich the message is send
        /// </summary>
        public GameTime Time
        {
            get { return this.time; }
        }

        #endregion
    }
}
