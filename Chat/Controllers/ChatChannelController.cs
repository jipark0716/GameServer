using Chat.Packets;
using Chat.Packets.Request;
using Chat.Packets.Response;
using Chat.Services;
using EchoServer.Application;
using EchoServer.Dtos;
using EchoServer.Extensions;

namespace Chat.Controllers
{
    [SocketController]
    public class ChatChannelController
    {
        private readonly ChatChannelService _chatChannelService;

        public ChatChannelController(ChatChannelService chatChannelService)
        {
            _chatChannelService = chatChannelService;
        }

        [Action(1523)]
        public ResponsePacket CreateChannel(EchoServer.Dtos.ISession session, CreateChannelRequest request)
        {
            CreateChannelResponse response = new()
            {
                Sequence = request.Sequence,
                TopicId = _chatChannelService.CreateChannel(session).TopicId,
            };

            return new()
            {
                Target = new UserTarget() { Uid = session.Uid },
                Payload = response.ToJsonByte(),
            };
        }

        [Action(1524)]
        public ResponsePacket JoinChannel(EchoServer.Dtos.ISession session, JoinChannelRequest request)
        {
            var message = _chatChannelService.JoinChannel(session, request.TopicId);
            MessageResponse response = new()
            {
                Sequence = request.Sequence,
                TopicId = request.TopicId,
                Message = message,
            };
            
            return new()
            {
                Target = new TopicTarget() { TopicId = request.TopicId},
                Payload = response.ToJsonByte(),
            };
        }
    }
}

