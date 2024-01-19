using Network.Attributes;
using Network.EventListeners;
using Network.Packets;

namespace Network.Rooms;

public abstract class HasOwnerRoom : Room
{
    protected ulong OwnerId;
    protected readonly SimpleMiddleware IsOwnerMiddleware;

    protected HasOwnerRoom(ulong id, Author author, string name) : base(id, name)
    {
        IsOwnerMiddleware = new(o => o.UserId == OwnerId);
        OwnerId = (ulong)author.UserId!;
        Listener.AddAction(2001, nameof(ChangeOwner), IsOwnerMiddleware);
    }
    
    public override bool LeaveUser(ulong id)
    {
        var result = base.LeaveUser(id);
        
        // 방장이 나가면 방장 변경
        if (id == OwnerId)
            OwnerId = Users.FirstOrDefault().Key;
        
        return result;
    }

    public void ChangeOwner([JsonBody] ulong id)
    {
        if (Users.Any(o => o.Key == id))
        {
            OwnerId = id;
        }
    }
}