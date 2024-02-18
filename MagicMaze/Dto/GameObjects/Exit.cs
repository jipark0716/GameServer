using MagicMaze.Enums;

namespace MagicMaze.Dto.GameObjects;

public class Exit(CharacterType characterType) : IGameObject
{
    public readonly CharacterType CharacterType = characterType;
}