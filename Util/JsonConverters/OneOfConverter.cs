using System.Text.Json;
using System.Text.Json.Serialization;

namespace Util.JsonConverters;

public class OneOfConverter<T, TEnum> : JsonConverter<T>
    where T : class
    where TEnum : struct, IConvertible
{
    private readonly string _field;
    private readonly Dictionary<TEnum, Type> _types;

    public OneOfConverter(Dictionary<TEnum, Type> types, string field = "type")
    {
        _field = field;
        _types = types;

        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException($"{nameof(OneOfConverter<T, TEnum>)} must be enum argument");
        }
    }

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var content = reader.GetString() ?? throw new ArgumentException("reader dose not work");

        var temp = JsonSerializer.Deserialize<Dictionary<string, object>>(content);

        if (temp is null || temp.TryGetValue(_field, out var typeFile) is false)
        {
            return null;
        }

        if (_types.TryGetValue(Enum.Parse<TEnum>((string)typeFile), out var type))
        {
            return JsonSerializer.Deserialize(content, type) as T;
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}