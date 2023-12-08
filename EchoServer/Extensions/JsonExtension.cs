using System.Text;
using System.Text.Json;

namespace EchoServer.Extensions
{
	public static class JsonExtension
	{
        public static byte[] ToBytes(this string source)
        {
            return Encoding.Default.GetBytes(source);
        }

        public static string ToJsonString<T>(this T source)
        {
            return JsonSerializer.Serialize(source);
        }

        public static byte[] ToJsonByte<T>(this T source)
        {
            return source.ToJsonString().ToBytes();
        }
    }
}