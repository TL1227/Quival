using System.Windows.Controls;

namespace QuivalCombatTestWPF;

public class SummonSlot
{
    public Position Position { get; set; }

    private BoardCard? card;
    public BoardCard? Card
    {
        get => card;
        set
        {
            if (card != null)
                Canvas.Children.Remove(card);

            value.SetPos(Position);
            Canvas.Children.Add(value);
            card = value;
        }
    }

    public Canvas Canvas { get; set; }

    public SummonSlot(Canvas canvas)
    {
        Canvas = canvas;
    }

    void RemoveCard(BoardCard boardCard)
    {
        Canvas.Children.Remove(boardCard);
        card = null;
    }
}
