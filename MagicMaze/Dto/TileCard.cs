using MagicMaze.Dto.GameObjects;

namespace MagicMaze.Dto;

public class TileCard(Dictionary<byte, IGameObject> gameObjects, bool isStartTile)
{
    public readonly Dictionary<byte, IGameObject> GameObjects = gameObjects;
    public readonly bool IsStartTile = isStartTile;
}