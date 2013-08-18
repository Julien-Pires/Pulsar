using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Pulsar.Core
{
    /// <summary>
    /// Receive and send message between sender and listener
    /// A message is made of an EventType object, the mediator receives messages
    /// and dispatches them according to the event object find in each messages
    /// Mediator can send message in real time (trigger) or add them to a queue
    /// which is processed later
    /// </summary>
    public sealed class Mediator
    {
        #region Fields

        private const long InfiniteTime = long.MaxValue;
        private const int CountQueue = 2;
        
        private int _activeQueue;
        private readonly Queue<Message>[] _queueList = new Queue<Message>[CountQueue];
        private readonly Dictionary<int, List<IEventHandler>> _eventListenersMap =
            new Dictionary<int, List<IEventHandler>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Mediator class
        /// </summary>
        public Mediator()
        {
            for (int i = 0; i < CountQueue; i++)
            {
                _queueList[i] = new Queue<Message>();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get list of listeners for a specific event
        /// </summary>
        /// <param name="evType">Event type</param>
        /// <returns>Returns a list of listeners if found otherwise null</returns>
        private List<IEventHandler> GetListeners(EventType evType)
        {
            List<IEventHandler> listeners;
            _eventListenersMap.TryGetValue(evType.eventHash, out listeners);

            return listeners;
        }

        /// <summary>
        /// Register a new event managed by the mediator, it will dispatches messages
        /// for this event
        /// </summary>
        /// <param name="evType">Event to register</param>
        private List<IEventHandler> RegisterEvent(EventType evType)
        {
            List<IEventHandler> listeners = GetListeners(evType);
            if (listeners != null) return listeners;

            listeners = new List<IEventHandler>();
            _eventListenersMap.Add(evType.eventHash, listeners);

            return listeners;
        }

        /// <summary>
        /// Unregister an event, mediator will stop dispatching message for this event
        /// </summary>
        /// <param name="evType">Event to unregister</param>
        /// <returns>Return true if the event is unregistered, otherwise false</returns>
        private bool UnregisterEvent(EventType evType)
        {
            List<IEventHandler> listeners;
            _eventListenersMap.TryGetValue(evType.eventHash, out listeners);

            return (listeners != null) && _eventListenersMap.Remove(evType.eventHash);
        }

        /// <summary>
        /// Register a listener for a specific event
        /// </summary>
        /// <param name="evType">Event the listener is listening for</param>
        /// <param name="listener">Listener receiving messages for the specified event</param>
        public void Register(EventType evType, IEventHandler listener)
        {
            List<IEventHandler> listeners = RegisterEvent(evType);
            if (listeners.Contains(listener)) throw new Exception(string.Format("Listener {0} already listen to event {1}", listener, evType.eventName));

            listeners.Add(listener);
        }

        /// <summary>
        /// Unregister a listener for a specific event
        /// </summary>
        /// <param name="evType">Event to stop listening for</param>
        /// <param name="listener">Listener</param>
        /// <returns>Return true if the listener stop listening, otherwise false</returns>
        public bool Unregister(EventType evType, IEventHandler listener)
        {
            List<IEventHandler> listeners = GetListeners(evType);
            if (listeners == null) return false;

            return listeners.Remove(listener);
        }

        /// <summary>
        /// Unregister a listener for all event managed by the mediator
        /// </summary>
        /// <param name="listener">Listener</param>
        public void Unregister(IEventHandler listener)
        {
            Dictionary<int, List<IEventHandler>>.ValueCollection listenerSets = _eventListenersMap.Values;
            foreach(List<IEventHandler> handlerList in listenerSets)
            {
                handlerList.Remove(listener);
            }
        }

        /// <summary>
        /// Add a message to the queue
        /// </summary>
        /// <param name="msg">Message to add in the queue</param>
        public void QueueEvent(Message msg)
        {
            List<IEventHandler> listeners;
            _eventListenersMap.TryGetValue(msg.Event.eventHash, out listeners);
            if (listeners == null) return;

            _queueList[_activeQueue].Enqueue(msg);
        }

        /// <summary>
        /// Send a message immediately to concerned listeners
        /// </summary>
        /// <param name="msg">Message to send</param>
        public void Trigger(Message msg)
        {
            List<IEventHandler> listeners;
            _eventListenersMap.TryGetValue(msg.Event.eventHash, out listeners);
            if ((listeners == null) || (listeners.Count == 0)) return;

            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].HandleEvent(msg);
            }
        }

        /// <summary>
        /// Process the message queue
        /// </summary>
        /// <param name="maxProcessTime">Max time for processing the queue</param>
        public void Tick(long maxProcessTime)
        {
            long currentTime = Stopwatch.GetTimestamp();
            long maxTime = (maxProcessTime == InfiniteTime) ? InfiniteTime : (currentTime + maxProcessTime);

            int queueToProcessIdx = _activeQueue;
            _activeQueue = (_activeQueue + 1) % CountQueue;
            _queueList[_activeQueue].Clear();

            Queue<Message> processedQueue = _queueList[queueToProcessIdx];
            while (processedQueue.Count > 0)
            {
                Message msg = processedQueue.Dequeue();
                msg.Timestamp = currentTime;

                List<IEventHandler> listeners;
                _eventListenersMap.TryGetValue(msg.Event.eventHash, out listeners);
                if ((listeners == null) || (listeners.Count == 0)) continue;

                for (int i = 0; i < listeners.Count; i++)
                {
                    listeners[i].HandleEvent(msg);
                }

                currentTime = Stopwatch.GetTimestamp();
                if (maxProcessTime == InfiniteTime) continue;
                if (currentTime >= maxTime) break;
            }

            bool emptyQueue = (processedQueue.Count == 0);
            if (emptyQueue) return;

            while (processedQueue.Count > 0)
            {
                Message remainingMsg = processedQueue.Dequeue();
                _queueList[_activeQueue].Enqueue(remainingMsg);
            }
        }

        #endregion
    }
}
