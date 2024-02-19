using MagicMaze.Enums;

namespace MagicMaze.Dto;

public interface IWall;
public class BasicWall : IWall;
public class LoopHole : IWall;
public class OpendWall : IWall;
public class SearchWall(CharacterType characterType) : IWall
{
    public readonly CharacterType CharacterType = characterType;
}