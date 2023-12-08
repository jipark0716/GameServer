using System;
using System.Collections.Concurrent;
using Boostrap.DI;
using Chat.Packets.Dtos;
using EchoServer.Services;

namespace Chat.Services
{
	[LifeCycle(LifeCycle.Singleton)]
	public class ChatChannelService
	{
		private readonly TopicService _topicService;
		private readonly ConcurrentDictionary<ulong, ChatChannel> _channels;

        public ChatChannelService(TopicService topicService)
		{
			_topicService = topicService;
			_topicService.OnClose += OnClose;
			_channels = new();
        }

		public IChannelMember GetMember(EchoServer.Dtos.ISession session)
		{
			return new ChannelMember()
			{
				SessionId = session.Uid,
            };
		}

        public IMessage SendMessage(EchoServer.Dtos.ISession session, ulong topicId, IMessageContent messageContent)
		{
            if (_channels.TryGetValue(topicId, out var channel) is false)
            {
                throw new Exception("없는 토픽");
            }

            IMessage result = new MemberMessage()
			{
				Author = GetMember(session),
				MessageId = channel.MessageId,
				Content = messageContent,
			};

            channel.Messages.Add(result);

			return result;
        }

        public ChatChannel CreateChannel(EchoServer.Dtos.ISession session)
		{
            ChatChannel result = new()
			{
				TopicId = _topicService.Subscript(session),
				Members = new() { GetMember(session) }
			};
			_channels.TryAdd(result.TopicId, result);
			return result;
        }

		public JoinMessage JoinChannel(EchoServer.Dtos.ISession session, ulong topicId)
		{
			if (_channels.TryGetValue(topicId, out var channel) is false)
			{
				throw new Exception("없는 토픽");
			}

			if (channel.Members.Where(o => o.SessionId == session.Uid).Any())
			{
				throw new Exception("이미 등록된 회원");
			}

			channel.Members.Add(GetMember(session));
			var result = new JoinMessage()
			{
				Author = GetMember(session),
				MessageId = channel.MessageId,
			};
            channel.Messages.Add(result);
            _topicService.Subscript(session, topicId);
			return result;
        }

        private void OnClose(ulong topicId)
		{
			_channels.Remove(topicId, out _);
		}
	}
}

