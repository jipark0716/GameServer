using Microsoft.Extensions.Hosting;
using Network.Node;
using Network.Packets;
using Network.Sockets;
using Serilog;
using Util.Extensions;

namespace Network;

public class Application : IHostedService
{
    private readonly ConnectionManager _connectionManager;
    private readonly Queue<ClientMessage> _clientMessageQueue = new();
    private readonly IGameNode _gameNode;

    public Application(NetworkConfig config, IGameNode gameNode)
    {
        _gameNode = gameNode;
        _connectionManager = new(_clientMessageQueue, config.MaxConnections, config.Port);
    }

    public Task StartAsync(CancellationToken cancellationToken) => Task.Run(() =>
    {
        _connectionManager.Start(); // network 스레드
        _clientMessageQueue.DequeueLoop().Each(OnMessage);
    }, cancellationToken);
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("End Network Application");
        return Task.CompletedTask;
    }
    
    private IEnumerable<(ushort, byte[])> ChunkStream(byte[]? payload)
    {
        ObjectDisposedException.ThrowIf(
            payload is null,
            new ArgumentNullException(nameof(payload)));

        while (payload.Length >= 4)
        {
            var length = BitConverter.ToUInt16(payload.AsSpan()[2..4]);
            yield return (BitConverter.ToUInt16(payload.AsSpan()[..2]), payload[4..(length + 4)]);
            payload = payload[(length + 4)..];
        }
    }

    private void OnMessage(ClientMessage message)
    {
        try
        {
            switch (message.Type)
            {
                case MessageType.Disconnect:
                    _gameNode.OnDisconnect(message.Author);
                    break;
                case MessageType.Connect:
                    _gameNode.OnConnect(message.Author);
                    break;
                case MessageType.Message:
                    foreach (var (actionType, body) in ChunkStream(message.Payload))
                    {
                        _gameNode.OnMessage(message.Author, actionType, body);
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