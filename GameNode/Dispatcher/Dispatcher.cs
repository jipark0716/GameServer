using Common.Extensions;
using Common.Queues;
using GameNode.Packets;
using GameNode.Session;

namespace GameNode.Dispatcher;

public class Dispatcher(IQueue<ClientMessage> clientMessageQueue, SessionService sessionService)
{
    private readonly IQueue<ClientMessage> _clientMessageQueue = clientMessageQueue;
    private readonly SessionService _sessionService = sessionService;

    public void Start() => Task.Run(() => _clientMessageQueue.Each(Produce));

    public void Produce(ClientMessage clientMessage)
    {
        var sessionId = BitConverter.ToUInt64(clientMessage.Payload.AsSpan()[..8]);
        var actionType = BitConverter.ToUInt16(clientMessage.Payload.AsSpan()[8..10]);
        var payload = clientMessage.Payload[10..];
        switch (actionType)
        {
            case 0:
                _sessionService.AddSession(sessionId, payload); break;
            case 1:
                _sessionService.DisconnectSession(sessionId); break;
            default:
                var json = GetJsonPayload(actionType, clientMessage.Payload);
                break;
        }
    }
    
    private string GetJsonPayload(ushort actionType, byte[] payload)
    {
        return "";
    }
}
