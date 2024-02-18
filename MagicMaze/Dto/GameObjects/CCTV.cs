namespace MagicMaze.Dto.GameObjects;

public class Cctv : IGameObject
{
    private bool _isUsed = false;

    public bool Use()
    {
        if (_isUsed)
            return false;

        return _isUsed = true;
    }
}