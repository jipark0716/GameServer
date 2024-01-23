using Network;

namespace Chat;

public class ChatConfig
{
    public required NetworkConfig NetworkConfig { get; init; }
    public required string JwtKey { get; init; }
}