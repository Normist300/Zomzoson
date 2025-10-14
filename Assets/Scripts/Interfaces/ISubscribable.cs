public interface ISubscribable<T> where T : IController
{
    void Subscribe(T controller);
    void Unsubscribe(T controller);
}