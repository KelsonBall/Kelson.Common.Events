using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kelson.Common.Events
{
    public class ChanneledEventManager<TChannel> : IChanneledEventManager<TChannel> where TChannel : Enum
    {
        private readonly Dictionary<TChannel, SubscriptionCollection> subscriptions = new Dictionary<TChannel, SubscriptionCollection>();
        private readonly Dictionary<TChannel, RequestCollection> requests = new Dictionary<TChannel, RequestCollection>();
        
        public ISubscription Subscribe<T>(TChannel channel, Action<T> action)
        {
            if (!subscriptions.ContainsKey(channel))
                subscriptions.Add(channel, new SubscriptionCollection());
            return subscriptions[channel].Subscribe(action);
        }

        public ISubscription Listen<T>(TChannel channel, Action<T> action)
        {
            if (!subscriptions.ContainsKey(channel))
                subscriptions.Add(channel, new SubscriptionCollection());
            return subscriptions[channel].Listen(action);
        }

        public void Publish<T>(TChannel channel, T payload)
        {
            if (!subscriptions.ContainsKey(channel))
                subscriptions.Add(channel, new SubscriptionCollection());
            foreach (var sub in subscriptions[channel][typeof(T)])
                sub.Publish(payload);
        }        

        public async Task PublishAsync<T>(TChannel channel, T payload)
        {
            if (!subscriptions.ContainsKey(channel))
                subscriptions.Add(channel, new SubscriptionCollection());
            await Task.Run(() => Publish(channel, payload));
        }

        public ISubscription Handle<TRequest, TResponse>(TChannel channel, Func<TRequest, TResponse> requestHandler)
        {
            if (!requests.ContainsKey(channel))
                requests.Add(channel, new RequestCollection());
            return requests[channel].Subscribe<TRequest, TResponse>(requestHandler);
        }

        public IEnumerable<TResponse> Request<TRequest, TResponse>(TChannel channel, TRequest request)
        {
            if (!subscriptions.ContainsKey(channel))
                subscriptions.Add(channel, new SubscriptionCollection());

            foreach (var sub in requests[channel][typeof(TRequest), typeof(TResponse)])
            {
                var (success, response) = sub.Query<TRequest, TResponse>(request);
                if (success)
                    yield return response;
            }
        }

        public async Task<IEnumerable<TResponse>> RequestAsync<TRequest, TResponse>(TChannel channel, TRequest request)
        {
            if (!subscriptions.ContainsKey(channel))
                subscriptions.Add(channel, new SubscriptionCollection());
            return await Task.Run(() => Request<TRequest, TResponse>(channel, request));
        }        
    }
}
