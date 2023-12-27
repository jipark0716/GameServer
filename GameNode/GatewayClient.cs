using Common.Queues;
using System.Net.Sockets;
using GameNode.Packets;
using Common.Extensions;

namespace GameNode;

public class GatewayClient(byte serverId, IQueue<ClientMessage> clientMessageQueue, IQueue<ServerMessage> serverMessageQueue) : IDisposable
{
    private readonly byte _serverId = serverId;
    private TcpClient? _tcpClient;
    private NetworkStream? _networkStream;
    private readonly IQueue<ClientMessage> _clientMessageQueue = clientMessageQueue;
    private readonly IQueue<ServerMessage> _serverMessageQueue = serverMessageQueue;
    private readonly InvalidOperationException _invalidOperationException = new();

    public void Start()
    {
        _tcpClient = new TcpClient("127.0.0.1", 7000);
        _networkStream = _tcpClient.GetStream();
        _networkStream.Write([_serverId], 0, 1);
        Task.Run(() =>
        {
            Receive();
            Dispose();
        });
        Task.Run(() => _serverMessageQueue.Each(Send));
    }

    private void Send(ServerMessage serverMessage)
    {
        var payload = serverMessage.GetBytes();
        (_networkStream ?? throw _invalidOperationException).Write(payload, 0, payload.Length);
    }

    private void Receive()
    {
        int payloadSize;
        byte[] buffer = new byte[1024 * 4];
        while (true)
        {
            payloadSize = (_networkStream ?? throw _invalidOperationException).Read(buffer, 0, buffer.Length);
            _clientMessageQueue.EnQueue(new()
            {
                SessionId = BitConverter.ToUInt64(buffer.AsSpan()[..8]),
                ProtocolType = BitConverter.ToUInt16(buffer.AsSpan()[8..10]),
                Payload = buffer[10..payloadSize],
            });
        }
    }

    public void Dispose()
    {
        _networkStream?.Close();
        _tcpClient?.Close();
    }
}
