using QuivalCombatTestWPF.Colours;
using QuivalLogicEngine.Cards;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuivalCombatTestWPF;

public partial class CombatZone : UserControl
{
    public event EventHandler CardClicked;
    public event EventHandler PlayerZoneClicked;

    private Grid[] SummonSlots { get; set; }

    public CombatZone()
    {
        InitializeComponent();
        MouseLeftButtonDown += HandleClick;
        SummonSlots = [ SummonSlot1, SummonSlot2, SummonSlot3, SummonSlot4, SummonSlot5 ];
    }

    public void Highlight(bool highlight)
    {
        if (highlight)
        {
            Background = QuivalColour.HighlightColour;
            Opacity = 0.5;
        }
        else
        {
            Background = Brushes.Transparent;
            Opacity = 1;
        }
    }

    public void UpdateCombatZone(List<CreatureCard> cards)
    {
        foreach (var card in cards)
        {
            if (CardIsInASummonSlot(card.Id))
            {
                continue;
            }
            else
            {
                //Find an empty slot and summon the card
            }
        }
    }

    private bool CardIsInASummonSlot(int cardId)
    {
        foreach (var slot in SummonSlots)
        {
            if (slot.Children != null && slot.Children[0] is BoardCard bc && bc.CardId == cardId) 
                return true;
            else
                continue;
        }

        return false;
    }

    public bool CardIsSummonedByPlayer(BoardCard bc, Side side)
    {
        foreach (BoardCard child in CombatZones[(int)side].Children)
            if (child == bc)
                return true;

        return false;
    }

    public bool CardIsSummonedByPlayer(int cardId, Side side)
    {
        foreach (BoardCard child in CombatZones[(int)side].Children)
            if (child.CardId == cardId)
                return true;

        return false;
    }

    public BoardCard? GetBoardCard(int cardId)
    {
        foreach (var zone in CombatZones)
            foreach (BoardCard child in zone.Children)
            if (child.CardId == cardId)
                return child;

        return null;
    }

    public int GetNumberOfSummonedCards(Side side)
    {
        return CombatZones[(int)side].Children.Count;
    }

    public void ClearHighlightedCards()
    {
        foreach (var zone in CombatZones)
            foreach (BoardCard child in zone.Children)
            child.Overlay.Opacity = 0.0;
    }

    private void HandleClick(object obj, MouseButtonEventArgs args)
    {
        if (obj is BoardCard bc)
        {
            if (bc.HasActed)
            {
                return;
            }
            else
            {
                CardClicked?.Invoke(obj, args);
            }
        }
        else
        {
            PlayerZoneClicked?.Invoke(obj, args);
        }
    }
}
