using MagicMaze.Dto;
using MagicMaze.Dto.GameObjects;
using MagicMaze.Enums;

namespace MagicMaze.Repositories;

public class TileRepository(Rule rule)
{
    public TileCard GetStartTile(int tileId, int turned)
        => GetStartTile(rule.TitleCards[tileId], turned);

    private TileCard GetStartTile(Rule.TileCard card, int turned)
        => new([

        ], [

        ], card.IsStartCards);

    private IWall? CreateWall(int id)
        => id switch
        {
            0 => null,
            1 => new BasicWall(),
            2 or 3 or 4 or 5 => new SearchWall((CharacterType)id - 2),
            6 => new LoopHole(),
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
        };

    private IGameObject? CreateObject(Rule.TileCard card, int index, int id, int turned)
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

// 오브젝트
    // 0 none
    // 1 Pause
    // 2, 3, 4, 5 Teleportation
    // 6, 7 Escalator
    // 8, 9, 10, 11 Item
    // 12, 13, 14, 15 Exit
    // 16 CCTV
}