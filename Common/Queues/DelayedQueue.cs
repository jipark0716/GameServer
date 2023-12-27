using System.Collections;

namespace Common.Queues;

public class DelayedQueue<T>(TimeSpan delay) : IQueue<T>
{
    private readonly TimeSpan _delay = delay;
    private readonly DoubleBufferingQueue<(T item, DateTime at)> _queue = new();

    public void EnQueue(T item)
    {
        _queue.EnQueue((item, DateTime.Now));
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var (item, at) in _queue)
        {
            var diff = DateTime.Now - at;
            if (diff < _delay)
            {
                Thread.Sleep(_delay - diff);
            }
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
