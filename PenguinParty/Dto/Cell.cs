using System.Dynamic;
using Util.Extensions;

namespace PenguinParty.Dto;

public class Cell(byte x, byte y)
{
    public readonly byte X = x;
    public readonly byte Y = y;
    public Card? Card;
    public SubmitAbleType SubmitAble { get; private set; } = SubmitAbleType.None;
    public readonly Card?[] SubmitAbleCards = new Card?[2];

    public void SetSubmitAble(SubmitAbleType type)
    {
        ObjectDisposedException.ThrowIf(
            type == SubmitAbleType.Part,
            new InvalidOperationException("type part is not support"));

        SubmitAble = type;
        SubmitAbleCards.Clear();
    }

    public void SetPartSubmitAble(Card a, Card b)
    {
        SubmitAble = SubmitAbleType.Part;
        SubmitAbleCards[0] = a;
        SubmitAbleCards[1] = a != b ? b : null;
    }

    public void SetPartSubmitAble((Card a, Card b) c)
        => SetPartSubmitAble(c.a, c.b);
}

public enum SubmitAbleType
{
    None,
    All,
    Part
}