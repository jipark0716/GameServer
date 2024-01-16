namespace Network.Packets;

public class AuthorizeRequestDto
{
    public required ulong UserId { get; set; }    
}

public class AuthorizeResponseDto(ulong userId)
{
    public ulong UserId { get; } = userId;
}