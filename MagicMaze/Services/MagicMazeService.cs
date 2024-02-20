using MagicMaze.Dto;
using MagicMaze.Enums;
using MagicMaze.Repositories;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Util.Extensions;

namespace MagicMaze.Services;

public class MagicMazeService(TileRepository tileRepository, MagicMazeRoomState state)
{
    public void Start()
    {
        state.Players.AddRange(state.Users.Values.Select(o => new Player(o)));
        GetTileCards(state.ScenarioNo).Each(state.TileCards.Enqueue);
        GetOperationCards(state.Users.Count).Shuffle().Each((o, i) => state.Players[i].Operations = o);
    }

    private IEnumerable<TileCard> GetTileCards(int scenarioNo)
    {
        var tileCount = scenarioNo switch
        {
            1 => 9,
            2 or 3 => 12,
            4 or 5 => 14,
            6 => 17,
            7 => 19,
            _ => 24
        };
        return Enumerable.Range(2, tileCount).Select(tileRepository.Get);
    }

    private IEnumerable<List<Operation>> GetOperationCards(int playerCount)
        => playerCount switch
        {
            2 => [
                [Operation.Escalator, Operation.Search, Operation.MoveBottom, Operation.MoveLeft],
                [Operation.Teleportation, Operation.MoveTop, Operation.MoveRight]
            ],
            3 => [
                [Operation.MoveTop, Operation.MoveRight],
                [Operation.Teleportation, Operation.MoveLeft],
                [Operation.Escalator, Operation.Search, Operation.MoveBottom]
            ],
            4 => [
                [Operation.Teleportation, Operation.MoveLeft],
                [Operation.MoveTop],
                [Operation.MoveBottom, Operation.Search],
                [Operation.MoveRight, Operation.Escalator]
            ],
            5 => [
                [Operation.Teleportation, Operation.MoveLeft],
                [Operation.MoveTop],
                [Operation.MoveBottom, Operation.Search],
                [Operation.MoveRight, Operation.Escalator],
                [Operation.MoveLeft]
            ],
            6 => [
                [Operation.Teleportation, Operation.MoveLeft],
                [Operation.MoveTop],
                [Operation.MoveBottom, Operation.Search],
                [Operation.MoveRight, Operation.Escalator],
                [Operation.MoveLeft],
                [Operation.MoveRight]
            ],
            7 => [
                [Operation.Teleportation, Operation.MoveLeft],
                [Operation.MoveTop],
                [Operation.MoveBottom, Operation.Search],
                [Operation.MoveRight, Operation.Escalator],
                [Operation.MoveLeft],
                [Operation.MoveRight],
                [Operation.MoveBottom]
            ],
            8 => [
                [Operation.Teleportation, Operation.MoveLeft],
                [Operation.MoveTop],
                [Operation.MoveBottom, Operation.Search],
                [Operation.MoveRight, Operation.Escalator],
                [Operation.MoveLeft],
                [Operation.MoveRight],
                [Operation.MoveBottom],
                [Operation.MoveTop]
            ],
            _ => throw new ArgumentOutOfRangeException(nameof(playerCount), $"playerCount {playerCount} is not support")
        };
}