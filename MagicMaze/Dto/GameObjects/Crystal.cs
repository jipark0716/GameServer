namespace MagicMaze.Dto.GameObjects;

public class Crystal
{
    private bool _isUsed = false;

    public bool Use()
    {
        if (_isUsed)
            return false;

        return _isUsed = true;
    }
}