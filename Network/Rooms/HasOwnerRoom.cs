using Network.Attributes;
using Network.EventListeners;
using Network.Packets;

namespace Network.Rooms;

public abstract class HasOwnerRoom : Room
{
    private ulong _ownerId;
    protected readonly SimpleMiddleware IsOwnerMiddleware;

    protected HasOwnerRoom(ulong id, Author author, string name) : base(id, name)
    {
        IsOwnerMiddleware = new(o => o.UserId == _ownerId);
        _ownerId = (ulong)author.UserId!;
        Listener.AddAction(2001, nameof(ChangeOwner), IsOwnerMiddleware);
    }
    
    public override bool LeaveUser(ulong id)
    {
        var result = base.LeaveUser(id);

        if (result)
        {
            return true;
        }
        
        // 방장이 나가면 방장 변경
        if (id == _ownerId)
            _ownerId = Users.FirstOrDefault().Key;
        
        return false;
    }

    public void ChangeOwner([JsonBody] ulong id)
    {
        if (Users.Any(o => o.Key == id))
        {
            _ownerId = id;
        }
    }
}