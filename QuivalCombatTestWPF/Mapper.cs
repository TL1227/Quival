using QuivalLogicEngine.Cards;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace QuivalCombatTestWPF
{
    internal static class Mapper
    {
        public static FullCard MapToFullCard(Card card)
        {
            FullCard fullCard = new();
            fullCard.CardNameLabel.Content = card.Name;
            fullCard.CardDescriptionLabel.Text = card.Description;
            fullCard.CostContent.Content = card.Cost;
            fullCard.CardBackground.Background = GetColor(card);
            fullCard.Tag = card;

            string imagePath = GetImagePath(card.Name);
            if (imagePath != "")
                fullCard.CardImage.Source = new BitmapImage(new Uri(imagePath));

            string uniqueId = "";
            if (card.UniqueId < 10)
                uniqueId += "00";
            else if (card.UniqueId < 100)
                uniqueId += "0";

            uniqueId += card.UniqueId.ToString();

            fullCard.GlobalId.Content = card.SetCode + uniqueId;

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
            string imagePath = GetImagePath(card.Name);



            if (imagePath != "")
                handcard.CardImage.Source = new BitmapImage(new Uri(imagePath));

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
            bc.MouseDown += onClick;

            Debug.WriteLine($"Mapper: card id {card.Id} has acted is {card.HasActed}");

            bc.CardNameLabel.Content = card.Name;
            bc.AttackLabel.Content = card.Attack;
            bc.HealthLabel.Content = card.CurrentHealth;
            bc.CardBackground.Background = GetColor(card);
            bc.Tag = card;

            string imagePath = GetImagePath(card.Name);
            if (imagePath != "")
                bc.CardImage.Source = new BitmapImage(new Uri(imagePath));

            if (card.CurrentHealth < card.Health)
                bc.HealthLabel.Foreground = Brushes.Red;

            return bc;
        }

        private static SolidColorBrush GetColor(Card card)
        {
            if (card is CreatureCard cc)
            {
                if (cc.Triggers.Count > 0)
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

        private static string GetImagePath(string cardName)
        {
            if (cardName == "Zap")
            {
                return "C:\\Users\\lavelle.t\\Projects\\Personal\\Quival\\Resources\\zap.png";
            }

            return "";
        }
    }
}
