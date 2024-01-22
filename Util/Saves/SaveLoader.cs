namespace Network.Saves;

public class SaveLoader<T>
    where T : class
{
}

public class Save
{
    public ulong Id;
    public byte GameType;
    public byte[] Body;
    public DateTime CreatedAt;
}