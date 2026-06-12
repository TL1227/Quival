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

    public BoardCard?[] SummonedCards { get; set; } = new BoardCard[5];

    public CombatZone()
    {
        InitializeComponent();
        MouseLeftButtonDown += HandleClick;
    }

    public void Highlight(bool highlight)
    {
        Background = highlight ? QuivalColour.HighlightColour 
            : Brushes.Transparent;

        Opacity = highlight ? 0.5 : 1 ;
    }

    public void UpdateCombatZone(List<CreatureCard> newCards, LayoutCanvas layout, Position[] slotPositions, MouseButtonEventHandler onClick)
    {
        //remove items not in the card list
        var newCardIds = newCards.Select(c => c.Id);
        for (int i = 0; i < 5; i++)
        {
            var summonedCard = SummonedCards[i];
            if (summonedCard != null)
            {
                if (!newCardIds.Contains(summonedCard.Id)) {

                    layout.Canvas.Children.Remove(summonedCard);
                    SummonedCards[i] = null;
                }
            }
        }

        var summonedCardIds = GetIdsOfAllSummonedCards();
        foreach (var newCard in newCards)
        {
            //update existing card health 
            if (summonedCardIds.Contains(newCard.Id))
            {
                int i = GetSlotIndexFromCardId(newCard.Id);
                if (SummonedCards[i] != null)
                {
                    SummonedCards[i]!.AttackLabel.Content = newCard.Attack;
                    SummonedCards[i]!.HealthLabel.Content = newCard.CurrentHealth;
                    SummonedCards[i]!.HasActed = newCard.HasActed;

                    if (newCard.AttackBuff <= 0)
                        SummonedCards[i]!.AttackLabel.Foreground = Brushes.Black;

                    if (newCard.CurrentHealth < newCard.Health)
                        SummonedCards[i]!.HealthLabel.Foreground = Brushes.Red;
                }
            }
            else
            {
                //Summon new newCards
                for (int i = 0; i < 5;i++)
                {
                    if (SummonedCards[i] == null)
                    {
                        SummonedCards[i] = Mapper.MapToBoardCard(newCard, onClick);
                        SummonedCards[i]!.MouseLeftButtonDown += HandleClick;
                        layout.Canvas.Children.Add(SummonedCards[i]);
                        SummonedCards[i]!.SetPos(slotPositions[i]);

                        break;
                    }
                }
            }
        }
    }

    public int RemoveCardFromZone(int cardId, LayoutCanvas layout)
    {
        for (int i = 0; i < SummonedCards.Length; i++)
        {
            if (SummonedCards[i] != null && SummonedCards[i]!.Id == cardId)
            {
                layout.Canvas.Children.Remove(SummonedCards[i]);
                SummonedCards[i] = null;
                return i;
            }
        }

        return -1;
    }
    public int AddCardToNextFreeSlot(BoardCard card, LayoutCanvas layout)
    {
        int i = GetNextFreeSummonSlotIndex();
        if (i >= 0)
        {
            card.MouseLeftButtonDown += HandleClick;
            layout.Canvas.Children.Add(card);
            SummonedCards[i] = card;
        }

        return i;
    }

    private int GetSlotIndexFromCardId(int cardId)
    {
        for (int i = 0; i < SummonedCards.Length; i++)
            if (SummonedCards[i] != null && SummonedCards[i]!.Id == cardId)
                return i;

        return -1;
    }

    private List<int> GetIdsOfAllSummonedCards()
    {
        List<int> ids = new();
        foreach (var card in SummonedCards)
        {
            if (card != null)
                ids.Add(card.Id);
        }

        return ids;
    }


    public void MarkActedCards()
    {
        foreach (var summonedCard in SummonedCards)
        {
            if (summonedCard != null)
            {
                if (summonedCard.HasActed)
                {
                    summonedCard.MarkAsActed(true);
                }
            }
        }
    }


    public bool CardIsSummonedByPlayer(BoardCard bc)
    {
        foreach (var card in SummonedCards)
            if (card == bc)
                return true;

        return false;
    }

    public bool CardIsSummonedByPlayer(int cardId)
    {
        return GetBoardCard(cardId) != null;
    }

    public BoardCard? GetBoardCard(int cardId)
    {
        foreach (var summonedCard in SummonedCards)
            if (summonedCard != null && summonedCard.Id == cardId)
            {
                if (Canvas.GetTop(summonedCard) == double.NaN)
                {
                    Debug.WriteLine("What the fuck!!?!?!");
                }

                return summonedCard;
            }

        return null;
    }

    public int GetNumberOfSummonedCards()
    {
        return SummonedCards.Count(x => x != null);
    }

    public void ClearHighlightedCards()
    {
        foreach (var card in SummonedCards)
        {
            if (card != null )
            {
                card.MarkSelected(false);

                if (!card.HasActed)
                    card.Overlay.Opacity = 0.0;
            }

        }
    }

    public List<BoardCard> GetBoardCards()
    {
        List<BoardCard> list = SummonedCards.Where(s => s != null).Select(c => c).ToList()!;
        return list;
    }

    public int GetNextFreeSummonSlotIndex()
    {
        for (int i = 0; i < SummonedCards.Length; i++)
        {
            if (SummonedCards[i] == null) 
            {
                return i;
            } 
        }

        return -1;
    }

    private void HandleClick(object obj, MouseButtonEventArgs args)
    {
        if (obj is BoardCard card)
        {
            if (card != null)
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
