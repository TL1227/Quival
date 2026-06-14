using QuivalCombatTestWPF.Colours;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using QuivalLogicEngine.Turns;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuivalCombatTestWPF
{
    //TOODO: put this somewhere else later
    public enum Side
    {
        Player,
        Opponent,
    }

    public class Position
    {
        public double Top;
        public double Left;
    }

    public partial class MainWindow : Window
    {
        QuivalClient Client { get; set; }
        Control? SelectedCard { get; set; }
        private ClientGameState CurrentGameState { get; set; }

        int MaxSummonedCards = 5;

        private CombatZone[] CombatZones { get; set; }
        private BlockZone[] BlockZones { get; set; }
        private Grid[] SummonZones { get; set; }

        public int? MyPlayerId { get; set; } = null;

        public int PlayerSide = (int)Side.Player;
        public int OpponentSide = (int)Side.Opponent;

        public CardSelector? CardSelector { get; set; }

        public QuivalTurn? CurrentTurn { get; set; }

        public bool CanClickCards { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();

            PlayerBlockZone.Side = Side.Player;
            OpponentBlockZone.Side = Side.Opponent;

            HandZone.CardClicked += HandZone_CardClicked;
            PlayerCombatZone.CardClicked += CombatZone_CardClicked;
            PlayerBlockZone.ZoneClicked += PlayerBlockZone_ZoneClicked;
            OpponentBlockZone.ZoneClicked += OpponentBlockZone_ZoneClicked;
            ClickBlocker.MouseLeftButtonDown += ClickBlocker_MouseLeftButtonDown;
            EndTurnButton.Click += EndTurnButton_Click;

            CombatZones = [PlayerCombatZone, OpponentCombatZone];
            BlockZones = [PlayerBlockZone, OpponentBlockZone];
            SummonZones = [PlayerSummonZone, OpponentSummonZone];

            KeyDown += MainWindow_KeyDown;

            Layout.Loaded += (_, _) =>
            {
                Client = new QuivalClient(this);
                Client.ConnectToServer();
            };
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (CardSelector != null)
                {
                    Layout.Canvas.Children.Remove(CardSelector);
                    CardSelector = null;
                    UnselectAll();
                }
            }
        }

        #region Animation
        public async void UpdateGameState(ClientGameState cgs)
        {
            CurrentGameState = cgs;

            MyPlayerId ??= cgs.PlayerState.Id;

            UnselectAll();

            var castEvents = cgs.GameEvents.OfType<CastEvent>().ToList<EventMessage>();
            await AnimateEvents(castEvents);

            var summonEvents = cgs.GameEvents.OfType<SummonEvent>().ToList<EventMessage>();
            await AnimateEvents(summonEvents);

            var blockEvents = cgs.GameEvents.OfType<MoveToBlockZoneEvent>().ToList<EventMessage>();
            await AnimateEvents(blockEvents);

            var attackEvents = cgs.GameEvents.OfType<AttackEvent>().ToList<EventMessage>();
            await AnimateEvents(attackEvents);

            var deathEvents = cgs.GameEvents.OfType<CreatureDeathEvent>().ToList();
            await PlayDeathAnimations(deathEvents);

            UpdateUIFromGameState();
        }

        private async Task AnimateEvents(List<EventMessage> events)
        {
            var myEvents = events.Where(e => e.PlayerId == MyPlayerId).ToList();
            foreach (var myEvent in myEvents)
            {
                Debug.WriteLine($"PLayer {myEvent.PlayerId} [EVENT] {myEvent.GetString()}");
                await PlayEventAnimation(myEvent, Side.Player);
            }

            var opponentEvents = events.Where(e => e.PlayerId != MyPlayerId && e.PlayerId != -1).ToList();
            foreach (var opponentEvent in opponentEvents)
            {
                Debug.WriteLine($"PLayer {opponentEvent.PlayerId} [EVENT] {opponentEvent.GetString()}");
                await PlayEventAnimation(opponentEvent, Side.Opponent);
            }
        }

        private async Task PlayEventAnimation(EventMessage eventMessage, Side side)
        {
            switch (eventMessage)
            {
                case CastEvent castEvent:
                    await PlayCastAnimation(castEvent, side);
                    break;
                case SummonEvent summonEvent:
                    await PlaySummonAnimation(summonEvent, side);
                    break;
                case MoveToBlockZoneEvent moveToBlockZoneEvent:
                    await PlayBlockAnimation(moveToBlockZoneEvent, side);
                    break;
                case BlockSwapEvent blockSwapEvent:
                    await PlayBlockSwapAnimation(blockSwapEvent, side);
                    break;
                case AttackEvent attackEvent:
                    await PlayAttackAnimation(attackEvent, side);
                    break;
                default:
                    break;
            }
        }

        private async Task PlayBlockSwapAnimation(BlockSwapEvent blockSwapEvent, Side side)
        {
            var newCreature = CombatZones[(int)side].GetBoardCard(blockSwapEvent.CreatureId);
            var oldCreature = BlockZones[(int)side].GetCardFromBlockZone();

            if (newCreature != null && oldCreature != null)
            {
                List<Task> tasks = new();

                tasks.Add(Animation.MoveToPoint(newCreature, newCreature.GetPos(), Layout.BlockAreas[(int)side]));
                int removedCardIndex = CombatZones[(int)side].RemoveCardFromZone(newCreature.Id, Layout);
                BlockZones[(int)side].AddCardToBlockZone(newCreature, Layout, Layout.BlockAreas[(int)side]);

                tasks.Add(Animation.MoveToPoint(oldCreature, oldCreature.GetPos(), newCreature.GetPos()));
                CombatZones[(int)side].SummonedCards[removedCardIndex] = oldCreature;

                await Task.WhenAll(tasks);
            }
        }

        private async Task PlayBlockAnimation(MoveToBlockZoneEvent blockEvent, Side side)
        {
            int theSide = (int)side;
            foreach (var summonedCard in CombatZones[theSide].SummonedCards)
            {
                if (summonedCard != null && summonedCard.Id == blockEvent.CreatureId)
                {
                    await Animation.MoveToPoint(summonedCard, summonedCard.GetPos(), Layout.BlockAreas[theSide]);
                    CombatZones[theSide].RemoveCardFromZone(summonedCard.Id, Layout);
                    BlockZones[theSide].AddCardToBlockZone(summonedCard, Layout, Layout.BlockAreas[theSide]);
                }
            }
        }

        private async Task PlayAttackAnimation(AttackEvent attackEvent, Side side)
        {
            var attackingBoardCard = GetBoardCard(attackEvent.CreatureId);

            if (attackingBoardCard != null)
            {
                foreach (var action in attackEvent.CardActionEvents)
                {
                    await PlayCardActionAnimation(action, side);
                }

                Position originalPos = attackingBoardCard.GetPos();
                await Animation.MoveToPoint(attackingBoardCard, attackingBoardCard.GetPos(), Layout.BlockAreas[OppositeSide((int)side)]);

                var attackingCreature = (CreatureCard)attackingBoardCard.Tag;
                if (attackingCreature != null)
                {
                    var blockingBoardCard = GetBoardCard(attackEvent.BlockingCreatureId);
                    if (blockingBoardCard != null)
                    {
                        blockingBoardCard.TakeDamage(attackingCreature.GetAttackDamage());

                        var blockingCreature = (CreatureCard)blockingBoardCard.Tag;
                        if (blockingCreature != null)
                        {
                            attackingBoardCard.TakeDamage(blockingCreature.GetAttackDamage());
                        }
                    }
                }

                await Animation.MoveToPoint(attackingBoardCard, attackingBoardCard.GetPos(), originalPos, System.Windows.Media.Animation.EasingMode.EaseOut);
            }
        }

        private async Task PlayDeathAnimations(List<CreatureDeathEvent> deathEvents)
        {
            List<Task> tasks = new();

            foreach (var eve in deathEvents)
            {
                var card = GetBoardCard(eve.CreatureId);

                if (card != null)
                {
                    Debug.WriteLine("Animating Death!");
                    tasks.Add(Animation.AnimateDeath(card));
                }
                else
                {
                    Debug.WriteLine("Card not found. Can't animate!");
                }
            }

            await Task.WhenAll(tasks);
        }

        private async Task PlaySummonAnimation(SummonEvent summonEvent, Side side)
        {
            CreatureCard? cardToSummon = CurrentGameState.BoardState.SummonedCreatures[summonEvent.PlayerId].SingleOrDefault(c => c.Id == summonEvent.CreatureId);
            if (cardToSummon != null)
            {
                var boardCard = Mapper.MapToBoardCard(cardToSummon, BoardCard_Clicked);
                if (boardCard != null)
                {
                    Position handcardPos = new();
                    foreach (var child in Layout.Canvas.Children)
                    {
                        if (child is HandCard handCard)
                        {
                            if (handCard.Id == boardCard.Id)
                            {
                                handcardPos.Left = Canvas.GetLeft(handCard);
                                handcardPos.Top = Canvas.GetTop(handCard);
                                handCard.Visibility = Visibility.Hidden;
                            }
                        }
                    }

                    if (side == Side.Player)
                    {
                        boardCard.SetPos(handcardPos);
                    }
                    else
                    {
                        Position position = new() { Left = Layout.ActualWidth / 2, Top = 100 };
                        boardCard.SetPos(position);
                    }

                    int summonIndex = CombatZones[(int)side].AddCardToNextFreeSlot(boardCard, Layout);
                    await Animation.MoveToPoint(boardCard, handcardPos, Layout.SummonSlots[(int)side][summonIndex]);
                }
            }
        }
        private async Task PlayCardActionAnimation(CardActionEvent actionEvent, Side side)
        {
            foreach (var target in actionEvent.TargetsCardIds)
            {
                var targetCard = GetBoardCard(target);
                if (targetCard == null)
                {
                    Debug.WriteLine($"[Action] {targetCard.Id} is null");
                }

                switch (actionEvent.Intent)
                {
                    case Intent.AttackBuff:
                        {
                            await targetCard.FlashUp(Brushes.Aquamarine);

                            targetCard.AttackLabel.Content = targetCard.GetAttackFromLabel() + actionEvent.Value;
                            targetCard.AttackLabel.Foreground = Brushes.Aquamarine;

                            await targetCard.FlashDown(Brushes.Aquamarine);
                        }
                        break;
                    case Intent.DamageAbsorbToken:
                        break;
                    case Intent.DirectDamage:
                        {
                            targetCard.TakeDamage(actionEvent.Value);
                        }
                        break;
                    case Intent.DrawCard:
                        break;
                    case Intent.RushDown:
                        break;
                    case Intent.RestoreAction:
                        break;
                    case Intent.None:
                    default:
                        break;
                }
            }
        }

        private async Task PlayCastAnimation(CastEvent castEvent, Side side)
        {
            //TODO: don't hard code all this!
            var fullCard = Mapper.MapToHandCard(castEvent.CastCard);
            var centerY = Layout.ActualHeight / 2;
            var centerX = Layout.ActualWidth / 2;
            fullCard.SetPos(new Position() { Left = centerX, Top = centerY });
            Layout.Canvas.Children.Add(fullCard);

            await fullCard.SummonIn(Brushes.Aquamarine);

            await Task.Delay(500);

            foreach (var action in castEvent.CardActionEvents)
            {
                await PlayCardActionAnimation(action, side);
            }

            await Task.Delay(500);

            Layout.Canvas.Children.Remove(fullCard);

        }
        #endregion

        public int OppositeSide(int i) => i == PlayerSide ? OpponentSide : PlayerSide;

        #region StateUpdates
        public void UpdateUIFromGameState()
        {
            Debug.WriteLine($"Gamestate update: Turn {CurrentGameState.TurnCount} round {CurrentGameState.RoundCount}");
            var gs = CurrentGameState;

            //TODO: rework this with the new layout canvas
            CombatZones[PlayerSide].UpdateCombatZone(gs.BoardState.SummonedCreatures[gs.PlayerState.Id], Layout, Layout.SummonSlots[PlayerSide], BoardCard_Clicked);
            CombatZones[OpponentSide].UpdateCombatZone(gs.BoardState.SummonedCreatures[gs.OpponentId], Layout, Layout.SummonSlots[OpponentSide], BoardCard_Clicked);

            PlayerResources.HealthPoints.Content = gs.PlayerState.HealthPoints;
            OpponentResources.HealthPoints.Content = gs.OpponentHealthPoints;

            PlayerResources.ManaPoints.Content = gs.PlayerState.ManaPoints;
            OpponentResources.ManaPoints.Content = gs.OpponentManaPoints;

            Card?[] blockingCreatures = [ gs.PlayerState.BlockingCreature, gs.OpponentBlockCard ];
            for (int i = 0; i < 2; i++)
            {
                if (blockingCreatures[i] == null)
                {
                    BlockZones[i].RemoveCardFromBlockZone(Layout);
                }
                else if (blockingCreatures[i] is CreatureCard cc)
                {
                    if (BlockZones[i].CurrentCard != null )
                    {
                        if (BlockZones[i].CurrentCard!.Id == cc.Id)
                        {
                            //update
                            BlockZones[i].CurrentCard!.HealthLabel.Content = cc.CurrentHealth;
                            BlockZones[i].CurrentCard!.HasActed = cc.HasActed;

                            if (cc.CurrentHealth < cc.Health)
                                BlockZones[i].CurrentCard!.HealthLabel!.Foreground = Brushes.Red;
                        }
                        else
                        {
                            var blockingCreature = Mapper.MapToBoardCard(cc, BoardCard_Clicked);
                            BlockZones[i].AddCardToBlockZone(blockingCreature, Layout, Layout.BlockAreas[i]);
                        }
                    }
                }
            }

            UpdateHand(gs.PlayerState.Hand);

            string gameEvents = "";
            foreach (var gameEvent in gs.GameEvents)
                gameEvents += gameEvent.GetString() + "\n";

            // GameEventLog.Text = gameEvents;
            Debug.WriteLine(gameEvents);

            CastSpellButton.Content = "";

            Counter.HiglightRound(gs.RoundCount);
            Counter.TurnLabel.Content = gs.TurnCount.ToString();

            UnselectAll();

            ResetHighlightColour();

            //Mark all cards that have acted
            for (int i = 0; i < CombatZones.Length; i++)
                CombatZones[i].MarkActedCards();

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

            //TODO: both sides
            PlayerSummonZone.Children.Clear();
            OpponentSummonZone.Children.Clear();
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
            Layout.ClearHand();

            for (int i = 0; i < cards.Count; i++)
            {
                if (i >= Layout.HandSlots.Length)
                    break;

                HandCard hand = Mapper.MapToHandCard(cards[i]);
                hand.MouseLeftButtonDown += HandZone_CardClicked;
                hand.SetPos(Layout.HandSlots[i]);
                Layout.Canvas.Children.Add(hand);
            }
        }

        public void UnselectAll()
        {
            for (int i = 0; i < 2; i++)
            {
                BlockZones[i].SetHighlighted(false);
                CombatZones[i].Highlight(false);
                CombatZones[i].ClearHighlightedCards();
            }

            foreach (var child in Layout.Canvas.Children)
            {
                if (child is HandCard hc)
                    hc.RemoveHighlight();
            }

            ResetHighlightColour();

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

        private void EndTurnButton_Click(object sender, RoutedEventArgs e)
        {
            QuivalTurn turn = new()
            {
                TurnType = TurnType.EndTurn
            };

            Client.SubmitTurn(turn);

            CastSpellButton.Content = "Cast";
            SelectedCard = null;
        }

        private void CastCard(HandCard hc)
        {
            CurrentTurn = new()
            {
                TurnType = TurnType.Cast,
                CardToPlayId = hc.Id
            };

            Card cardToCast = (Card)hc.Tag;
            var castAbility = cardToCast.Abilities.SingleOrDefault(a => a.Trigger == QuivalLogicEngine.Cards.Trigger.Cast);
            if (castAbility != null)
            {
                var cardActions = castAbility.Actions.Where(a => a.TargetType != TargetType.None).ToList();

                List<NumberOfIntents> actionIntents = new();
                foreach (var action in cardActions)
                {
                    actionIntents.Add(new NumberOfIntents
                    {
                        Intent = action.Intent,
                        Number = action.NumberOfTargets
                    });
                }

                CardSelector = new(actionIntents);
                Layout.Canvas.Children.Add(CardSelector);
                Canvas.SetLeft(CardSelector, 100);
                Canvas.SetTop(CardSelector, 100);
            }
            else
            {

                Client.SubmitTurn(CurrentTurn);

                QuivalColour.ChangetoPurpleHighlights();
                CastSpellButton.Content = "Cast";
                SelectedCard = null;
            }
        }

        private void BoardCard_Clicked(object? sender, MouseButtonEventArgs e)
        {
            if (sender is BoardCard bc)
            {
                if (!CanClickCards) return;

                if (CardSelector != null)
                {
                    var selectionFinished = CardSelector.SelectCard(bc.Id);
                    bc.MarkSelected(true);

                    if (selectionFinished != null)
                    {
                        Layout.Canvas.Children.Remove(CardSelector);
                        CardSelector = null;

                        if (CurrentTurn != null)
                        {
                            CurrentTurn.SelectedCardIds = selectionFinished;
                            Client.SubmitTurn(CurrentTurn);
                        }
                    }

                    e.Handled = true; //So we don't trigger other click events
                }
                else if (IsInBlockZone(bc))
                {

                }

            }
        }

        private bool IsInBlockZone(BoardCard card)
        {
            return BlockZones[0].CurrentCard == card || BlockZones[1].CurrentCard == card;
        }

        private void CombatZone_CardClicked(object? sender, EventArgs e)
        {
            if (sender is BoardCard card)
            {
                if (CardSelector != null)
                {
                    var result = CardSelector.SelectCard(card.Id);
                    if (result != null)
                    {
                        Layout.Canvas.Children.Remove(CardSelector);
                        CardSelector = null;
                        CastSpellButton.Content = "Cast";
                        //TODO: play the turn
                    }
                }
                else if (CombatZones[PlayerSide].CardIsSummonedByPlayer(card))
                {
                    if (!card.HasActed)
                    {
                        UnselectAll();

                        card.Highlight();
                        //card.Overlay.Opacity = 0.4;
                        SelectedCard = card;

                        PlayerBlockZone.SetHighlighted(true);
                        OpponentBlockZone.SetHighlighted(true);
                    }
                }
            }
        }

        //BlockZones
        private void PlayerBlockZone_ZoneClicked(object? sender, EventArgs e)
        {
            if (sender is BoardCard cc)
            {
                var result = CardSelector?.SelectCard(cc.Id);
                if (result != null)
                {
                    Layout.Canvas.Children.Remove(CardSelector);
                    CardSelector = null;
                    //TODO: play the turn
                }
            }
            if (SelectedCard != null && SelectedCard is BoardCard bc)
            {
                QuivalTurn turn = new()
                {
                    TurnType = TurnType.MoveToBlock,
                    CardToPlayId = bc.Id
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
            if (sender is BoardCard cc)
            {
                var result = CardSelector?.SelectCard(cc.Id);
                if (result != null)
                {
                    Layout.Canvas.Children.Remove(CardSelector);
                    CardSelector = null;
                    //TODO: play the turn
                }
            }
            else if (SelectedCard != null && SelectedCard is BoardCard bc)
            {
                if (CombatZones[PlayerSide].CardIsSummonedByPlayer(bc))
                {
                    QuivalTurn turn = new()
                    {
                        TurnType = TurnType.Attack,
                        CardToPlayId = bc.Id
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
                if (CardSelector != null || !CanClickCards)
                    return;

                if (card == SelectedCard)
                {
                    CastCard(card);
                    QuivalColour.ChangetoPurpleHighlights();
                    return;
                }
                else
                {
                    int cost = ((Card)card.Tag).Cost;
                    int mana = CurrentGameState.PlayerState.ManaPoints;

                    if (cost > mana)
                        return;

                    UnselectAll();

                    if (CombatZones[PlayerSide].GetNumberOfSummonedCards() < MaxSummonedCards)
                    {
                        card.Highlight();
                        SelectedCard = card;
                    }
                }
            }
        }
        #endregion

        public void MessageSent()
        {
            ClickBlocker.IsHitTestVisible = true;
            CanClickCards = false;
        }

        public void MessageRecieved()
        {
            ClickBlocker.IsHitTestVisible = false;
            CurrentTurn = null;
            CanClickCards = true;
        }

        public BoardCard? GetBoardCard(int cardId)
        {
            foreach (var bc in Layout.Canvas.Children.OfType<BoardCard>())
            {
                if (bc.Id == cardId)
                    return bc;
            }

            return null;
            
            /*
            UIElementCollection[] cardsGroup =
            [
                PlayerSummonZone.Children
            ];

            foreach (var cards in cardsGroup)
                foreach (var card in cards)
                    if (card is BoardCard boardCard)
                        if (boardCard != null && boardCard.Id == cardId)
                            return boardCard;

            List<BoardCard> otherLocations = new();
            otherLocations.AddRange(PlayerCombatZone.GetBoardCards());
            otherLocations.AddRange(OpponentCombatZone.GetBoardCards());

            if (PlayerBlockZone.CurrentCard != null)
                otherLocations.Add(PlayerBlockZone.CurrentCard!);

            if (OpponentBlockZone.CurrentCard != null)
                otherLocations.Add(OpponentBlockZone.CurrentCard!);

            foreach (var card in otherLocations)
                        if (card != null && card.Id == cardId)
                            return card;

            return null;
            */
        }
    }
}
