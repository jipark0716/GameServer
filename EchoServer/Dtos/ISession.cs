namespace EchoServer.Dtos
{
    public interface ISession
    {
        public ulong Uid { get; }
    }

    public abstract class Session : ISession
    {
        public ulong Uid { get; } 

        public Session(ulong uid)
        {
            Uid = uid;
        }
    }
}
