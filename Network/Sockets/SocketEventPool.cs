using System.Net.Sockets;

namespace Network.Sockets;

public class SocketEventPool(int capacity)
{
    private readonly Stack<SocketAsyncEventArgs> _pool = new(capacity);

    public void Init(int size, Func<SocketAsyncEventArgs> create)
    {
        for (var i = 0; i < size; i++)
        {
            Push(create());
        }
    }

    public void Push(SocketAsyncEventArgs item)
    {
        lock (_pool)
        {
            _pool.Push(item);
        }
    }

    public SocketAsyncEventArgs Pop()
    {
        lock (_pool)
        {
            return _pool.Pop();
        }
    }
}