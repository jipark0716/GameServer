namespace PenguinParty.Dto;

public class Cell
{
    public Card? Card;
}

public class Board
{
    public readonly Cell[][] Cells;
    private readonly Dictionary<(int, int), (Card?, Card?)> _submittable = [];
    private readonly int _size;
    private byte _submitCount;
    
    public Board(int size = 8)
    {
        _size = size;
        
        Cells = new Cell[size][];
        for (var y = 0; y < size; y++)
        {
            var xSize = size - y;
            Cells[y] = new Cell[xSize];
            for (var x = 0; x < xSize; x++)
            {
                Cells[y][x] = new Cell();
            }
        }
    }

    public bool Submit(int x, int y, Card card)
    {
        if (!ValidateSubmit(x, y, card)) return false;
        
        Cells[y][x].Card = card;
        _submitCount++;
        _submittable.Remove((x, y));

        // 좌우 싱크
        AsyncSubmittable(x - 1, y);
        AsyncSubmittable(x + 1, y);
        return true;
    }

    private void AsyncSubmittable(int x, int y)
    {
        if (y > _size || x > _size - y || x < 0)
        {
            return;
        }

        if (_submittable.TryGetValue((x, y), out _))
        {
            return;
        }
        
        if (Cells[y][x].Card is null)
        {
            if (y == 0)
            {
                AddSubmittable(x, y);
            }
            else
            {
                var card1 = Get(x, y - 1)?.Card;
                var card2 = Get(x + 1, y - 1)?.Card;
                if (card1 is not null && card2 is not null)
                {
                    AddSubmittable(x, y, card1, card2);
                }
            }
        }
        else
        {
            AsyncSubmittable(x, y + 1);
            AsyncSubmittable( x - 1, y + 1);
        }
    }

    private Cell? Get(int x, int y)
    {
        try
        {
            return Cells[y][x];
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }

    private void AddSubmittable(int x, int y, Card? card1 = null, Card? card2 = null)
    {
        if (card1 == card2)
        {
            card2 = null;
        }

        _submittable.TryAdd((x, y), (card1, card2));
    }

    private bool ValidateSubmit(int x, int y, Card card)
    {
        if (_submitCount == 0) return true;

        if (_submittable.TryGetValue((x, y), out var cards) is false)
        {
            return false;
        }

        if (cards.Item1 is null)
        {
            return true;
        }

        return card == cards.Item1 || card == cards.Item2;
    }
}