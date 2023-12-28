using Common.Queues;
using GameNode.Listener;
using GameNode.Packets;

namespace GameNode.PenguinParty;

public class GameListener(IQueue<ClientMessage> queue) : BaseListener(queue)
{
}
