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
        QuivalClient Client { get; set; }
        Control? SelectedCard { get; set; }
        bool RoundMessageSent { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Client = new QuivalClient(this);
            Client.ConnectToServer();

            HandZone.CardClicked += HandZone_CardClicked;
            CombatZone.CardClicked += CombatZone_CardClicked;
            PlayerBlockZone.ZoneClicked += PlayerBlockZone_ZoneClicked;

            RoundMessageSent = false;
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

        public void UpdatePlayerBlockZone(CreatureCard? card)
        {
            if (card != null)
            {
                BoardCard bc = Mapper.MapToBoardCard(card);
                PlayerBlockZone.AddCardToBlockZone(bc);
            }
        }

        public void UpdateOpponentBlockZone(CreatureCard? card)
        {
            if (card != null)
            {
                BoardCard bc = Mapper.MapToBoardCard(card);
                OpponentBlockZone.AddCardToBlockZone(bc);
            }
        }

        #endregion

        #region Clicking
        private void PlayerBlockZone_ZoneClicked(object? sender, EventArgs e)
        {
            if (SelectedCard != null && SelectedCard is BoardCard bc)
            {
                //PlayerBlockZone.AddCardToBlockZone(bc);
                Client.PlayBlock(bc.CardId);
            }
        }

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
                if (Grid.GetRow(card) == (int)Side.Player)
                {
                    HandZone.DeselectAllCards();
                    CombatZone.DeselectAllCards();
                    SelectedCard = null;

                    card.Overlay.Opacity = 0.4;
                    SelectedCard = card;

                    PlayerBlockZone.BlockAreaHighlight.Opacity = 0.5;
                }
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

        public void MessageSent()
        {
            RoundMessageSent = true;
        }
        public void MessageRecieved()
        {
            RoundMessageSent = false;
        }
    }
}
