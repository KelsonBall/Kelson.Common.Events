using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kelson.Common.Events
{
    public class EventManager : IEventManager
    {
        private readonly SubscriptionCollection subscriptions = new SubscriptionCollection();

        /// <summary>
        /// Subscribe to events with payloads of type T
        /// </summary>        
        public ISubscription Subscribe<T>(Action<T> action) => subscriptions.Subscribe(action);

        /// <summary>
        /// Subscribe with a weak reference. Automatically unsubscribe when returned token goes out of scope.
        /// </summary>        
        public ISubscription Listen<T>(Action<T> action) => subscriptions.Listen(action);

        public void Publish<T>(T payload)
        {
            foreach (var sub in subscriptions[typeof(T)])
                sub.Publish(payload);
        }

        public async Task PublishAsync<T>(T payload) => await Task.Run(() => Publish(payload));

        public void Publish<TEvent, TPayload>(TPayload payload) where TEvent : Event<TPayload>
        {
            var e = Activator.CreateInstance<TEvent>();
            e.Args = payload;
            Publish(e);
        }

        private readonly RequestCollection requests = new RequestCollection();

        public ISubscription Handle<TRequest, TResponse>(Func<TRequest, TResponse> requestHandler)
        {
            return requests.Subscribe(requestHandler);
        }

        public IEnumerable<TResponse> Request<TRequest, TResponse>(TRequest request)
        {
            foreach (var sub in requests[typeof(TRequest), typeof(TResponse)])
            {
                var (success, response) = sub.Query<TRequest, TResponse>(request);
                if (success)
                    yield return response;
            }
        }

        public async Task<IEnumerable<TResponse>> RequestAsync<TRequest, TResponse>(TRequest request)
            => await Task.Run(() => Request<TRequest, TResponse>(request));
    }
}
