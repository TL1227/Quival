using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuivalCombatTestWPF.Colours;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using QuivalLogicEngine.Messages;

namespace QuivalCombatTestWPF
{
    public partial class MainWindow : Window
    {
        QuivalClient Client { get; set; }
        Control? SelectedCard { get; set; }
        private ClientGameState CurrentGameState { get; set; }

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
            ClickBlocker.MouseLeftButtonDown += ClickBlocker_MouseLeftButtonDown;
        }

        #region StateUpdates

        public void UpdateGameState(ClientGameState cgs)
        {
            CurrentGameState = cgs;
            UpdateUIFromGameState();
        }

        public void UpdateUIFromGameState()
        {
            var gs = CurrentGameState;

            CombatZone.UpdateCombatZone(gs.BoardState.SummonedCreatures[gs.PlayerState.Id], Side.Player);
            CombatZone.UpdateCombatZone(gs.BoardState.SummonedCreatures[gs.OpponentId], Side.Opponent);

            PlayerResources.HealthPoints.Content = gs.PlayerState.HealthPoints;
            OpponentResources.HealthPoints.Content = gs.OpponentHealthPoints;

            PlayerResources.ManaPoints.Content = gs.PlayerState.ManaPoints;
            OpponentResources.ManaPoints.Content = gs.OpponentManaPoints;

            if (gs.OpponentBlockCard != null && 
                gs.OpponentBlockCard is CreatureCard opponentBlocker)
            {
                OpponentBlockZone.AddCardToBlockZone(Mapper.MapToBoardCard(opponentBlocker));
            }

            if (gs.PlayerState.BlockingCreature != null && 
                gs.PlayerState.BlockingCreature is CreatureCard playerBlocker)
            {
                PlayerBlockZone.AddCardToBlockZone(Mapper.MapToBoardCard(playerBlocker));
            }

            UpdateHand(gs.PlayerState.Hand);

            string gameEvents = "";
            foreach (var gameEvent in gs.GameEvents)
                gameEvents += gameEvent + "\n";

            GameEventLog.Text = gameEvents;

            CastSpellButton.Content = "";

            UnselectAll();

            ResetHighlightColour();

            MessageRecieved();

            if (!PlayerCanMove())
            {
                Client.PlayCard(new BlankCard());
            }
        }

        private bool PlayerHasCardsSummoned()
        {
            return CurrentGameState.BoardState.SummonedCreatures[CurrentGameState.PlayerState.Id].Count > 0;
        }

        private bool PlayerHasEnoughManaToPlayACard()
        {
            foreach (var card in CurrentGameState.PlayerState.Hand)
            {
                if (card.Cost <= CurrentGameState.PlayerState.ManaPoints)
                    return true;
            }

            return false;
        }

        public bool PlayerCanMove()
        {
            if (PlayerHasCardsSummoned())
                return true;

            if (PlayerHasEnoughManaToPlayACard())
                return true;

            return false;
        }

        public void UpdateHand(List<Card> cards)
        {
            List<HandCard> hand = Mapper.MapToHandCards(cards);
            
            HandZone.ClearHand();
            HandZone.SetHand(hand);
        }

        public void UpdateCombatZone(List<CreatureCard> playerCreatures, List<CreatureCard> opponentCreatures)
        {
            CombatZone.UpdateCombatZone(playerCreatures, Side.Player);
            CombatZone.UpdateCombatZone(opponentCreatures, Side.Opponent);
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

        public void ResetHighlightColour()
        {
            QuivalColour.ChangetoBlueHighlights();
        }

        #endregion

        #region Clicking
        private void ClickBlocker_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CombatZone_PlayerZoneClicked(object? sender, EventArgs e)
        {
            if (SelectedCard != null && SelectedCard is HandCard hc)
            {
                Client.PlayCard(hc.CardId);
                QuivalColour.ChangetoPurpleHighlights();
                CastSpellButton.Content = "Summon";
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

                OpponentBlockZone.SetHighlighted(false);
                QuivalColour.ChangetoPurpleHighlights();
                CastSpellButton.Content = "Block";
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

                    PlayerBlockZone.SetHighlighted(false);
                    QuivalColour.ChangetoPurpleHighlights();
                    CastSpellButton.Content = "Attack";
                    SelectedCard = null;
                }
            }
        }

        private void HandZone_CardClicked(object? sender, EventArgs e)
        {
            if (sender is HandCard card)
            {
                //TODO: get this from some card lookup, not the client UI
                int cost = int.Parse(card.CostContent.Content.ToString()!);
                int mana = CurrentGameState.PlayerState.ManaPoints;

                if (cost > mana)
                    return;

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
            ClickBlocker.IsHitTestVisible = true;
        }

        public void MessageRecieved()
        {
            ClickBlocker.IsHitTestVisible = false;
        }
    }
}
