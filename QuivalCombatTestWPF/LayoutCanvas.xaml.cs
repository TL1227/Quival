using QuivalLogicEngine.Cards;
using System.Windows.Controls;

namespace QuivalCombatTestWPF;

public partial class LayoutCanvas : UserControl
{
    public double CenterWidth { get; set; }
    public double SummonSlotsWidth { get; set; }
    public double SummonSlotsCenter { get; set; }
    public double SummonSlotsStartLeft { get; set; }
    public double SummonSlotPadding { get; set; }
    public Position[] PlayerSummonSlots { get; set; }
    public Position[] OpponentSummonSlots { get; set; }
    public Position[] HandSlots { get; set; }
    public Position PlayerBlockArea { get; set; }
    public Position OpponentBlockArea { get; set; }

    public Position[][] SummonSlots { get; set; }
    public Position[] BlockAreas { get; set; }

    public LayoutCanvas()
    {
        InitializeComponent();
        Loaded += LayoutCanvas_Loaded;
    }

    private void LayoutCanvas_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        //card slots layout
        SummonSlotPadding = 60;
        CenterWidth = Canvas.ActualWidth / 2;
        SummonSlotsWidth = (BoardCard.DefaultWidth * 5) + (SummonSlotPadding * 4); //NOTE it's 4 paddings because there are 4 gaps between the 5 cards
        SummonSlotsCenter = SummonSlotsWidth / 2;
        SummonSlotsStartLeft = CenterWidth - SummonSlotsCenter - 60;

        double CenterHeight = Canvas.ActualHeight / 2;
        OpponentSummonSlots = new Position[5];
        for (int i = 0; i < 5; i++)
        {
            OpponentSummonSlots[i] = new();
            OpponentSummonSlots[i].Left = SummonSlotsStartLeft + ((BoardCard.DefaultWidth + SummonSlotPadding) * i);
            OpponentSummonSlots[i].Top = CenterHeight - BoardCard.DefaultHeight - 60;
        }

        OpponentBlockArea = new();
        OpponentBlockArea.Top = Canvas.ActualHeight * 0.14;
        OpponentBlockArea.Left = CenterWidth - (BoardCard.DefaultWidth / 2);

        PlayerSummonSlots = new Position[5];
        for (int i = 0; i < 5; i++)
        {
            PlayerSummonSlots[i] = new();
            PlayerSummonSlots[i].Left = SummonSlotsStartLeft + ((BoardCard.DefaultWidth + SummonSlotPadding) * i);
            PlayerSummonSlots[i].Top = CenterHeight - 60;
        }

        PlayerBlockArea = new();
        PlayerBlockArea.Top = Canvas.ActualHeight * 0.58;
        PlayerBlockArea.Left = CenterWidth - (BoardCard.DefaultWidth / 2);

        HandSlots = new Position[7];
        for (int i = 0; i < 7; i++)
        {
            HandSlots[i] = new();
            HandSlots[i].Left = SummonSlotsStartLeft + ((BoardCard.DefaultWidth + 8) * i);
            HandSlots[i].Top = Canvas.ActualHeight - HandCard.DefaultHeight - 10;
        }

        SummonSlots = [
            PlayerSummonSlots,
            OpponentSummonSlots
            ];

        BlockAreas = [
            PlayerBlockArea,
            OpponentBlockArea
            ];

        TestLayout();
    }

    private void TestLayout()
    {
        for (int i = 0; i < PlayerSummonSlots.Length; i++)
        {
            BoardCard bc = new()
            {
                Id = -1,
                HasActed = false,
            };

            bc.SetPos(PlayerSummonSlots[i]);
            Canvas.Children.Add(bc);
        }

        for (int i = 0; i < PlayerSummonSlots.Length; i++)
        {
            BoardCard bc = new()
            {
                Id = -1,
                HasActed = false,
            };

            bc.SetPos(OpponentSummonSlots[i]);
            Canvas.Children.Add(bc);
        }

        for (int i = 0; i < 2; i++)
        {
            BoardCard blockCard = new()
            {
                Id = -1,
                HasActed = false,
            };

            blockCard.SetPos(BlockAreas[i]);
            Canvas.Children.Add(blockCard);
        }

        for (int i = 0; i < HandSlots.Length; i++)
        {
            HandCard hc = new(-1);

            hc.SetPos(HandSlots[i]);
            Canvas.Children.Add(hc);
        }
    }

    //TODO: put this in the handzone class
    public void ClearHand()
    {
        List<HandCard> handCards = new();
        foreach (var card in Canvas.Children)
        {
            if (card is HandCard hc)
            {
                handCards.Add(hc);
            }
        }

        foreach (var card in handCards)
        {
            Canvas.Children.Remove(card);
        }
    }
}
