using Network;

namespace MagicMaze;

public class MagicMazeConfig
{
    public required NetworkConfig NetworkConfig { get; init; }
    public required string JwtKey { get; init; }
}