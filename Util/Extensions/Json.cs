using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Util.Extensions;

public static class Json
{
    public static byte[] ToJsonByte(this object source, JsonSerializerOptions? options = null)
        => JsonSerializer.SerializeToUtf8Bytes(source, options);
    
    public static string ToJsonString(this object source, JsonSerializerOptions? options = null)
        => JsonSerializer.Serialize(source, options);

    public static byte[] Encapsulation(this object source, ushort type, JsonSerializerOptions? options = null)
    {
        var body = source.ToJsonByte(options);
        return BitConverter.GetBytes(type).Merge(BitConverter.GetBytes((ushort)body.Length), body);
    }

    public static T? Serialize<T>(this IEnumerable<Claim> source) where T : new()
        => (T?)source.Serialize(typeof(T));
    
    public static object? Serialize(this IEnumerable<Claim> source, Type type)
    {
        var result = Activator.CreateInstance(type);

        if (result is null) return null;
        
        foreach (var row in source)
        {
            var propertyInfo = type
                .GetProperties()
                .FirstOrDefault(o => o.GetCustomAttribute<JsonPropertyNameAttribute>()?
                    .Name == row.Type);
            
            propertyInfo?
                .SetValue(result, Convert.ChangeType(row.Value, propertyInfo.PropertyType));
        }
        
        return result;
    }
}