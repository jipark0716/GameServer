using MagicMaze.Dto.GameObjects;

namespace MagicMaze.Dto;

public class TileCard(IGameObject?[] gameObjects, IWall?[] walls)
{
    public readonly IGameObject?[] GameObjects = gameObjects;
    public readonly IWall?[] Walls = walls;
}