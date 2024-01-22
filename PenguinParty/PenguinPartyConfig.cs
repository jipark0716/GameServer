using Network;
using Util.Config;

namespace PenguinParty;

public class PenguinPartyConfig
{
    public required DatabaseConfigs Database { get; init; }
    public required NetworkConfig NetworkConfig { get; init; }
}

public class DatabaseConfigs
{
    public required DatabaseConfig Game { get; init; }
}