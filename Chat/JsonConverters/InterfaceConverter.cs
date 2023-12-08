using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using EchoServer.Extensions;

namespace Chat.JsonConverters
{
    public class InterfaceConverter<V, T> : JsonConverter<T>
        where T : class
        where V : notnull
    {
        private class TypeOnly<E> : IJsonOneofConverterable<E>
        {
            public required E Type { get; set; }
        }

        private readonly Dictionary<V, Type> _types;

        public InterfaceConverter(Dictionary<V, Type> types)
        {
            _types = types;
        }

        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonString = reader.GetString();
            if (jsonString is null)
            {
                return null;
            }
            var temp = JsonSerializer.Deserialize<TypeOnly<V>>(jsonString, options);
            if (temp is null || _types.TryGetValue(temp.Type, out var type) is false)
            {
                throw new Exception("unsupport type");
            }

            return JsonSerializer.Deserialize(jsonString, type, options) as T ?? throw new Exception("unsupport type");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteRawValue(value.ToJsonString());
        }
    }

    public interface IJsonOneofConverterable<T>
    {
        public T Type { get; }
    }
}