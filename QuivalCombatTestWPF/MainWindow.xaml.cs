using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using QuivalCombatTestWPF.Colours;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using QuivalServer;

namespace QuivalCombatTestWPF
{
    public partial class MainWindow : Window
    {
        QuivalClient Client { get; set; }
        Control? SelectedCard { get; set; }
        private ClientGameState CurrentGameState { get; set; }

        int MaxSummonedCards = 5;

        public int? MyPlayerId { get; set; } = null;

        public MainWindow()
        {
            InitializeComponent();

            if (Environment.GetCommandLineArgs().Contains("--animation-test"))
            {
                ClientGameState cgs = new ClientGameState();
                cgs.BoardState = new();
                cgs.GameEvents = new();

                cgs.BoardState.SummonedCreatures[0] = new()
                {
                    new CreatureCard(1, 3, 3, 1){ Name = "Animation Test" },
                    new CreatureCard(1, 3, 3, 1){ Name = "Animation Test" },
                    new CreatureCard(1, 3, 3, 1){ Name = "Animation Test" },
                    new CreatureCard(1, 3, 3, 1){ Name = "Animation Test" },
                    new CreatureCard(1, 3, 3, 1){ Name = "Animation Test" },
                };

                cgs.BoardState.SummonedCreatures[1] = new()
                {
                    new CreatureCard(1, 2, 3, 1){ Name = "Animation Test" },
                    new CreatureCard(1, 2, 3, 1){ Name = "Animation Test" },
                    new CreatureCard(1, 2, 3, 1){ Name = "Animation Test" },
                    new CreatureCard(1, 2, 3, 1){ Name = "Animation Test" },
                    new CreatureCard(1, 2, 3, 1){ Name = "Animation Test" },
                };

                UpdateGameState(cgs);
            }
            else
            {
                Client = new QuivalClient(this);
                Client.ConnectToServer();
            }

            PlayerBlockZone.Side = Side.Player;
            OpponentBlockZone.Side = Side.Opponent;

            HandZone.CardClicked += HandZone_CardClicked;
            CombatZone.CardClicked += CombatZone_CardClicked;
            CombatZone.PlayerZoneClicked += CombatZone_PlayerZoneClicked;
            PlayerBlockZone.ZoneClicked += PlayerBlockZone_ZoneClicked;
            OpponentBlockZone.ZoneClicked += OpponentBlockZone_ZoneClicked;
            ClickBlocker.MouseLeftButtonDown += ClickBlocker_MouseLeftButtonDown;

        }

        #region StateUpdates

        public async void UpdateGameState(ClientGameState cgs)
        {
            CurrentGameState = cgs;

            MyPlayerId ??= cgs.PlayerState.Id;

            //ANIMATION
            var blockEvents = cgs.GameEvents.OfType<MoveToBlockZoneEvent>().ToList();
            await PlayBlockAnimations(blockEvents);

            var blockSwapEvents = cgs.GameEvents.OfType<BlockSwapEvent>().ToList();
            await PlayBlockSwapAnimations(blockSwapEvents);

            var attackEvents = cgs.GameEvents.OfType<AttackEvent>().ToList();
            await PlayAttackAnimations(attackEvents);

            var creatureDeathEvents = cgs.GameEvents.OfType<CreatureDeathEvent>().ToList();
            await PlayDeathAnimations(creatureDeathEvents);

            UpdateUIFromGameState();
        }

        private async Task PlayBlockSwapAnimations(List<BlockSwapEvent> blockSwapEvents)
        {
            List<Task> tasks = new();
            var playerBlockSwap = blockSwapEvents.Where(b => b.PlayerId == MyPlayerId).ToList();
            foreach (var swap in playerBlockSwap)
            {
                var creature = CombatZone.GetBoardCard(swap.CreatureId);
                var oldCreature = PlayerBlockZone.GetCardFromBlockZone();

                if (creature != null && oldCreature != null)
                {
                    tasks.Add(creature.AnimateMoveToBlockZone(BattleField, OpponentBlockZone.BlockArea, PlayerBlockZone.BlockArea));
                    tasks.Add(oldCreature.AnimateReturnFromBlockZone(BattleField, OpponentBlockZone.BlockArea, PlayerBlockZone.BlockArea, creature));
                }
            }
            await Task.WhenAll(tasks);

            tasks.Clear();
            var opponentBlockSwap = blockSwapEvents.Where(b => b.PlayerId != MyPlayerId).ToList();
            foreach (var swap in opponentBlockSwap)
            {
                var creature = CombatZone.GetBoardCard(swap.CreatureId);
                var oldCreature = OpponentBlockZone.GetCardFromBlockZone();

                if (creature != null && oldCreature != null)
                {
                    tasks.Add(creature.AnimateMoveToBlockZone(BattleField, OpponentBlockZone.BlockArea, PlayerBlockZone.BlockArea));
                    tasks.Add(oldCreature.AnimateReturnFromBlockZone(BattleField, OpponentBlockZone.BlockArea, PlayerBlockZone.BlockArea, creature));
                }
            }
            await Task.WhenAll(tasks);
        }

