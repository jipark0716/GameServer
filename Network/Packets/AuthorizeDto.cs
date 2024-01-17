using System.Text.Json.Serialization;

namespace Network.Packets;

public class AuthorizeRequestDto
{
    [JsonPropertyName("id")]
    public required ulong Id { get; set; }
    
    [JsonPropertyName("type")]
    public required string Type { get; set; }
}

public class AuthorizeResponseDto(ulong userId)
{
    public ulong UserId { get; } = userId;
}