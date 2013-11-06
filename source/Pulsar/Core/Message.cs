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

        private readonly EventType _eventType;

        private readonly object _payload;

        private long _timestamp;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Message struct
        /// </summary>
        /// <param name="eventType">Type of event</param>
        public Message(EventType eventType)
        {
            _eventType = eventType;
            _payload = null;
            _timestamp = 0;
        }

        /// <summary>
        /// Constructor of Message struct
        /// </summary>
        /// <param name="eventType">Type of event</param>
        /// <param name="payload">Payload data</param>
        public Message(EventType eventType, object payload)
        {
            _eventType = eventType;
            _payload = payload;
            _timestamp = 0;
            
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the type of event
        /// </summary>
        public EventType Event
        {
            get { return _eventType; }
        }

        /// <summary>
        /// Get the payload data
        /// </summary>
        public object Payload
        {
            get { return _payload; }
        }

        /// <summary>
        /// Get the time at which the message was sent
        /// </summary>
        public long Timestamp
        {
            get { return _timestamp; }
            internal set { _timestamp = value; }
        }

        #endregion
    }
}
