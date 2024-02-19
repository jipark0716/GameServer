using MagicMaze.Dto.GameObjects;

namespace MagicMaze.Dto;

public class TileCard(IGameObject[] gameObjects, IWall[] walls, bool isStartTile)
{
    public readonly IGameObject[] GameObjects = gameObjects;
    public readonly IWall[] Walls = walls;
    public readonly bool IsStartTile = isStartTile;
}