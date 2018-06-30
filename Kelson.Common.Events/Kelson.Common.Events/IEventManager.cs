using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kelson.Common.Events
{
    /// <summary>
    /// An unchanneled event manager
    /// </summary>
    public interface IEventManager
    {
        /// <summary>
        /// Subscribe to an event type
        /// </summary>
        /// <returns>A subscription token</returns>
        ISubscription Subscribe<T>(Action<T> action);

        /// <summary>
        /// Subscribes with a weak reference, so the subscription token must be stored.
        /// </summary>
        /// <returns>A subscription token that should contain the only strong reference to the subscription</returns>
        ISubscription Listen<T>(Action<T> action);

        /// <summary>
        /// Publishes an event, dispatching the payload to subscribers synchronously
        /// </summary>
        void Publish<T>(T payload);

        /// <summary>
        /// Publishes an event, dispatching the payload to subscribers asynchronously
        /// </summary>
        Task PublishAsync<T>(T payload);

        /// <summary>
        /// Wraps a payload in an event
        /// </summary>
        void Publish<TEvent, TPayload>(TPayload payload) where TEvent : Event<TPayload>;

        /// <summary>
        /// Routes requests for TRequest to requestHandler
        /// </summary>
        ISubscription Handle<TRequest, TResponse>(Func<TRequest, TResponse> requestHandler);        

        /// <summary>
        /// Publishes a request object and returns every response
        /// </summary>
        IEnumerable<TResponse> Request<TRequest, TResponse>(TRequest request);

        /// <summary>
        /// Asynchronously publishes a request object and returns every response
        /// </summary>
        Task<IEnumerable<TResponse>> RequestAsync<TRequest, TResponse>(TRequest request);
    }
}
