using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kelson.Common.Events
{
    interface IChanneledEventManager<TChannel> where TChannel : Enum
    {
        /// <summary>
        /// Subscribe to an event type
        /// </summary>
        /// <returns>A subscription token</returns>
        ISubscription Subscribe<T>(TChannel channel, Action<T> action);

        /// <summary>
        /// Subscribes with a weak reference, so the subscription token must be stored.
        /// </summary>
        /// <returns>A subscription token that should contain the only strong reference to the subscription</returns>
        ISubscription Listen<T>(TChannel channel, Action<T> action);

        /// <summary>
        /// Publishes an event, dispatching the payload to subscribers synchronously
        /// </summary>
        void Publish<T>(TChannel channel, T payload);

        /// <summary>
        /// Publishes an event, dispatching the payload to subscribers asynchronously
        /// </summary>
        Task PublishAsync<T>(TChannel channel, T payload);        

        /// <summary>
        /// Routes requests for TRequest to requestHandler
        /// </summary>
        ISubscription Handle<TRequest, TResponse>(TChannel channel, Func<TRequest, TResponse> requestHandler);        

        /// <summary>
        /// Publishes a request object and returns every response
        /// </summary>
        IEnumerable<TResponse> Request<TRequest, TResponse>(TChannel channel, TRequest request);

        /// <summary>
        /// Asynchronously publishes a request object and returns every response
        /// </summary>
        Task<IEnumerable<TResponse>> RequestAsync<TRequest, TResponse>(TChannel channel, TRequest request);
    }
}