        private async Task PlayBlockAnimations(List<MoveToBlockZoneEvent> blockEvents)
        {
            List<List<BoardCard>> blockingCards = [new List<BoardCard>(), new List<BoardCard>()];

            foreach (var eve in blockEvents)
            {
                foreach (BoardCard bc in CombatZone.PlayerCombatZone.Children)
                    if (bc.CardId == eve.CreatureId)
                        blockingCards[0].Add(bc);

                foreach (BoardCard bc in CombatZone.OpponentCombatZone.Children)
                    if (bc.CardId == eve.CreatureId)
                        blockingCards[1].Add(bc);
            }

            for (int i = 0; i < blockingCards.Count; i++)
            {
                foreach (var block in blockingCards[i])
                {
                    await block.AnimateMoveToBlockZone(BattleField, OpponentBlockZone.BlockArea, PlayerBlockZone.BlockArea);
                }
            }
        }

        private async Task PlayAttackAnimations(List<AttackEvent> attackEvents)
        {
            //ATTACK ANIMATION
            List<List<BoardCard>> attackingCards = [new List<BoardCard>(), new List<BoardCard>()];

            foreach (var eve in attackEvents)
            {
                foreach (BoardCard bc in CombatZone.PlayerCombatZone.Children)
                    if (bc.CardId == eve.CreatureId)
                        attackingCards[0].Add(bc);

                foreach (BoardCard bc in CombatZone.OpponentCombatZone.Children)
                    if (bc.CardId == eve.CreatureId)
                        attackingCards[1].Add(bc);
            }

            for (int i = 0; i < attackingCards.Count; i++)
            {
                foreach (var attack in attackingCards[i])
                {
                    await attack.AnimateAttack(BattleField, OpponentBlockZone.BlockArea, PlayerBlockZone.BlockArea);
                }
            }
        }
        private async Task PlayDeathAnimations(List<CreatureDeathEvent> deathEvents)
        {
            List<Task> tasks = new();
            foreach (var eve in deathEvents)
            {
                var card = GetBoardCard(eve.CreatureId);

                if (card != null)
                    tasks.Add(card.AnimateDeath());
            }

            await Task.WhenAll(tasks);
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

            if (gs.OpponentBlockCard == null)
            {
                OpponentBlockZone.RemoveCardFromBlockZone();
            }
            else if (gs.OpponentBlockCard is CreatureCard opponentBlocker)
            {
                OpponentBlockZone.AddCardToBlockZone(Mapper.MapToBoardCard(opponentBlocker, OpponentBlockZone.Side));
            }

            if (gs.PlayerState.BlockingCreature == null)
            {
                PlayerBlockZone.RemoveCardFromBlockZone();
            }
            else if (gs.PlayerState.BlockingCreature is CreatureCard playerBlocker)
            {
                PlayerBlockZone.AddCardToBlockZone(Mapper.MapToBoardCard(playerBlocker, PlayerBlockZone.Side));
            }

            UpdateHand(gs.PlayerState.Hand);

            string gameEvents = "";
            foreach (var gameEvent in gs.GameEvents)
                gameEvents += gameEvent.GetString() + "\n";

            GameEventLog.Text = gameEvents;

            CastSpellButton.Content = "";

            Counter.HiglightRound(gs.RoundCount);
            Counter.TurnLabel.Content = gs.TurnCount.ToString();

            UnselectAll();

            ResetHighlightColour();

            MarkAllHaveActedCards();

            MessageRecieved();

            if (!PlayerCanMove())
            {
                Client.PlayBlank();
                CastSpellButton.Content = "No Actions";
                SelectedCard = null;
            }
        }

        private void MarkAllHaveActedCards()
        {
            //check block zones

            foreach (BoardCard bc in CombatZone.PlayerCombatZone.Children)
                if (bc.HasActed)
                    bc.MarkAsActed();

            foreach (BoardCard bc in CombatZone.OpponentCombatZone.Children)
                if (bc.HasActed)
                    bc.MarkAsActed();
        }

        private bool SummonedCardsCantMove()
        {
            foreach (var creature in CurrentGameState.BoardState.SummonedCreatures[CurrentGameState.PlayerState.Id])
                if (!creature.HasActed)
                    return true;

            return false;
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
            if (SummonedCardsCantMove())
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

        public BoardCard? GetBoardCard(int cardId)
        {
            UIElementCollection[] cardsGroup =
            [
                PlayerBlockZone.BlockArea.Children,
                OpponentBlockZone.BlockArea.Children,
                CombatZone.PlayerCombatZone.Children,
                CombatZone.OpponentCombatZone.Children,
            ];

            foreach (var cards in cardsGroup)
                foreach (var card in cards)
                    if (card is BoardCard boardCard)
                        if (boardCard != null && boardCard.CardId == cardId)
                            return boardCard;

            return null;
        }
    }
}
