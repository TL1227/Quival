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
using QuivalCombatTestWPF.Colours;
using QuivalLogicEngine.Cards;

namespace QuivalCombatTestWPF
{
    public partial class MainWindow : Window
    {
        QuivalClient Client { get; set; }
        Control? SelectedCard { get; set; }
        bool RoundMessageSent { get; set; }

        int MaxSummonedCards = 5;

        public MainWindow()
        {
            InitializeComponent();
            Client = new QuivalClient(this);
            Client.ConnectToServer();

            HandZone.CardClicked += HandZone_CardClicked;
            CombatZone.CardClicked += CombatZone_CardClicked;
            CombatZone.PlayerZoneClicked += CombatZone_PlayerZoneClicked;
            PlayerBlockZone.ZoneClicked += PlayerBlockZone_ZoneClicked;
            OpponentBlockZone.ZoneClicked += OpponentBlockZone_ZoneClicked;

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
            CombatZone.UpdateCombatZone(playerCreatures, Side.Player);
            CombatZone.UpdateCombatZone(opponentCreatures, Side.Opponent);

            //unhighlight everything
            PlayerBlockZone.SetHighlighted(false);
            CombatZone.Highlight(false, Side.Player);
            CombatZone.Highlight(false, Side.Opponent);
        }

        public void UnselectAll()
        {
            PlayerBlockZone.SetHighlighted(false);
            OpponentBlockZone.SetHighlighted(false);

            CombatZone.Highlight(false, Side.Player);
            CombatZone.Highlight(false, Side.Opponent);
            CombatZone.ClearHighlightedCards();

            HandZone.ClearAllHighlightedCards();

            SelectedCard = null;
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
        private void CombatZone_PlayerZoneClicked(object? sender, EventArgs e)
        {
            if (SelectedCard != null && SelectedCard is HandCard hc)
            {
                Client.PlayCard(hc.CardId);
                QuivalColour.ChangetoPurpleHighlights();
                SpellStreamCastButton.Content = "Summon";
                SelectedCard = null;
            }
        }

        private void CombatZone_CardClicked(object? sender, EventArgs e)
        {
            if (sender is BoardCard card)
            {
                if (SelectedCard != null)
                {
                    CombatZone_PlayerZoneClicked(sender, e);
                }
                else if (CombatZone.CardIsSummonedByPlayer(card, Side.Player))
                {
                    UnselectAll();

                    card.Overlay.Opacity = 0.4;
                    SelectedCard = card;

                    PlayerBlockZone.SetHighlighted(true);
                    OpponentBlockZone.SetHighlighted(true);
                }
            }
        }

        //BlockZones
        private void PlayerBlockZone_ZoneClicked(object? sender, EventArgs e)
        {
            if (SelectedCard != null && SelectedCard is BoardCard bc)
            {
                Client.PlayBlock(bc.CardId);
                QuivalColour.ChangetoPurpleHighlights();
                SpellStreamCastButton.Content = "Block";
                SelectedCard = null;
            }
        }

        private void OpponentBlockZone_ZoneClicked(object? sender, EventArgs e)
        {
            if (SelectedCard != null && SelectedCard is BoardCard bc)
            {
                if (CombatZone.CardIsSummonedByPlayer(bc, Side.Player))
                {
                    Client.PlayAttack(bc.CardId);
                    QuivalColour.ChangetoPurpleHighlights();
                    SpellStreamCastButton.Content = "Attack";
                    SelectedCard = null;
                }
            }
        }

        private void HandZone_CardClicked(object? sender, EventArgs e)
        {
            if (sender is HandCard card)
            {
                UnselectAll();

                if (CombatZone.GetNumberOfSummonedCards(Side.Player) < MaxSummonedCards)
                {
                    card.Overlay.Opacity = 0.4;
                    SelectedCard = card;

                    CombatZone.Highlight(true, Side.Player);
                }
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
