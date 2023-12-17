namespace Common.Queues;

public interface IQueue<T> : IEnumerable<T>
{
    public void EnQueue(T data);
}
