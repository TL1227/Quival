using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuivalCombatTestWPF;

public partial class LayoutCanvas : UserControl
{
    public double CenterWidth { get; set; }
    public double SummonSlotsWidth { get; set; }
    public double SummonSlotsCenter { get; set; }
    public double SummonSlotsStartLeft { get; set; }
    public double SummonSlotPadding { get; set; }

    public Position[] OpponentHandSlots { get; set; }
    public Position OpponentBlockArea { get; set; }
    public Position[] OpponentSummonSlots { get; set; }
    public Position[] PlayerSummonSlots { get; set; }
    public Position PlayerBlockArea { get; set; }
    public Position[] PlayerHandSlots { get; set; }

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
        SummonSlotsStartLeft = CenterWidth - SummonSlotsCenter;
        double CenterHeight = Canvas.ActualHeight / 2;

        OpponentHandSlots = new Position[7];
        for (int i = 0; i < 7; i++)
        {
            OpponentHandSlots[i] = new();
            OpponentHandSlots[i].Left = (SummonSlotsStartLeft - 10)  + ((BoardCard.DefaultWidth) * i);
        }

        OpponentBlockArea = new();
        OpponentBlockArea.Top = Canvas.ActualHeight * 0.10;
        OpponentBlockArea.Left = CenterWidth - (BoardCard.DefaultWidth / 2);


        OpponentSummonSlots = new Position[5];
        for (int i = 0; i < 5; i++)
        {
            OpponentSummonSlots[i] = new();
            OpponentSummonSlots[i].Top = Canvas.ActualHeight * 0.25;
            OpponentSummonSlots[i].Left = SummonSlotsStartLeft + ((BoardCard.DefaultWidth + SummonSlotPadding) * i);
        }

        PlayerSummonSlots = new Position[5];
        for (int i = 0; i < 5; i++)
        {
            PlayerSummonSlots[i] = new();
            PlayerSummonSlots[i].Top = Canvas.ActualHeight * 0.55;
            PlayerSummonSlots[i].Left = SummonSlotsStartLeft + ((BoardCard.DefaultWidth + SummonSlotPadding) * i);
        }

        PlayerBlockArea = new();
        PlayerBlockArea.Top = Canvas.ActualHeight * 0.70;
        PlayerBlockArea.Left = CenterWidth - (BoardCard.DefaultWidth / 2);

        PlayerHandSlots = new Position[7];
        for (int i = 0; i < 7; i++)
        {
            PlayerHandSlots[i] = new();
            PlayerHandSlots[i].Left = (SummonSlotsStartLeft - 10)  + ((BoardCard.DefaultWidth) * i);
            PlayerHandSlots[i].Top = Canvas.ActualHeight - (HandCard.DefaultHeight * 0.5);
        }

        SummonSlots = [
            PlayerSummonSlots,
            OpponentSummonSlots
            ];

        BlockAreas = [
            PlayerBlockArea,
            OpponentBlockArea
            ];

        //TestLayout();
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

        for (int i = 0; i < PlayerHandSlots.Length; i++)
        {
            HandCard hc = new(-1);

            hc.SetPos(PlayerHandSlots[i]);
            Canvas.Children.Add(hc);
        }

        for (int i = 0; i < OpponentHandSlots.Length; i++)
        {
            OpponentHandCard hc = new();
            Canvas.SetTop(hc, 0);
            Canvas.SetLeft(hc, OpponentHandSlots[i].Left);
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
    public void ClearOpponentHand()
    {
        List<OpponentHandCard> handCards = new();
        foreach (var card in Canvas.Children)
        {
            if (card is OpponentHandCard hc)
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
