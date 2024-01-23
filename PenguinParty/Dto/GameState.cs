namespace PenguinParty.Dto;

[Serializable]
public class GameState
{
    public bool IsStart;
    public Player[] Players = [];
    public readonly Board Board = new(8);
    public ushort Turn;
    public byte CurrentTurnPlayerId => (byte)(Turn % Players.Length);
    public Player CurrentTurnPlayer => Players[CurrentTurnPlayerId];
}