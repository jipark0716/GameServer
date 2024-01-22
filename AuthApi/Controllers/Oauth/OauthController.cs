using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Util;

namespace AuthApi.Controllers.Oauth;

[ApiController]
public abstract class OauthController(AuthConfig config, OauthConfig oauthConfig) : ControllerBase
{
    [HttpGet]
    public IActionResult Invoke()
    {
        NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

        queryString.Add("client_id", oauthConfig.ClientId);
        queryString.Add("response_type", "code");
        queryString.Add("redirect_uri", $"http://{HttpContext.Request.Host}{HttpContext.Request.Path}/redirect");
        queryString.Add("scope", string.Join("+", oauthConfig.Scope));
        
        return Redirect($"{oauthConfig.AuthorizeUrl}?{queryString}");
    }

    [HttpGet("redirect")]
    public async Task<IActionResult> CodeRedirect([FromQuery] string code)
    {
        Dictionary<string, string> postArgs = new();
        postArgs.Add("grant_type", "authorization_code");
        postArgs.Add("code", code);
        postArgs.Add("redirect_uri", $"http://{HttpContext.Request.Host}{HttpContext.Request.Path}");
        HttpClient client = new();
        HttpRequestMessage request = new(HttpMethod.Post, oauthConfig.GrantUrl);
        request.Content = new FormUrlEncodedContent(postArgs);
        request.Headers.Add("Authorization", $"Basic {oauthConfig.BasicAuth}");
        var response = await client.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return BadRequest("invalid code");
        }

        var body = await GetTokenBody(response);

        return Redirect($"jipark://login?code={GetToken(body)}");
    }

    private string GetToken(TokenBody body)
    {
        JwtHeader header = new(new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.JwtKey)),
            SecurityAlgorithms.HmacSha256));
        JwtPayload payload = new(
            issuer: null,
            audience: null,
            claims: new Claim[]
            {
                new("id", body.Id.ToString(), ClaimValueTypes.UInteger64),
                new("type", body.Type)
            },
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddHours(12));
        
        var token = new JwtSecurityToken(header, payload);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public abstract Task<TokenBody> GetTokenBody(HttpResponseMessage response);
}

public class TokenBody(ulong id, string type)
{
    public ulong Id { get; } = id;
    public string Type { get; } = type;
}