namespace PenguinParty.Dto;

public class GameState
{
    public GameSetting GameSetting = new();
    public bool IsStart;
    public byte CurrentRound;
    public Player[] Players = [];
    public ushort Turn;
    public byte StartPlayerId;
    public byte CurrentTurnPlayerId => (byte)((Turn + StartPlayerId) % Players.Length);
    public Player CurrentTurnPlayer => Players[CurrentTurnPlayerId];

    public readonly Cell[][] Board = Enumerable.Range(1, 8)
        .Reverse()
        .Select(i
            => Enumerable.Range(1, i)
                .Select(o => new Cell((byte)o, (byte)i))
                .ToArray())
        .ToArray();

    public sbyte SkipCount;
}