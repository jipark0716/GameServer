using Network;
using Network.Node;
using Network.Node.Traits;
using Network.Rooms;

namespace Chat;

public class NodeFactory(JwtDecoder jwtDecoder, IRoomRepository roomRepository)
{
    public IGameNode Create()
    {
        IGameNode result = new BasicGameNode();
        result = new AuthTrait(result, jwtDecoder);
        result = new RoomTrait(result, jwtDecoder, roomRepository);
        return new GameNodeLogger(result);
    }
}