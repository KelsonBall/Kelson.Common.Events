using System;

namespace Kelson.Common.Events
{
    public class WeakSubscription : IPublishable
    {
        internal readonly WeakReference<IPublishable> subref;
        internal readonly Guid ID;
        internal bool Active = true;

        public WeakSubscription(IPublishable subscription, Guid? id = null)
        {
            ID = id ?? Guid.NewGuid();
            subref = new WeakReference<IPublishable>(subscription);
        }

        public void Publish<T>(T payload)
        {
            if (!Active)
                return;
            if (subref.TryGetTarget(out IPublishable sub))
                sub.Publish(payload);
            else
                Unsubscribe();
        }

        public int Invokations
        {
            get
            {
                if (subref.TryGetTarget(out IPublishable sub))
                    return sub.Invokations;
                else
                    Unsubscribe();
                return -1;
            }
        }

        public void Unsubscribe()
        {
            Active = false;
            if (subref.TryGetTarget(out IPublishable sub))
                sub.Unsubscribe();
        }
    }
}
