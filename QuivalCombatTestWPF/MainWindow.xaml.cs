using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using QuivalLogicEngine.Cards;

namespace QuivalCombatTestWPF
{
    public partial class MainWindow : Window
    {
        QuivalClient Client;
        Control? SelectedCard;

        public MainWindow()
        {
            InitializeComponent();
            Client = new QuivalClient(this);
            Client.ConnectToServer();

            HandZone.CardClicked += HandZone_CardClicked;
            CombatZone.CardClicked += CombatZone_CardClicked;
            PlayerBlockZone.ZoneClicked += PlayerBlockZone_ZoneClicked;
        }

        private void PlayerBlockZone_ZoneClicked(object? sender, EventArgs e)
        {
            if (SelectedCard != null && SelectedCard is BoardCard bc)
            {
                PlayerBlockZone.AddCardToBlockZone(bc);
            }
        }

        #region StateUpdates
        public void UpdateHand(List<Card> cards)
        {
            List<HandCard> hand = Mapper.MapToHandCards(cards);
            
            HandZone.ClearHand();
            HandZone.SetHand(hand);
        }

        public void UpdatePlayerHealth(int health)
        {
            PlayerResources.HealthPoints.Content = health;
        }

        public void UpdateOpponentHealth(int health)
        {
            OpponentResources.HealthPoints.Content = health;
        }

        public void UpdateCombatZone(List<CreatureCard> playerCreatures, List<CreatureCard> opponentCreatures)
        {
            CombatZone.ClearCombatZone();
            CombatZone.UpdatePlayerCombatZone(playerCreatures);
            CombatZone.UpdateOpponentCombatZone(opponentCreatures);
        }

        public void UpdatePlayerBlockZone(CreatureCard card)
        {
            BoardCard bc = Mapper.MapToBoardCard(card);
            PlayerBlockZone.AddCardToBlockZone(bc);
        }

        public void UpdateOpponentBlockZone(CreatureCard card)
        {
            BoardCard bc = Mapper.MapToBoardCard(card);
            OpponentBlockZone.AddCardToBlockZone(bc);
        }

        #endregion

        #region Clicking
        private void HandZone_CardClicked(object? sender, EventArgs e)
        {
            if (sender is HandCard card)
            {
                HandZone.DeselectAllCards();
                CombatZone.DeselectAllCards();
                SelectedCard = null;

                card.Overlay.Opacity = 0.4;
                SelectedCard = card;
            }
        }

        private void CombatZone_CardClicked(object? sender, EventArgs e)
        {
            if (sender is BoardCard card)
            {
                if (Grid.GetRow(card) == (int)Side.Opponent) 
                    return;

                HandZone.DeselectAllCards();
                CombatZone.DeselectAllCards();
                SelectedCard = null;

                card.Overlay.Opacity = 0.4;
                SelectedCard = card;
            }
        }

        private void SpellStreamCastButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCard is HandCard hc)
            {
                Client.PlayCard(hc.CardId);
            }
            else if (SelectedCard is BoardCard bc)
            {
                Client.PlayAttack(bc.CardId);
            }
        }
        #endregion
    }
}
