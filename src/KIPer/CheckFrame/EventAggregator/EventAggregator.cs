using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace KipTM.EventAggregator
{
    public class EventAggregator : IEventAggregator
    {
        private readonly SynchronizationContext _context;
        private readonly Dictionary<EventSubscriber, List<WeakReference>> _eventSubscriberLists = new Dictionary<EventSubscriber, List<WeakReference>>();
        private readonly object _registerLock = new object();

        public EventAggregator()
        {
            _context = SynchronizationContext.Current;
        }

        public void Subscribe(object subscriber, object token = null)
        {
            Type type = subscriber.GetType();
            var subscriberTypes = GetSubscriberInterfaces(type)
                .ToArray();
            if (!subscriberTypes.Any())
            {
                throw new ArgumentException("Подписчик должен реализовать хотя бы один интерфейс ISubscriber<TEvent>");
            }

            lock (_registerLock)
            {
                var weakReference = new WeakReference(subscriber);
                foreach (var subscriberType in subscriberTypes)
                {
                    var subscribers = GetSubscribers(new EventSubscriber { SubscriberType = subscriberType, Token = token });
                    subscribers.Add(weakReference);
                }
            }
        }

        public void Unsubscribe(object subscriber, object token = null)
        {
            Type type = subscriber.GetType();
            var subscriberTypes = GetSubscriberInterfaces(type);

            lock (_registerLock)
            {
                foreach (var subscriberType in subscriberTypes)
                {
                    var subscribers = GetSubscribers(new EventSubscriber { SubscriberType = subscriberType, Token = token });
                    subscribers.RemoveAll(x => x.IsAlive && ReferenceEquals(x.Target, subscriber));
                }
            }
        }

        public void Send<TMessage>(TMessage message, object token = null)
        {
            var subscriberType = typeof(ISubscriber<>).MakeGenericType(typeof(TMessage));
            var subscribers = GetSubscribers(new EventSubscriber { SubscriberType = subscriberType, Token = token });
            var subscribersToRemove = new List<WeakReference>();

            WeakReference[] subscribersArray;
            lock (_registerLock)
            {
                subscribersArray = subscribers.ToArray();
            }

            foreach (var weakSubscriber in subscribersArray)
            {
                var subscriber = (ISubscriber<TMessage>)weakSubscriber.Target;
                if (subscriber != null && _context != null)
                {
                    _context.Send(m => subscriber.OnEvent(message), null);
                }
                else
                {
                    subscribersToRemove.Add(weakSubscriber);
                }
            }
            if (subscribersToRemove.Any())
            {
                lock (_registerLock)
                {
                    foreach (var remove in subscribersToRemove)
                        subscribers.Remove(remove);
                }
            }
        }

        public void Post<TMessage>(TMessage message, object token = null)
        {
            var subscriberType = typeof(ISubscriber<>).MakeGenericType(typeof(TMessage));
            var subscribers = GetSubscribers(new EventSubscriber { SubscriberType = subscriberType, Token = token });
            var subscribersToRemove = new List<WeakReference>();

            WeakReference[] subscribersArray;
            lock (_registerLock)
            {
                subscribersArray = subscribers.ToArray();
            }

            foreach (var weakSubscriber in subscribersArray)
            {
                var subscriber = (ISubscriber<TMessage>)weakSubscriber.Target;
                if (subscriber != null && _context != null)
                {
                    _context.Post(m => subscriber.OnEvent(message), null);
                }
                else
                {
                    subscribersToRemove.Add(weakSubscriber);
                }
            }
            if (subscribersToRemove.Any())
            {
                lock (_registerLock)
                {
                    foreach (var remove in subscribersToRemove)
                        subscribers.Remove(remove);
                }
            }
        }

        private List<WeakReference> GetSubscribers(EventSubscriber subscriberType)
        {
            List<WeakReference> subscribers;
            lock (_registerLock)
            {
                var found = _eventSubscriberLists.TryGetValue(subscriberType, out subscribers);
                if (!found)
                {
                    subscribers = new List<WeakReference>();
                    _eventSubscriberLists.Add(subscriberType, subscribers);
                }
            }
            return subscribers;
        }

        private IEnumerable<Type> GetSubscriberInterfaces(Type subscriberType)
        {
            return subscriberType
                .GetInterfaces()
                .Where(i => i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ISubscriber<>));
        }
    }
}
