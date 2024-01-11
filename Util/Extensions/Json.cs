using System.Text.Json;

namespace Util.Extensions;

public static class Json
{
    public static byte[] ToJsonByte(this object source, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.SerializeToUtf8Bytes(source, options);
    }
}