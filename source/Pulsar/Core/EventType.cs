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

        private static readonly Dictionary<string, EventType> AvailableEvent = new Dictionary<string, EventType>();

        /// <summary>
        /// Name of the event
        /// </summary>
        public readonly string EventName;

        /// <summary>
        /// Hash of the event generated from the name
        /// </summary>
        public readonly int EventHash;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of EvenType class
        /// </summary>
        /// <param name="name">Name of the event</param>
        private EventType(string name)
        {
            EventName = name;
            EventHash = name.GetHashCode();
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
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            EventType ev;
            AvailableEvent.TryGetValue(name, out ev);
            if (ev != null) return ev;

            ev = new EventType(name);
            AvailableEvent.Add(name, ev);

            return ev;
        }

        #endregion
    }
}
