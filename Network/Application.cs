using System.Net.Sockets;
using Network.Attributes;
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
        Listener.AddAction(100, nameof(Authorization));
        _connectionManager = new(_clientMessageQueue, maxConnections, port);
    }

    public void Authorization([Author] Author author, [Jwt] AuthorizeRequestDto request)
    {
        author.UserId = request.Id;
        AuthorizeResponseDto response = new(author.UserId ?? throw new Exception("user id is null"));
        author.Socket.SendAsync(response.Encapsuleation(101));
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
            var length = BitConverter.ToUInt16(payload.AsSpan()[2..4]);
            yield return (BitConverter.ToUInt16(payload.AsSpan()[..2]), payload[4..(length + 4)]);
            payload = payload[(length + 4)..];
        }
    }

    protected virtual void OnConnect(Socket socket, ulong connectionId)
        => socket.SendAsync(new HelloPacket(connectionId).Encapsuleation(100));

    private void OnMessage(ClientMessage message)
    {
        try
        {
            switch (message.Type)
            {
                case MessageType.Disconnect:
                    OnDisconnect(message.Author.ConnectionId);
                    break;
                case MessageType.Connect:
                    OnConnect(message.Author.Socket, message.Author.ConnectionId);
                    break;
                case MessageType.Message:
                    foreach (var (actionType, body) in ChunkStream(message.Payload))
                    {
                        try
                        {
                            Listener.OnMessage(actionType, message.Author, body);
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
        catch (Exception e)
        {
            // ignored
        }
    }
}