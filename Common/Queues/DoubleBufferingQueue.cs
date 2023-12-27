using Common.Extensions;
using System.Collections;

namespace Common.Queues;

public class DoubleBufferingQueue<T> : IQueue<T>
{
    private Queue<T> _inQueue = new();
    private Queue<T> _outQueue = new();
    private SpinLock _lock = new();

    public void EnQueue(T data)
    {
        _lock.Enter(() =>
        {
            _inQueue.Enqueue(data);
            return Task.CompletedTask;
        });
    }

    public Queue<T> TakeAll()
    {
        Swap();
        return _outQueue;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return GetEnumerator(TimeSpan.FromMilliseconds(50));
    }

    public IEnumerator<T> GetEnumerator(TimeSpan Cycle)
    {
        Queue<T> packetQueue;
        while (true)
        {
            packetQueue = TakeAll();
            if (packetQueue.Count == 0)
            {
                Thread.Sleep(Cycle);
                continue;
            }

            while (packetQueue.TryDequeue(out var result))
            {
                yield return result;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void Swap()
    {
        _lock.Enter(() =>
        {
            (_outQueue, _inQueue) = (_inQueue, _outQueue);
            return Task.CompletedTask;
        });
    }
}
