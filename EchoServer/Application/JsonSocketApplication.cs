using EchoServer.Dtos;
using EchoServer.Queues;
using EchoServer.Services;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Boostrap.DI;
using System.Reflection;

namespace EchoServer.Application
{
    public class JsonSocketApplication : ISocketApplication
    {
        protected DoubleBufferingQueue<ResponsePacket>? _responseQueue;
        protected JsonSerializerOptions? _jsonSerializerOptions;
        protected TopicService? _topicService;
        protected readonly Dictionary<ushort, (Type Controller, MethodInfo Method)> Actions;
        protected readonly IServiceProvider _serviceProvider;

        public JsonSocketApplication(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Actions = new();
            foreach (var controller in AssamblyExtendsion.GetAllClasses<SocketControllerAttribute>())
            {
                foreach (var method in controller.GetMethods<ActionAttribute>())
                {
                    var actionAttribute = method.GetCustomAttribute<ActionAttribute>() ?? throw new("action 누락");
                    Actions.Add(actionAttribute.Id, (controller, method));
                }
            }
        }

        public void Init(DoubleBufferingQueue<ResponsePacket> responseQueue, TopicService topicService)
        {
            Init(responseQueue, topicService, new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
        }

        public void Init(DoubleBufferingQueue<ResponsePacket> responseQueue, TopicService topicService, JsonSerializerOptions jsonSerializerOptions)
        {
            _responseQueue = responseQueue;
            _topicService = topicService;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        public void OnMessage(Packet packet)
        {
            if (Actions.TryGetValue(packet.Header, out var action) is false)
            {
                return;
            }

            List<object?> parameters = new();
            foreach (var parameter in action.Method.GetParameters())
            {
                parameter.GetType();
            }
            var controller = _serviceProvider.GetRequiredService(action.Controller);
            action.Method.Invoke(controller, parameters.ToArray());

            JsonSerializer.Deserialize<string>(packet.Body, _jsonSerializerOptions);
        }

        public void Enqueue(ResponsePacket packet)
        {
            (_responseQueue ?? throw new("resposne queue init 안됨")).EnQueue(packet);
        }

        public void Enqueue(string payload, ITarget? target = null)
        {
            Enqueue(new ResponsePacket()
            {
                Target = target ?? new BroadcastTarget(),
                Payload = Encoding.UTF8.GetBytes(payload),
            });
        }

        public void Enqueue(object payload, ITarget? target = null)
        {
            Enqueue(
                JsonSerializer.Serialize(payload, _jsonSerializerOptions),
                target);
        }
    }
}
