namespace MagicMaze.Dto.GameObjects;

public class Escalator(int x, int y, int turned) : IGameObject
{
    // 타일카드 회전시 에스컬레이터 타는 방향 변경
    public readonly int MoveX = x * (turned is 0 or 1 ? 1 : -1);
    public readonly int MoveY = y * (turned is 0 or 2 ? 1 : -1);
}