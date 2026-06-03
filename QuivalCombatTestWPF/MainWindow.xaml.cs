using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuivalCombatTestWPF.Colours;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using QuivalLogicEngine.Turns;

namespace QuivalCombatTestWPF
{
    //TOODO: put this somewhere else later
    public enum Side
    {
        Player,
        Opponent,
    }

    public partial class MainWindow : Window
    {
        QuivalClient Client { get; set; }
        Control? SelectedCard { get; set; }
        private ClientGameState CurrentGameState { get; set; }

        private CardLayoutManager CardLayoutManager { get; set; }

        private Animator Animator { get; set; } 

        int MaxSummonedCards = 5;

        private CombatZone[] CombatZones { get; set; }

        public int? MyPlayerId { get; set; } = null;

        public int PlayerSide = (int)Side.Player;
        public int OpponentSide = (int)Side.Opponent;

        public MainWindow()
        {
            InitializeComponent();

            Client = new QuivalClient(this);
            Client.ConnectToServer();

            PlayerBlockZone.Side = Side.Player;
            OpponentBlockZone.Side = Side.Opponent;

            HandZone.CardClicked += HandZone_CardClicked;
            PlayerCombatZone.CardClicked += CombatZone_CardClicked;
            PlayerCombatZone.PlayerZoneClicked += CombatZone_PlayerZoneClicked;
            PlayerBlockZone.ZoneClicked += PlayerBlockZone_ZoneClicked;
            OpponentBlockZone.ZoneClicked += OpponentBlockZone_ZoneClicked;
            ClickBlocker.MouseLeftButtonDown += ClickBlocker_MouseLeftButtonDown;

            Animator = new(AnimationCanvas);

            BattleField.Loaded += (s, e) =>
            {
                CardLayoutManager = new();
                CardLayoutManager.FillCombatZonePoints(PlayerCombatZone, BattleField);
                CardLayoutManager.FillBlockZonePoints(PlayerBlockZone, OpponentBlockZone);
            };

            CombatZones = [PlayerCombatZone, OpponentCombatZone];
        }

        #region StateUpdates

        public async void UpdateGameState(ClientGameState cgs)
        {
            CurrentGameState = cgs;

            MyPlayerId ??= cgs.PlayerState.Id;

            UnselectAll();

            //ANIMATION
            var summonEvents = cgs.GameEvents.OfType<SummonEvent>().ToList();
            await PlaySummonAnimations(summonEvents);

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
                var creature = CombatZones[(int)Side.Opponent].GetBoardCard(swap.CreatureId);
                var oldCreature = PlayerBlockZone.GetCardFromBlockZone();

                if (creature != null && oldCreature != null)
                {
                    tasks.Add(creature.AnimateMoveToBlockZone(BattleField, OpponentBlockZone.BlockArea, PlayerBlockZone.BlockArea));
                    Point end = creature.TransformToVisual(BattleField).Transform(new Point(0, 0));
                    tasks.Add(oldCreature.AnimateReturnFromBlockZone(BattleField, OpponentBlockZone.BlockArea, PlayerBlockZone.BlockArea, end));
                }
            }
            await Task.WhenAll(tasks);

            tasks.Clear();
            var opponentBlockSwap = blockSwapEvents.Where(b => b.PlayerId != MyPlayerId).ToList();
            foreach (var swap in opponentBlockSwap)
            {
                var creature = CombatZones[(int)Side.Opponent].GetBoardCard(swap.CreatureId);
                var oldCreature = OpponentBlockZone.GetCardFromBlockZone();

                if (creature != null && oldCreature != null)
                {
                    tasks.Add(creature.AnimateMoveToBlockZone(BattleField, OpponentBlockZone.BlockArea, PlayerBlockZone.BlockArea));
                    Point end = creature.TransformToVisual(BattleField).Transform(new Point(0, 0));
                    tasks.Add(oldCreature.AnimateReturnFromBlockZone(BattleField, OpponentBlockZone.BlockArea, PlayerBlockZone.BlockArea, end));
                }
            }
            await Task.WhenAll(tasks);
        }

        private async Task PlayBlockAnimations(List<MoveToBlockZoneEvent> blockEvents)
        {
            List<List<BoardCard>> blockingCards = [new List<BoardCard>(), new List<BoardCard>()];

            foreach (var eve in blockEvents)
            {
                for (int i = 0; i < CombatZones.Length; i++)
                {
                    foreach (var slot in CombatZones[i].SummonSlots)
                        if (slot.Children[0] is BoardCard bc && bc.CardId  == eve.CreatureId)
                            blockingCards[i].Add(bc);
                }
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
                for (int i = 0; i < CombatZones.Length; i++)
                {
                    foreach (var slot in CombatZones[i].SummonSlots)
                        if (slot.Children[0] is BoardCard bc && bc.CardId  == eve.CreatureId)
                            attackingCards[i].Add(bc);
                }
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

        private async Task PlaySummonAnimations(List<SummonEvent> summonEvents)
        {
            List<Task> tasks = new();

            List<BoardCard> bcs = new();
            foreach (var eve in summonEvents)
            {
                var bc = GetBoardCard(eve.CreatureId);

                if (bc != null)
                {
                    //Summon test
                    bc.Visibility = Visibility.Visible;
                    Side side = eve.PlayerId == MyPlayerId ? Side.Player : Side.Opponent;
                    var end = CardLayoutManager.CombatZones[(int)side][0];
                    tasks.Add(bc.AnimateSummon(end, BattleField));
                    bcs.Add(bc);
                }
            }

            await Task.WhenAll(tasks);

            foreach (var b in bcs)
            {
                PlayerSummonZone.Children.Remove(b);
            }
        }

        public void UpdateUIFromGameState()
        {
            var gs = CurrentGameState;

            CombatZones[PlayerSide].UpdateCombatZone(gs.BoardState.SummonedCreatures[gs.PlayerState.Id]);
            CombatZones[OpponentSide].UpdateCombatZone(gs.BoardState.SummonedCreatures[gs.OpponentId]);

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
                QuivalTurn turn = new()
                {
                    TurnType = TurnType.EndTurn
                };
                Client.SubmitTurn(turn);

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
            List<HandCard> hand = Mapper.MapToHandCards(cards, PlayerSummonZone);
            
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
                QuivalTurn turn = new()
                {
                    TurnType = TurnType.Cast,
                    CardToPlayId = hc.CardId
                };

                Client.SubmitTurn(turn);

                QuivalColour.ChangetoPurpleHighlights();
                CastSpellButton.Content = "Cast";
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
                QuivalTurn turn = new()
                {
                    TurnType = TurnType.MoveToBlock,
                    CardToPlayId = bc.CardId
                };

                Client.SubmitTurn(turn);

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
                    QuivalTurn turn = new()
                    {
                        TurnType = TurnType.Attack,
                        CardToPlayId = bc.CardId
                    };

                    Client.SubmitTurn(turn);

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
                PlayerSummonZone.Children
            ];

            foreach (var cards in cardsGroup)
                foreach (var card in cards)
                    if (card is BoardCard boardCard)
                        if (boardCard != null && boardCard.CardId == cardId)
                            return boardCard;

            List<BoardCard> combatZoneCards = new();
            combatZoneCards.AddRange(PlayerCombatZone.GetBoardCards());
            combatZoneCards.AddRange(OpponentCombatZone.GetBoardCards());


            return null;
        }
        public HandCard? GetHandCard(int cardId)
        {
            foreach (var card in HandZone.HandGrid.Children)
                    if (card is HandCard handCard)
                        if (handCard != null && handCard.CardId == cardId)
                            return handCard;

            return null;
        }
    }
}
