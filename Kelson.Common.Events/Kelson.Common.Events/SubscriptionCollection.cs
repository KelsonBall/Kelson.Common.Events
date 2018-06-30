using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Kelson.Common.Events
{
    internal class SubscriptionCollection
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IPublishable>> subscriptions = new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IPublishable>>();

        public IEnumerable<IPublishable> this[Type type]
        {
            get
            {
                if (subscriptions.TryGetValue(type, out ConcurrentDictionary<Guid, IPublishable> subs))
                    foreach (var sub in subs.Values)
                        yield return sub;
                yield break;
            }
        }

        public ISubscription Subscribe<T>(Action<T> action)
        {
            var subscription = new Subscription<T>(t => action(t), s => subscriptions[typeof(T)].TryRemove(s.ID, out IPublishable sub));
            if (!subscriptions.ContainsKey(typeof(T)))
                subscriptions[typeof(T)] = new ConcurrentDictionary<Guid, IPublishable>();
            subscriptions[typeof(T)].TryAdd(subscription.ID, subscription);
            return subscription;
        }
              
        public ISubscription Listen<T>(Action<T> action)
        {
            var id = Guid.NewGuid();
            var innerSub = new Subscription<T>(t => action(t), s => subscriptions[typeof(T)].TryRemove(s.ID, out IPublishable sub), id);
            var outerSub = new WeakSubscription(innerSub, id);
            if (!subscriptions.ContainsKey(typeof(T)))
                subscriptions[typeof(T)] = new ConcurrentDictionary<Guid, IPublishable>();
            subscriptions[typeof(T)].TryAdd(innerSub.ID, outerSub);
            return innerSub;
        }
    }
}
