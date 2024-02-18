using MagicMaze.Enums;

namespace MagicMaze.Dto.GameObjects;

public class Teleportation(CharacterType characterType) : IGameObject
{
    public readonly CharacterType CharacterType = characterType;
}