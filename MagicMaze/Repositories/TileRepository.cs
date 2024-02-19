using MagicMaze.Dto;
using MagicMaze.Dto.GameObjects;
using MagicMaze.Enums;

namespace MagicMaze.Repositories;

public class TileRepository(Rule rule)
{
    public TileCard GetStartTile(int tileId, int turned)
        => GetStartTile(rule.TitleCards[tileId], turned);

    private TileCard GetStartTile(Rule.TileCard card, int turned)
        => new(
            card.Objects.Select((o, i) => CreateObject(card, o, i, turned)).ToArray(),
            card.Walls.Select(CreateWall).ToArray());

    private IWall? CreateWall(int id)
        => id switch
        {
            0 => null,
            1 => new BasicWall(),
            2 or 3 or 4 or 5 => new SearchWall((CharacterType)id - 2),
            6 => new LoopHole(),
            7 => new OpendWall(),
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
        };

    private IGameObject? CreateObject(Rule.TileCard card, int id, int index, int turned)
        => id switch
        {
            0 => null,
            1 => new Pause(),
            2 or 3 or 4 or 5 => new Teleportation((CharacterType)id - 2),
            6 or 7 => CreateEscalator(card, index, id, turned),
            8 or 9 or 10 or 11 => new Item((CharacterType)id - 8),
            12 or 13 or 14 or 15 => new Exit((CharacterType)id - 12),
            16 => new Cctv(),
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
        };

    private Escalator CreateEscalator(Rule.TileCard card, int index, int id, int turned)
    {
        foreach (var obj in card.Objects.Select((o, i) => (o, i)))
        {
            if (id != obj.o || index == obj.i) continue;

            var x = index / 4 - obj.i / 4;
            var y = index % 4 - obj.i % 4;
            if (x >= 0) return new(x, y, turned);

            x += 4;
            y -= 1;
            return new(x, y, turned);
        }

        throw new Exception("gameRule.json 파일 잘못됨");
    }
}