using Network.Attributes;
using Network.EventListeners;
using Network.Packets;
using Network.Rooms;
using PenguinParty.Dto;
using PenguinParty.Packets;
using PenguinParty.Repositories;
using Serilog;
using Util.Extensions;

namespace PenguinParty;

public class PenguinPartyRoom : HasOwnerRoom
{
    private readonly GameState _gameState = new();
    private readonly CardRepository _cardRepository;
    
    public PenguinPartyRoom(
        ulong id,
        Author author,
        string name,
        CardRepository cardRepository) : base(id, author, name)
    {
        _cardRepository = cardRepository;
        
        SimpleMiddleware isPreStartMiddleware = new(_ => !_gameState.IsStart); // 시작전인지 검사
        SimpleMiddleware isStartedMiddleware = new(_ => _gameState.IsStart); // 게임중인지 검사
        SimpleMiddleware isCurrentTurnPlayerMiddleware = new(o => _gameState.CurrentTurnPlayer.UserId == o.UserId); // 요청자의 차례인지 검사

        Listener.Instance = this;
        Listener.AddAction(1001, nameof(ChangeSetting), isPreStartMiddleware, IsOwnerMiddleware);
        Listener.AddAction(1002, nameof(Start), isPreStartMiddleware, IsOwnerMiddleware);
        Listener.AddAction(1003, nameof(SubmitCard), isStartedMiddleware, isCurrentTurnPlayerMiddleware);
        Listener.AddAction(1003, nameof(SkipTurn), isStartedMiddleware, isCurrentTurnPlayerMiddleware);
    }

    public void SubmitCard([JsonBody] SubmitCardRequest request)
    {
        Cell[] floor;
        Cell cell;
        try
        {
            floor = _gameState.Board[request.Y];
            cell = floor[request.X];
        }
        catch(IndexOutOfRangeException e)
        {
            Log.Information(e, "보드 밖에 접근 x:{x} y:{y}", request.X, request.Y);
            return;
        }
        
        var card = _gameState.CurrentTurnPlayer.Cards[request.CardIndex];

        if (ValidateSubmitCard(cell, card) is false)
        {
            return;
        }

        SubmitCardAction(card, floor, cell);
        TurnEnd();
    }

    private void SubmitCardAction(Card card, Cell[] floor, Cell cell)
    {
        // 카드 놓기
        cell.Card = card;
        cell.SetSubmitAble(SubmitAbleType.None);
        
        // 놓을 수 있는곳 업데이트
        if (floor.Length == 1)
        {
            return;
        }
        
        if (_gameState.Turn == 0)
        {
            // 첫턴이면 놓은 위치 좌우면 놓을 수 있음
            _gameState.Board[cell.Y].Each((o, i) =>
            {
                o.SetSubmitAble(Math.Abs(cell.X - i) == 1 ? SubmitAbleType.None : SubmitAbleType.All);
            });
            return;
        }
    
        // 왼쪽 처리
        if (cell.X != 0)
            SyncSubmitAble(cell, floor, false);
        
        // 오른쪽 처리
        if (cell.X + 1 < floor.Length)
            SyncSubmitAble(cell, floor, true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell">변경된 셀</param>
    /// <param name="floor">변경된 셀이 포함된 층</param>
    /// <param name="right"></param>
    private void SyncSubmitAble(Cell cell, Cell[] floor, bool right)
    {
        // 방향 조정
        var targetCell = floor[cell.X + (right ? 1: -1)];

        // 카드가 이미 있으면 그 위층에 놓을 수 있음
        if (targetCell.Card is not null)
        {
            var upperCell = _gameState.Board[cell.Y + 1][cell.X - (right ? 0: -1)];
            upperCell.SetPartSubmitAble(cell.Card!, targetCell.Card);
        }
        
        // 1층 이면 아무카드나 넣을 수 있음
        if (cell.Y == 0)
        {
            targetCell.SetSubmitAble(SubmitAbleType.All);
        }
        else
        {
            targetCell.SetPartSubmitAble(GetLowerCards((byte)(cell.X + (right ? 1: -1)), cell.Y));
        }
    }

    private (Cell, Cell) GetLowerCells(byte x, byte y)
    {
        var lowerFloor = _gameState.Board[y - 1];
        return (lowerFloor[x], lowerFloor[x + 1]);
    }

    private (Card, Card) GetLowerCards(byte x, byte y)
    {
        var result = GetLowerCells(x, y);
        return (result.Item1.Card, result.Item2.Card)!;
    }

    /// <summary>
    /// 카드 놓을 수 있는지 검사
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="card"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool ValidateSubmitCard(Cell cell, Card card) =>
        cell.SubmitAble switch
        {
            SubmitAbleType.All => true,
            SubmitAbleType.None => false,
            SubmitAbleType.Part => cell.SubmitAbleCards.Any(o => o == card),
            _ => throw new ArgumentOutOfRangeException()
        };

    public void SkipTurn([JsonBody] SkipTurnRequest request)
    {
        _gameState.SkipCount++;
        TurnEnd(false);
    }

    private void TurnEnd(bool clearSkipCount = true)
    {
        if(clearSkipCount)
            _gameState.SkipCount = 0;
        _gameState.Turn++;

        if (_gameState.SkipCount != _gameState.Players.Length)
            return;
        
        RoundEnd();
        RoundStart();
    }

    public void ChangeSetting([JsonBody] GameSetting setting)
    {
        _gameState.GameSetting = setting;
        Send(setting.Encapsuleation(1003));
    }

    public void Start()
    {
        _gameState.IsStart = true;
        _gameState.Players = Users
            .Select(o => new Player(o.Key))
            .ToArray();
        RoundStart();
    }

    private byte GetCardCount()
    {
        var cardCount = _gameState.GameSetting.CardCount;
        return (byte)(cardCount - cardCount % _gameState.Players.Length);
    }

    private void ShuffleCard()
    {
        var hands = _cardRepository .Get(GetCardCount())
            .Shuffle()
            .Chunk(_gameState.Players.Length)
            .ToArray();
        
        foreach (var (player, i) in _gameState.Players.WithIndex())
        {
            player.Cards.AddRange(hands[i]);
        }
    }

    private void ClearBoard()
    {
        _gameState.Board.Each((row, i) =>
            row.Each(o =>
            {
                o.Card = null;
                o.SetSubmitAble(i == 0 ? SubmitAbleType.All : SubmitAbleType.None); // 1층만 놓을 수 있음
            }));
            
    }

    private void RoundEnd()
    {
        _gameState.StartPlayerId = _gameState.CurrentTurnPlayerId; // 다음라운드 시작 플레이어
        _gameState.Turn = 0;
        _gameState.CurrentRound++;
        _gameState.Players.Each(o =>
        {
            o.Score += (byte)o.Cards.Count;
            o.Cards.Clear();
        });
        ClearBoard();
    }

    private void RoundStart()
    {
        _gameState.SkipCount = 0;
        ShuffleCard();
    }
}