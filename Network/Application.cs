using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Network.Attributes;
using Network.EventListeners;
using Network.Packets;
using Network.Sockets;
using Serilog;
using Util.Extensions;

namespace Network;

public abstract class Application : IHostedService
{
    private readonly ConnectionManager _connectionManager;
    private readonly Queue<ClientMessage> _clientMessageQueue = new();
    protected readonly OnClientMessageListener Listener;

    protected Application(NetworkConfig config)
    {
        Listener = new(this);
        Listener.AddAction(100, nameof(Authorization));
        _connectionManager = new(_clientMessageQueue, config.MaxConnections, config.Port);
    }

    public void Authorization([Author] Author author, [Jwt] AuthorizeRequestDto request)
    {
        author.UserId = request.Id;
        AuthorizeResponseDto response = new(author.UserId ?? throw new Exception("user id is null"));
        author.Socket.SendAsync(response.Encapsulation(101));
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.Run(() =>
    {
        _connectionManager.Start(); // network 스레드
        _clientMessageQueue.DequeueLoop().Each(OnMessage);
    });
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("End Network Application");
        return Task.CompletedTask;
    }
    
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
    {
        Log.Information(
            "[{connectionId}] connect ip:{ip}",
            connectionId,
            socket.RemoteEndPoint?.ToString());
        socket.SendAsync(new HelloPacket(connectionId).Encapsulation(100));
    }

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
                        catch (SecurityTokenMalformedException)
                        {
                            Log.Information(
                                "[{connectionId}] auth fail id:{id} type:{type} request:{body}",
                                message.Author.ConnectionId, 
                                message.Author.UserId,
                                actionType,
                                Encoding.Default.GetString(body));
                        }
                        catch (Exception e)
                        {
                            Log.Error(
                                e,
                                "[{connectionId}] action fail id:{id} type:{type} request:{body}",
                                message.Author.ConnectionId,
                                message.Author.UserId,
                                actionType,
                                Encoding.Default.GetString(body));
                        }
                    }

                    break;
                default:
                    throw new InvalidOperationException($"not support type {message.Type}");
            }
        }
        catch (Exception e)
        {
            Log.Error(e, 
                "[{connectionId}] request parse fail id:{Id}",
                message.Author.ConnectionId,
                message.Author.UserId);
        }
    }
}