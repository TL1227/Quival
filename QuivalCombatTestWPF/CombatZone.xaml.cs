using QuivalCombatTestWPF.Colours;
using QuivalLogicEngine.Cards;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuivalCombatTestWPF;

public partial class CombatZone : UserControl
{
    public event EventHandler CardClicked;
    public event EventHandler PlayerZoneClicked;

    public SummonSlot[] SummonSlots { get; set; }

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

        foreach (var slot in SummonSlots)
        {
            slot.MouseLeftButtonDown += HandleClick;
        }
    }

    public void Highlight(bool highlight)
    {
        Background = highlight ? QuivalColour.HighlightColour 
            : Brushes.Transparent;

        Opacity = highlight ? 0.5 : 1 ;
    }

    public void UpdateCombatZone(List<CreatureCard> newCards)
    {
        //remove items not in the card list
        var newCardIds = newCards.Select(c => c.Id);
        foreach (var slot in SummonSlots)
        {
            if (slot.Card != null)
            {
                if (!newCardIds.Contains(slot.Card.Id))
                {
                    slot.ClearCard();
                }
            }
        }

        var summonedCardIds = GetIdsOfAllSummonedCards();
        foreach (var newCard in newCards)
        {
            //update existing card health 
            if (summonedCardIds.Contains(newCard.Id))
            {
                var slot = GetSlotFromCardId(newCard.Id);
                if (slot != null)
                {
                    slot.SetCard(Mapper.MapToBoardCard(newCard, Side.Player));
                }
            }
            else
            {
                //Summon new newCards
                foreach (var slot in SummonSlots)
                {
                    if (slot.Card == null)
                    {
                        slot.SetCard(Mapper.MapToBoardCard(newCard, Side));
                        break;
                    }
                }
            }
        }
    }

    private SummonSlot? GetSlotFromCardId(int cardId)
    {
        foreach (var slot in SummonSlots)
            if (slot.Card != null && slot.Card.Id == cardId)
                return slot;

        return null;
    }

    private List<int> GetIdsOfAllSummonedCards()
    {
        List<int> ids = new();
        foreach (var slot in SummonSlots)
        {
            if (slot.Card != null)
                ids.Add(slot.Card.Id);
        }

        return ids;
    }

    public void MarkActedCards()
    {
        foreach (var summonSlot in SummonSlots)
        {
            if (summonSlot.Card != null)
            {
                if (summonSlot.Card.HasActed)
                {
                    summonSlot.Card.MarkAsActed();
                    Debug.WriteLine($"MarkActedCards(): card id {summonSlot.Card.Id} has acted is {summonSlot.Card.HasActed}");
                }
            }
        }
    }

    public bool CardIsSummonedByPlayer(BoardCard bc)
    {
        foreach (var slot in SummonSlots)
            if (slot.Card == bc)
                return true;

        return false;
    }

    public bool CardIsSummonedByPlayer(int cardId)
    {
        return GetBoardCard(cardId) != null;
    }

    public BoardCard? GetBoardCard(int cardId)
    {
        foreach (var slot in SummonSlots)
            if (slot.Card != null && slot.Card.Id == cardId)
                return slot.Card;

        return null;
    }

    public int GetNumberOfSummonedCards()
    {
        return SummonSlots.Count(x => x.Card != null);
    }

    public void ClearHighlightedCards()
    {
        foreach (var slot in SummonSlots)
            if (slot.Card != null)
                slot.Card.Overlay.Opacity = 0.0;
    }

    public List<BoardCard> GetBoardCards()
    {
        List<BoardCard> list = SummonSlots.Where(s => s.Card != null).Select(c => c.Card).ToList()!;
        return list;
    }

    private void HandleClick(object obj, MouseButtonEventArgs args)
    {
        if (obj is SummonSlot ss)
        {
            if (ss.Card != null && ss.Card.HasActed == false)
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
