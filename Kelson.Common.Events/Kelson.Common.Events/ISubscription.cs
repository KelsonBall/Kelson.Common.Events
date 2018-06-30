namespace Kelson.Common.Events
{
    public interface ISubscription
    {
        int Invokations { get; }
        void Unsubscribe();
    }

    public interface IPublishable : ISubscription
    {
        void Publish<T>(T payload);
    }

    public interface IRequestable : ISubscription
    {
        (bool success, TResponse response) Query<TRequest, TResponse>(TRequest request);
    }
}
