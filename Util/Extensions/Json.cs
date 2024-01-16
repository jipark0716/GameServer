using System.Text.Json;

namespace Util.Extensions;

public static class Json
{
    public static byte[] ToJsonByte(this object source, JsonSerializerOptions? options = null)
        => JsonSerializer.SerializeToUtf8Bytes(source, options);
    
    public static string ToJsonString(this object source, JsonSerializerOptions? options = null)
        => JsonSerializer.Serialize(source, options);

    public static byte[] Encapsuleation(this object source, ushort type, JsonSerializerOptions? options = null)
    {
        var body = source.ToJsonByte(options);
        return BitConverter.GetBytes(type).Merge(BitConverter.GetBytes((ushort)body.Length), body);
    }
    
    
}