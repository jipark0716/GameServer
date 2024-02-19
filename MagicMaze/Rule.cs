namespace MagicMaze;

public class Rule
{
    public required TileCard[] TitleCards { get; init; }

    public class TileCard
    {
        public required IEnumerable<int> Objects { get; init; }
        public required IEnumerable<int> Walls { get; init; }
        public required bool IsStartCards { get; init; }
    }
}