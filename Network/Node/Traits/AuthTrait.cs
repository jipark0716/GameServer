using Network.Attributes;
using Network.Packets;
using Util.Extensions;

namespace Network.Node.Traits;

public class AuthTrait(IGameNode node, JwtDecoder jwtDecoder) : BaseTrait(node, jwtDecoder)
{
    [Action(100)]
    public void Authorization([Author] Author author, [Jwt] AuthorizeRequestDto request)
    {
        author.UserId = request.Id;
        AuthorizeResponseDto response = new(request.Id);
        author.Socket.SendAsync(response.Encapsulation(101));
    }
}