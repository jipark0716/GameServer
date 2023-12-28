using Common.Extensions;
using Common.Queues;
using GameNode.Listener;
using GameNode.Packets;
using GameNode.Session;

namespace GameNode.Dispatcher;

public class Dispatcher(IQueue<ClientMessage> clientMessageQueue, SessionService sessionService)
{
    private readonly IQueue<ClientMessage> _clientMessageQueue = clientMessageQueue;
    private readonly SessionService _sessionService = sessionService;
    private event Action<ushort, byte[], Session.Session>? _listener;

    public void Start() => Task.Run(() => _clientMessageQueue.Each(Produce));

    public void Produce(ClientMessage clientMessage)
    {
        var actionType = BitConverter.ToUInt16(clientMessage.Payload.AsSpan()[..2]);
        var payload = clientMessage.Payload[2..];
        switch (actionType)
        {
            case 0:
                _sessionService.AddSession(clientMessage.SessionId, payload); break;
            case 1:
                _sessionService.DisconnectSession(clientMessage.SessionId); break;
            default:
                try
                {
                    _listener?.Invoke(actionType, clientMessage.Payload, _sessionService.GetSession(clientMessage.SessionId));
                }
                catch (Exception ex)
                {
                    // session not found
                }
                break;
        }
    }

    public void AddEventListener(BaseListener listener)
    {
        _listener += listener.OnMessage;
    }
}
