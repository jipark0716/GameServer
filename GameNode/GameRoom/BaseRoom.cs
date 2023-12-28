namespace GameNode.GameRoom;

public abstract class BaseRoom(int id)
{
    public readonly int Id = id;
    protected List<Session.Session> _userIds = [];

    public virtual void AddUser(Session.Session session) => AddUser(session);

    public virtual void OnDisconnect(Session.Session session)
    {

    }

    public virtual void CLose()
    {

    }
}
