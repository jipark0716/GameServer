namespace NetworkGateway.Session;

public class Session : ISession
{
    public required byte ServerId { get; set;  }
    public required ulong SessionId { get; set; }
}
