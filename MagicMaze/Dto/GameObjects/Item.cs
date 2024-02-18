using MagicMaze.Enums;

namespace MagicMaze.Dto.GameObjects;

public class Item(CharacterType characterType) : IGameObject
{
    public readonly CharacterType CharacterType = characterType;
}