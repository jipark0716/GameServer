using System.Diagnostics.Contracts;
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

    private void OnMessage(ClientMessage message)
    {
        if (message.MessageType is MessageType.Disconnect) // 연결 끊기 패킷
        {
            OnDisconnect(message.ConnectionId);
            return;
        }
        
        if (message.Payload is null || message.Payload.Length < 2)
        {
            return;
        }

        try
        {
            Listener.OnMessage(
                BitConverter.ToUInt16(message.Payload.AsSpan()[..2]),
                message.ConnectionId,
                message.Socket,
                message.Payload[2..]);
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}