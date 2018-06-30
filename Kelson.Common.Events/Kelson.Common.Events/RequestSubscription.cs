using System;

namespace Kelson.Common.Events
{
    public class RequestSubscription<TRequest, TResponse> : IRequestable
    {
        internal readonly Guid ID;
        internal bool Active = true;

        private readonly Func<TRequest, TResponse> handler;
        private readonly Action<RequestSubscription<TRequest, TResponse>> unsubscribe;

        internal RequestSubscription(Func<TRequest, TResponse> handle, Action<RequestSubscription<TRequest, TResponse>> unsubscribe, Guid? id = null)
        {
            ID = id ?? Guid.NewGuid();
            this.handler = request =>
            {
                Invokations++;
                return handle(request);
            };
            this.unsubscribe = unsubscribe;
        }

        public (bool active, TResponse response) Handle(TRequest request)
        {
            if (!Active)
                return (false, default);
            else
                return (true, handler(request));
        }

        public (bool success, TResponse1 response) Query<TRequest1, TResponse1>(TRequest1 request)
        {
            var (success, result) = Handle((TRequest)((object)request));
            return (success, (TResponse1)((object)result));
        }

        public int Invokations { get; private set; }

        public void Unsubscribe()
        {
            Active = false;
            unsubscribe(this);
        }        
    }
}
