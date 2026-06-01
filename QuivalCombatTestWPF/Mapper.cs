using QuivalLogicEngine.Cards;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuivalCombatTestWPF
{
    internal static class Mapper
    {
        public static List<HandCard> MapToHandCards(List<Card> cards)
        {
            List<HandCard> result = new();

            foreach (var card in cards)
            {
                HandCard handcard = new(card.Id);
                handcard.CardNameLabel.Content = card.Name;
                handcard.CardDescriptionLabel.Text = card.Description;
                handcard.CostContent.Content = card.Cost;

                Panel.SetZIndex(handcard, 500);

                if (card is CreatureCard cc)
                {
                    handcard.AttackLabel.Content = cc.Attack;
                    handcard.HealthLabel.Content = cc.Health;
                    handcard.CardBackground.Background = GetColor(cc.Attack);
                }


                result.Add(handcard);
            }

            return result;
        }

        public static BoardCard MapToBoardCard(CreatureCard card, Side side)
        {
            BoardCard bc = new()
            { 
                CardId = card.Id,
                HasActed = card.HasActed,
                Side = side,
            };

            bc.CardNameLabel.Content = card.Name;
            bc.AttackLabel.Content = card.Attack;
            bc.HealthLabel.Content = card.CurrentHealth;
            bc.CardBackground.Background = GetColor(card.Attack);

            if(card.CurrentHealth < card.Health )
                bc.HealthLabel.Foreground = Brushes.Red;

            return bc;
        }

        private static SolidColorBrush GetColor(int attack)
        {
            return attack switch
            {
                0 => Brushes.DarkOliveGreen,
                1 => Brushes.RoyalBlue,
                2 => Brushes.Teal,
                3 => Brushes.DarkOrange,
                4 => Brushes.Tomato,
                _ => Brushes.Salmon,
            };
        }
    }
}
