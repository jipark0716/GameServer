namespace MagicMaze.Dto.GameObjects;

public class Escalator(int x, int y) : IGameObject
{
    // 타일카드 회전시 에스컬레이터 타는 방향 변경
    public int X = x;
    public int Y = y;

    public void Turn(int time)
    {
        var currentTime = 1;
        while (time % 4 < currentTime)
        {
            if (int.Sign(X) == int.Sign(Y))
                Y *= -1;
            else
                X *= -1;
            currentTime++;
        }
    }
}