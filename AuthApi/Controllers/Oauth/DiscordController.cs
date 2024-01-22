using System.Text.Json;
using System.Text.Json.Serialization;
using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Mvc;
using Util;

namespace AuthApi.Controllers.Oauth;

[Route("oauth/[controller]")]
public class DiscordController(AuthConfig config) : OauthController(config, config.DiscordOauthConfig)
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
    public required string AccessToken { get; init; }
    
    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }
    
    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; init; }
    
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; init; }
    
    [JsonPropertyName("scope")]
    public required string Scope { get; init; }
}