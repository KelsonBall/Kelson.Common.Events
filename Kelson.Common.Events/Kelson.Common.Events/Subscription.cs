using System;

namespace Kelson.Common.Events
{
    public class Subscription<T> : ISubscription, IPublishable
    {
        internal readonly Guid ID;
        internal bool Active = true;

        private readonly Action<T> publish;
        private readonly Action<Subscription<T>> unsubscribe;

        internal Subscription(Action<T> publish, Action<Subscription<T>> unsubscribe, Guid? id = null)
        {
            ID = id ?? Guid.NewGuid();
            this.publish = payload =>
            {
                Invokations++;
                publish(payload);
            };
            this.unsubscribe = unsubscribe;
        }

        public void Publish<Tp>(Tp payload)
        {
            if (!Active)
                return;
            publish((T)((object)payload));
        }

        public int Invokations { get; private set; }

        public void Unsubscribe()
        {
            Active = false;
            unsubscribe(this);
        }
    }
}
