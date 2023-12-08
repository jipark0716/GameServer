using Chat.Packets.Request;
using Chat.Packets.Response;
using Chat.Services;
using EchoServer.Application;
using EchoServer.Dtos;
using EchoServer.Extensions;

namespace Chat.Controllers
{
    [SocketController]
    public class MessageController
    {
        private readonly ChatChannelService _chatChannelService;

        public MessageController(ChatChannelService chatChannelService)
        {
            _chatChannelService = chatChannelService;
        }

        [Action(1525)]
        public ResponsePacket Send(EchoServer.Dtos.ISession session, SendMessageRequest request)
        {
            var message = _chatChannelService.SendMessage(session, request.TopicId, request.MessageContent);
            MessageResponse response = new()
            {
                Sequence = request.Sequence,
                TopicId = request.TopicId,
                Message = message,
            };

            return new()
            {
                Target = new TopicTarget() { TopicId = request.TopicId },
                Payload = response.ToJsonByte(),
            };
        }
    }
}
