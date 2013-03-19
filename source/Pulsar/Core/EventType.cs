using System;

using System.Collections.Generic;

namespace Pulsar.Core
{
    /// <summary>
    /// Represent an event for the event system
    /// </summary>
    public sealed class EventType
    {
        #region Fields

        private static Dictionary<string, EventType> availableEvent = 
            new Dictionary<string, EventType>();

        /// <summary>
        /// Name of the event
        /// </summary>
        public readonly string eventName;

        /// <summary>
        /// Hash of the event generated from the name
        /// </summary>
        public readonly int eventHash;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of EvenType class
        /// </summary>
        /// <param name="name">Name of the event</param>
        private EventType(string name)
        {
            this.eventName = name;
            this.eventHash = name.GetHashCode();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create or get an event type instance
        /// </summary>
        /// <param name="name">Name of the event</param>
        /// <returns>Return an instance of EventType</returns>
        public static EventType CreateEvent(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Event name can't be null or empty");
            }

            EventType ev;
            EventType.availableEvent.TryGetValue(name, out ev);
            if (ev == null)
            {
                ev = new EventType(name);
                EventType.availableEvent.Add(name, ev);
            }

            return ev;
        }

        #endregion
    }
}
