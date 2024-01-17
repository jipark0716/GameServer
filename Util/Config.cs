namespace Util;

public class Config
{
    private static Config? _instance;
    public static Config Default => _instance ??= CreateInstance();

    private static Config CreateInstance() => new();

    public OauthConfig DiscordOauthConfig = new()
    {
        GrantUrl = "https://discord.com/api/v10/oauth2/token",
        AuthorizeUrl = "https://discord.com/api/oauth2/authorize",
        ClientSecret = "5lGt-gdwvRzkO16CbLxhzHdhpNJZiT4k",
        BasicAuth = "OTU5MjMyNDI1MDc3MTE2OTY5OjVsR3QtZ2R3dlJ6a08xNkNiTHhoekhkaHBOSlppVDRr",
        ClientId = "959232425077116969",
        Scope = new(){ "identify" },
    };

    public readonly string JwtKey = "7bb223e4b914dbaf37a8eaa5fc8d9af8b2352380db0bdd3f1591468c698fbbfa74776fe1600a43c1de315cc3e595375392bc6d661f691a3c0f360cb5fb9c1b4a";
}

public class OauthConfig
{
    public required string GrantUrl { get; set; }
    public required string AuthorizeUrl { get; set; }
    public required string ClientSecret { get; set; }
    public required string BasicAuth { get; set; }
    public required string ClientId { get; set; }
    public required List<string> Scope { get; set; }
}