using EchoServer.Extensions;
using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Net.WebSockets;

namespace EchoServer.Queues
{
    public class DoubleBufferingQueue<T> : IEnumerable<T>
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

        public Queue<T> TaksAll()
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
                packetQueue = TaksAll();
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
}
