using EchoServer.Dtos;
using EchoServer.Queues;
using EchoServer.Services;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Boostrap.DI;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace EchoServer.Application
{
    public class JsonSocketApplication : ISocketApplication
    {
        protected DoubleBufferingQueue<ResponsePacket>? _responseQueue;
        protected JsonSerializerOptions? _jsonSerializerOptions;
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

        public void Init(DoubleBufferingQueue<ResponsePacket> responseQueue)
        {
            Init(responseQueue, new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
        }

        public void Init(DoubleBufferingQueue<ResponsePacket> responseQueue, JsonSerializerOptions jsonSerializerOptions)
        {
            _responseQueue = responseQueue;
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        public void OnMessage(Packet packet)
        {
            if (Actions.TryGetValue(packet.Header, out var action) is false)
            {
                return;
            }

            try {
                List<object?> parameters = new();
                foreach (var parameter in action.Method.GetParameters())
                {
                    if (parameter.ParameterType.IsAssignableFrom(typeof(Dtos.ISession)))
                    {
                        parameters.Add(packet.Session);
                    }
                    else
                    {
                        parameters.Add(JsonSerializer.Deserialize(packet.Body, parameter.ParameterType, _jsonSerializerOptions));
                    }
                }
                var controller = _serviceProvider.GetRequiredService(action.Controller);
                Enqueue(action.Method.Invoke(controller, parameters.ToArray()));
            }
            catch (Exception e)
            {
                
            }
        }

        public async void Enqueue(object? obj)
        {
            switch (obj)
            {
                case ResponsePacket packet:
                    Enqueue(packet);
                    break;
                case IEnumerable<ResponsePacket> packets:
                    Enqueue(packets);
                    break;
                case Task<ResponsePacket> packet:
                    Enqueue(await packet);
                    break;
                case Task<IEnumerable<ResponsePacket>> packets:
                    Enqueue(await packets);
                    break;
                case IAsyncEnumerable<ResponsePacket> packets:
                    await foreach (var packet in packets) Enqueue(packet);
                    break;
                default:
                    break;
            }
        }

        public void Enqueue(ResponsePacket packet)
        {
            (_responseQueue ?? throw new("resposne queue init 안됨")).EnQueue(packet);
        }

        public void Enqueue(IEnumerable<ResponsePacket> packets)
        {
            foreach (var packet in packets)
            {
                Enqueue(packet);
            }
        }
    }
}
