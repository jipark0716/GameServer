using Boostrap.DI;
using ConcurrentCollections;
using EchoServer.Dtos;
using EchoServer.Factories;
using System.Collections.Generic;
using System.Net.WebSockets;
using EchoServer.Application;

namespace EchoServer.Services
{
    [LifeCycle(LifeCycle.Scoped)]
    public class WebsocketNetworkService : NetworkService<WebsocketSession>
    {
        public WebsocketNetworkService(
            IPacketFactory packetFactory,
            ISocketApplication application,
            TopicService topicService
        ) : base(packetFactory, application, topicService) { }

        public override async Task AddSession(WebsocketSession session)
        {
            await base.AddSession(session);

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult receiveResult;

            do
            {
                receiveResult = await session.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (receiveResult.EndOfMessage is false)
                {
                    _requestQueue.EnQueue(_packetFactory.CreateLargeRequest(session));
                    break;
                }
                _requestQueue.EnQueue(
                    receiveResult.CloseStatus.HasValue ?
                        _packetFactory.CreateClose(session) :
                        _packetFactory.CreateBasic(session, buffer[..receiveResult.Count])
                );
            } while (!receiveResult.CloseStatus.HasValue);

            await session.Socket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

        protected override void SendResponse(ulong receiver, byte[] payload)
        {
            if (!_sessions.TryGetValue(receiver, out var session))
            {
                return;
            }

            session.Socket.SendAsync(
                new ArraySegment<byte>(payload, 0, payload.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
}
