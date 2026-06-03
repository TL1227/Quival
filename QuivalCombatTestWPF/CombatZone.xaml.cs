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

    public Grid[] SummonSlots { get; set; }

    public Side Side { get; set; }

    public CombatZone()
    {
        InitializeComponent();
        MouseLeftButtonDown += HandleClick;

        //NOTE: would it make more sense to just have one SummonGrid and have a bunch of children?
        //would it make it too hard to keep the layout of summoned cards fixed?
        SummonSlots = [ 
            SummonSlot1, SummonSlot2, SummonSlot3, SummonSlot4, SummonSlot5
        ];
    }

    public void Highlight(bool highlight)
    {
        Background = highlight ? QuivalColour.HighlightColour 
            : Brushes.Transparent;

        Opacity = highlight ? 0.5 : 1 ;
    }

    public void UpdateCombatZone(List<CreatureCard> cards)
    {
        //save current layout
        BoardCard?[] currentLayout = new BoardCard?[SummonSlots.Count()];
        for (int i = 0; i < currentLayout.Length; i++)
        {
            if (SummonSlots[i].Children[0] != null)
            {
                currentLayout[i] = (BoardCard)SummonSlots[i].Children[i];
            }
        }

        //remove items not in the card list
        for (int i = 0; i < currentLayout.Length; i++)
        {
            if (currentLayout[i] == null) 
                continue;

            var cardIds = cards.Select(c => c.Id);
            if (!cardIds.Contains(currentLayout[i]!.CardId))
            {
                currentLayout[i] = null;
            }
        }

        //update existing card health and summon new cards
        foreach (var card in cards)
        {
            var ids = currentLayout.Select(bc => bc.CardId);
            if(ids.Contains(card.Id))
            {
                UpdateCreatureHealth(card.Id, card.CurrentHealth);
            }
            else
            {
                for (int i = 0; i < currentLayout.Length; i++)
                {
                    if (currentLayout[i] == null)
                    {
                        currentLayout[i] = Mapper.MapToBoardCard(card, Side);
                        break;
                    }
                }
            }
        }

        //clear summon slots
        foreach (var slot in SummonSlots)
        {
            slot.Children.Clear();
        }

        //add in our new layout
        for (int i = 0; i < currentLayout.Length; i++)
        {
            SummonSlots[i].Children.Add(currentLayout[i]);
        }
    }


    private void UpdateCreatureHealth(int cardId, int currentHealth)
    {
        var bc = GetBoardCard(cardId);

        if (bc != null)
            bc.HealthLabel.Content = currentHealth;
    }


    public bool CardIsSummonedByPlayer(BoardCard bc, Side side)
    {
        foreach (var slot in SummonSlots)
            if (slot.Children[0] == bc)
                return true;

        return false;
    }

    public bool CardIsSummonedByPlayer(int cardId, Side side)
    {
        foreach (var slot in SummonSlots)
            if (slot.Children[0] is BoardCard bc && bc.CardId == cardId)
                return true;

        return false;
    }

    public BoardCard? GetBoardCard(int cardId)
    {
        foreach (var slot in SummonSlots)
            if (slot.Children[0] is BoardCard bc && bc.CardId == cardId)
                return bc;

        return null;
    }

    public int GetNumberOfSummonedCards(Side side)
    {
        int count = 0;

        foreach (var slot in SummonSlots)
            if (slot.Children[0] != null && slot.Children[0] is BoardCard bc)
                count++;

        return count;
    }

    public void ClearHighlightedCards()
    {
        foreach (var slot in SummonSlots)
            foreach (BoardCard child in slot.Children)
            child.Overlay.Opacity = 0.0;
    }

    public List<BoardCard> GetBoardCards()
    {
        var cards = SummonSlots.Where(s => s.Children[0] is BoardCard).ToList();

        List<BoardCard> boardCards = new();
        foreach(var card in cards)
        {
            if (card.Children[0] is BoardCard bc)
                boardCards.Add(bc);
        }

        return boardCards;
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
