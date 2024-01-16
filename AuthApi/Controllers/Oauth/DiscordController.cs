using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers.Oauth;

[Route("oauth/[controller]")]
public class DiscordController(Config config) : OauthController(config, config.DiscordOauthConfig)
{
    public override async Task<TokenBody> GetTokenBody(HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<OauthResponse>(responseBody) ?? throw new InvalidOperationException("");
        var client = new DiscordRestClient();
        await client.LoginAsync(TokenType.Bearer, token.AccessToken);

        return new(client.CurrentUser.Id, "discord");
    }
}

public class OauthResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
    
    [JsonPropertyName("token_type")]
    public required string TokenType { get; set; }
    
    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; set; }
    
    [JsonPropertyName("scope")]
    public required string Scope { get; set; }
}