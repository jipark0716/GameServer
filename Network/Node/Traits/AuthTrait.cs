using Network.Attributes;
using Network.Packets;
using Util.Extensions;

namespace Network.Node.Traits;

public class AuthTrait(IGameNode node, JwtDecoder jwtDecoder) : BaseTrait(node, jwtDecoder)
{
    [Action(100)]
    public void Authorization([Author] Author author, [Jwt] AuthorizeRequestDto request)
    {
        if (author.UserId is not null) return;
        author.UserId = request.Id;
        author.Socket.SendAsync(new AuthorizeResponseDto
        {
            UserId = request.Id
        }.Encapsulation(101));
    }
}