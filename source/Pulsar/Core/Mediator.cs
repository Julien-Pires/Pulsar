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
    /// which is processed at the end of the update loop
    /// </summary>
    public sealed class Mediator : Singleton<Mediator>
    {
        #region Fields

        private const long infiniteTime = long.MaxValue;
        private const int countQueue = 2;
        
        private int activeQueue = 0;
        private Queue<Message>[] queueList = new Queue<Message>[Mediator.countQueue];
        private Dictionary<int, List<IEventHandler>> eventListenersMap =
            new Dictionary<int, List<IEventHandler>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of Mediator class
        /// </summary>
        public Mediator()
        {
            for (int i = 0; i < Mediator.countQueue; i++)
            {
                this.queueList[i] = new Queue<Message>();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Register a new event managed by the mediator, it will dispatches messages
        /// for this event
        /// </summary>
        /// <param name="evType">Event to register</param>
        private void RegisterEvent(EventType evType)
        {
            List<IEventHandler> listeners;
            this.eventListenersMap.TryGetValue(evType.eventHash, out listeners);
            if (listeners != null)
            {
                return;
            }

            listeners = new List<IEventHandler>();
            this.eventListenersMap.Add(evType.eventHash, listeners);
        }

        /// <summary>
        /// Unregister an event, mediator will stop dispatching message for this event
        /// </summary>
        /// <param name="evType">Event to unregister</param>
        /// <returns>Return true if the event is unregistered, otherwise false</returns>
        private bool UnregisterEvent(EventType evType)
        {
            List<IEventHandler> listeners;
            this.eventListenersMap.TryGetValue(evType.eventHash, out listeners);
            if (listeners == null)
            {
                return false;
            }

            return this.eventListenersMap.Remove(evType.eventHash);
        }

        /// <summary>
        /// Register a listener for a specific event
        /// </summary>
        /// <param name="evType">Event the listener is listening for</param>
        /// <param name="listener">Listener receiving messages for the specified event</param>
        public void Register(EventType evType, IEventHandler listener)
        {
            List<IEventHandler> listeners;
            this.RegisterEvent(evType);
            this.eventListenersMap.TryGetValue(evType.eventHash, out listeners);
            if (listeners.Contains(listener))
            {
                throw new Exception(string.Format("Listener {0} already listen to event {1}", listener, evType.eventName));
            }

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
            List<IEventHandler> listeners;
            this.eventListenersMap.TryGetValue(evType.eventHash, out listeners);

            return listeners.Remove(listener);
        }

        /// <summary>
        /// Unregister a listener for all event managed by the mediator
        /// </summary>
        /// <param name="listener">Listener</param>
        public void Unregister(IEventHandler listener)
        {
            Dictionary<int, List<IEventHandler>>.ValueCollection listenerSets = this.eventListenersMap.Values;
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
            this.eventListenersMap.TryGetValue(msg.Event.eventHash, out listeners);
            if (listeners == null)
            {
                return;
            }

            this.queueList[this.activeQueue].Enqueue(msg);
        }

        /// <summary>
        /// Send a message immediately to concerned listeners
        /// </summary>
        /// <param name="msg">Message to send</param>
        public void Trigger(Message msg)
        {
            List<IEventHandler> listeners;
            this.eventListenersMap.TryGetValue(msg.Event.eventHash, out listeners);
            if ((listeners == null) || (listeners.Count == 0))
            {
                return;
            }

            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].HandleEvent(msg);
            }
        }

        /// <summary>
        /// Process the message queue
        /// </summary>
        public void Tick(long maxProcessTime)
        {
            long currentTime = Stopwatch.GetTimestamp();
            long maxTime = (maxProcessTime == Mediator.infiniteTime) ? Mediator.infiniteTime : (currentTime + maxProcessTime);

            int queueToProcessIdx = this.activeQueue;
            this.activeQueue = (this.activeQueue + 1) % Mediator.countQueue;
            this.queueList[this.activeQueue].Clear();

            Queue<Message> processedQueue = this.queueList[queueToProcessIdx];
            while (processedQueue.Count > 0)
            {
                Message msg = processedQueue.Dequeue();
                List<IEventHandler> listeners;
                this.eventListenersMap.TryGetValue(msg.Event.eventHash, out listeners);
                if ((listeners == null) || (listeners.Count == 0))
                {
                    continue;
                }

                for (int i = 0; i < listeners.Count; i++)
                {
                    listeners[i].HandleEvent(msg);
                }

                currentTime = Stopwatch.GetTimestamp();
                if (maxProcessTime != Mediator.infiniteTime)
                {
                    if (currentTime >= maxTime)
                    {
                        break;
                    }
                }
            }

            bool emptyQueue = (processedQueue.Count == 0);
            if (!emptyQueue)
            {
                while (processedQueue.Count > 0)
                {
                    Message remainingMsg = processedQueue.Dequeue();
                    this.queueList[this.activeQueue].Enqueue(remainingMsg);
                }
            }
        }

        #endregion
    }
}
