using QuivalLogicEngine.Cards;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace QuivalCombatTestWPF
{
    internal static class Mapper
    {
        /*
        public static List<HandCard> MapToHandCards(List<Card> cards)
        {
            List<HandCard> result = new();

            foreach (var card in cards)
            {
                HandCard handcard = new(card.Id);
                handcard.CardNameLabel.Content = card.Name;
                handcard.CardDescriptionLabel.Text = card.Description;
                handcard.CostContent.Content = card.Cost;

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
        */

        public static FullCard MapToFullCard(Card card)
        {
            FullCard fullCard = new();
            fullCard.CardNameLabel.Content = card.Name;
            fullCard.CardDescriptionLabel.Text = card.Description;
            fullCard.CostContent.Content = card.Cost;
            fullCard.CardBackground.Background = GetColor(card);
            fullCard.Tag = card;

            fullCard.GlobalId.Content = card.UniqueId;

            if (card is CreatureCard cc)
            {
                fullCard.AttackLabel.Content = cc.Attack;
                fullCard.HealthLabel.Content = cc.Health;
                fullCard.Tag = cc;
            }
            else if (card is SpellCard sc)
            {
                fullCard.BottomGrid.Visibility = Visibility.Hidden;
                fullCard.Tag = sc;
            }

            return fullCard;
        }

        public static HandCard MapToHandCard(Card card)
        {
            HandCard handcard = new(card.Id);
            handcard.CardNameLabel.Content = card.Name;
            handcard.CardDescriptionLabel.Text = card.Description;
            handcard.CostContent.Content = card.Cost;
            handcard.CardBackground.Background = GetColor(card);
            handcard.Tag = card;

            if (card is CreatureCard cc)
            {
                handcard.AttackLabel.Content = cc.Attack;
                handcard.HealthLabel.Content = cc.Health;
                handcard.Tag = cc;
            }
            else if (card is SpellCard sc)
            {
                handcard.BottomGrid.Visibility = Visibility.Hidden;
                handcard.Tag = sc;
            }

            return handcard;
        }

        public static BoardCard MapToBoardCard(CreatureCard card, MouseButtonEventHandler onClick)
        {
            BoardCard bc = new()
            { 
                Id = card.Id,
                HasActed = card.HasActed,
            };
            bc.DebugId.Content = card.Id;
            bc.MouseLeftButtonDown += onClick;

            Debug.WriteLine($"Mapper: card id {card.Id} has acted is {card.HasActed}");

            bc.CardNameLabel.Content = card.Name;
            bc.AttackLabel.Content = card.Attack;
            bc.HealthLabel.Content = card.CurrentHealth;
            bc.CardBackground.Background = GetColor(card);

            if(card.CurrentHealth < card.Health )
                bc.HealthLabel.Foreground = Brushes.Red;

            return bc;
        }


        private static SolidColorBrush GetColor(Card card)
        {
            if (card is CreatureCard cc)
            {
                if (cc.Abilities.Count > 0)
                {
                    return Brushes.DarkOliveGreen;
                }
                else
                {
                    return Brushes.Coral;
                }
            }
            else if (card is SpellCard sc)
            {
                return Brushes.RoyalBlue;
            }

            return Brushes.Salmon;

            /*
            return attack switch
            {
                0 => Brushes.DarkOliveGreen,
                1 => Brushes.RoyalBlue,
                2 => Brushes.Teal,
                3 => Brushes.DarkOrange,
                4 => Brushes.Tomato,
                _ => Brushes.Salmon,
            };
            */
        }
    }
}
