using Boostrap.DI;
using EchoServer.Dtos;

namespace EchoServer.Factories
{
    [LifeCycle(LifeCycle.Scoped, typeof(WebsocketPacketFactory))]

    public interface IPacketFactory
    {
        public Packet CreateBasic(Dtos.ISession session, byte[] body);
        public Packet CreateClose(Dtos.ISession session);
        public Packet CreateLargeRequest(Dtos.ISession session);
    }

    [LifeCycle(LifeCycle.Scoped)]
    public class WebsocketPacketFactory : IPacketFactory
    {
        public Packet CreateBasic(Dtos.ISession session, byte[] payload)
        {
            return new()
            {
                Session = session,
                Header = BitConverter.ToUInt16(payload[..1]),
                Body = payload[1..],
                Type = PacketType.Basic,
            };
        }

        public Packet CreateClose(Dtos.ISession session)
        {
            return new()
            {
                Session = session,
                Type = PacketType.Close,
            };
        }

        public Packet CreateLargeRequest(Dtos.ISession session)
        {
            return new()
            {
                Session = session,
                Type = PacketType.ContentToLarge,
            };
        }
    }
}
