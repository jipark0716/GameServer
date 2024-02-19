using MagicMaze.Dto;
using MagicMaze.Dto.GameObjects;
using MagicMaze.Enums;

namespace MagicMaze.Repositories;

public class TileRepository(Rule rule)
{
    public TileCard GetStartTile(int turned)
    {
        return new TileCard([
            new Pause(), null, null, new Teleportation(CharacterType.Wizard),
            null, null, null, new Teleportation(CharacterType.Warrior),
            new Teleportation(CharacterType.Dwarf), null, null, new Escalator(-1, 1, turned),
            new Teleportation(CharacterType.Elf), null, new Escalator(1, -1, turned), null
        ], [
            new BasicWall()
        ], true);
    }

// 벽
    // 0 none
    // 1 벽
    // 2, 3, 4, 5 탐색
    // 6 개구명
    //
// 오브젝트
    // 0 none
    // 1 Pause
    // 2, 3, 4, 5 Teleportation
    // 6, 7 Escalator
    // 8, 9, 10, 11 Item
    // 12, 13, 14, 15 Exit
    // 16 CCTV
}