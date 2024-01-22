namespace Util.Config;

public class DatabaseConfig
{
    public required string ConnectionString { get; init; }
    public required string VersionString { get; init; }
    public int ConnectionPoolSize { get; init; } = 1024;
}