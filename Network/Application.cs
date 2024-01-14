using System.Net.Sockets;
using Network.EventListeners;
using Network.Packets;
using Network.Sockets;
using Util.Extensions;

namespace Network;

public abstract class Application
{
    private readonly ConnectionManager _connectionManager;
    private readonly Queue<ClientMessage> _clientMessageQueue = new();
    protected readonly OnClientMessageListener Listener;

    protected Application(int maxConnections, int port)
    {
        Listener = new(this);
        _connectionManager = new(_clientMessageQueue, maxConnections, port);
    }

    public Task StartAsync() => Task.Run(() =>
    {
        _connectionManager.Start(); // network 스레드
        _clientMessageQueue.DequeueLoop().Each(OnMessage);
    });
    
    protected virtual void OnDisconnect(ulong id) {}

    private IEnumerable<(ushort, byte[])> ChunkStream(byte[]? payload)
    {
        ObjectDisposedException.ThrowIf(
            payload is null,
            new ArgumentNullException("payload is must not null"));

        while (payload.Length >= 4)
        {
            (ushort, byte[]) result;
            try
            {
                var length = BitConverter.ToUInt16(payload.AsSpan()[2..4]);
                result = (BitConverter.ToUInt16(payload.AsSpan()[..2]), payload[4..(length + 4)]);
                payload = payload[(length + 4)..];
            }
            catch (Exception e)
            {
                continue;
            }
            yield return result;
        }
    }

    protected virtual void OnConnect(Socket socket, ulong connectionId)
        => socket.SendAsync(new HelloPacket(connectionId).Encapsuleation(100));

    private void OnMessage(ClientMessage message)
    {
        switch (message.Type)
        {
            case MessageType.Disconnect:
                OnDisconnect(message.ConnectionId);
                break;
            case MessageType.Connect:
                OnConnect(message.Socket, message.ConnectionId);
                break;
            case MessageType.Message:
                foreach (var (actionType, body) in ChunkStream(message.Payload))
                {
                    try
                    {
                        Listener.OnMessage(actionType, message.ConnectionId, message.Socket, body);
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                }
                break;
            default:
                throw new InvalidOperationException($"not support type {message.Type}");
        }
    }
}