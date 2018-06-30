using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Kelson.Common.Events
{
    internal class RequestCollection
    {
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IRequestable>>> subscriptions 
            = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IRequestable>>>();

        public IEnumerable<IRequestable> this[Type trequest, Type tresponse]
        {
            get
            {
                if (subscriptions.TryGetValue(trequest, out ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IRequestable>> requestSubs))
                    if (subscriptions[trequest].TryGetValue(tresponse, out ConcurrentDictionary<Guid, IRequestable> responseSubs)) // not atomic when nesting TryGetValue
                        foreach (var sub in responseSubs.Values)
                            yield return sub;
                yield break;
            }
        }

        public IRequestable Subscribe<TRequest, TResponse>(Func<TRequest, TResponse> action)
        {
            var subscription = new RequestSubscription<TRequest, TResponse>(
                t => action(t), 
                s => subscriptions[typeof(TRequest)][typeof(TResponse)].TryRemove(s.ID, out IRequestable sub));
            if (!subscriptions.ContainsKey(typeof(TRequest)))
                subscriptions[typeof(TRequest)] = new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IRequestable>>();
            if (!subscriptions[typeof(TRequest)].ContainsKey(typeof(TResponse)))
                subscriptions[typeof(TRequest)][typeof(TResponse)] = new ConcurrentDictionary<Guid, IRequestable>();
            subscriptions[typeof(TRequest)][typeof(TResponse)].TryAdd(subscription.ID, subscription);
            return subscription;
        }        
    }
}
