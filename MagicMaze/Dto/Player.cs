using MagicMaze.Enums;
using Network.Packets;

namespace MagicMaze.Dto;

public class Player(Author author)
{
    public readonly Author Author = author;
    public List<Operation>? Operations;
}