using QuivalLogicEngine.Cards;
using System.Windows.Controls;

namespace QuivalCombatTestWPF
{
    public class NumberOfIntents
    {
        public Intent Intent { get; set; }
        public int Number { get; set; }
    }

    public partial class CardSelector : UserControl
    {
        private int i = 0;
        private List<NumberOfIntents> RequiredCards { get; set; } = new();
        private Dictionary<Intent, List<int>> SelectedCards { get; set; } = new();

        public CardSelector(List<NumberOfIntents> requiredCards)
        {
            InitializeComponent(); 

            RequiredCards = requiredCards;
            foreach (var cards in RequiredCards)
            {
                SelectedCards[cards.Intent] = new();
            }

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            CardSelectorLabel.Content = $"Select {RequiredCards[i].Number} cards for {RequiredCards[i].Intent}";
        }

        public Dictionary<Intent, List<int>>? SelectCard(int cardId)
        {
            SelectedCards[RequiredCards[i].Intent].Add(cardId);

            RequiredCards[i].Number--;

            if (RequiredCards[i].Number <= 0)
            {
                if (++i >= RequiredCards.Count)
                {
                    //TODO: We've finished selecting cards
                    return SelectedCards;
                }
            }

            UpdateLabel();

            return null;
        }
    }
}
