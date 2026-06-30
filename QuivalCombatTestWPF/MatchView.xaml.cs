using QuivalCombatTestWPF.Colours;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using QuivalLogicEngine.Messages;
using QuivalLogicEngine.Turns;
using System.Data;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

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

    public partial class MatchView : UserControl
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
        public bool ViewingCard { get; set; } = false;

        public MatchView(QuivalClient client)
        {
            InitializeComponent();

            Client = client;

            PlayerBlockZone.Side = Side.Player;
            OpponentBlockZone.Side = Side.Opponent;

            HandZone.CardClicked += HandZone_CardClicked;
            PlayerCombatZone.CardClicked += CombatZone_CardClicked;
            PlayerBlockZone.ZoneClicked += PlayerBlockZone_ZoneClicked;
            OpponentBlockZone.ZoneClicked += OpponentBlockZone_ZoneClicked;
            ClickBlocker.MouseDown += ClickBlocker_Click;
            EndTurnButton.Click += EndTurnButton_Click;

            CombatZones = [PlayerCombatZone, OpponentCombatZone];
            BlockZones = [PlayerBlockZone, OpponentBlockZone];
            SummonZones = [PlayerSummonZone, OpponentSummonZone];

            KeyDown += MainWindow_KeyDown;

            PlayerResources.MouseLeftButtonDown += PlayerResources_MouseLeftButtonDown;
            OpponentResources.MouseLeftButtonDown += OpponentResources_MouseLeftButtonDown;

            Layout.Loaded += async (_, _) =>
            {
                await Client.SendMessageAsync(new StartMatchRequest());
            };
        }

        private void HandleCardSelectionClick(int cardId)
        {
            if (CardSelector.CardIsValidTarget(cardId))
            {
                var selectedCards = CardSelector.SelectCard(cardId);
                if (selectedCards != null)
                {
                    Layout.Canvas.Children.Remove(CardSelector);
                    CardSelector = null;
                    Client.SubmitSelection(selectedCards);
                }
            }
            else
            {
                Animation.DisplayMessage("Card is not a valid target", Layout.Canvas);
            }
        }

        private void PlayerResources_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CardSelector != null)
            {
                var id = (int)MyPlayerId;
                HandleCardSelectionClick(id);
            }
        }

        private void OpponentResources_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CardSelector != null)
            {
                var id = OppositeSide((int)MyPlayerId);
                HandleCardSelectionClick(id);
            }
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

            var blockSwapEvents = cgs.GameEvents.OfType<BlockSwapEvent>().ToList<EventMessage>();
            await AnimateEvents(blockSwapEvents);

            var attackEvents = cgs.GameEvents.OfType<AttackEvent>().ToList<EventMessage>();
            await AnimateEvents(attackEvents);

            var deathEvents = cgs.GameEvents.OfType<CreatureDeathEvent>().ToList();
            await PlayDeathAnimations(deathEvents);

            await UpdateUIFromGameState();
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

                //TODO: This is a silly hack for now. 
                //We're going to need a way to figure out which effects should animate before the attack and which should animate after
                foreach (var action in attackEvent.CardActionEvents)
                    if (action.Effect == QuivalLogicEngine.Cards.Effect.AttackBuffRound)
                        await PlayCardActionAnimation(action, side);

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

                //More of the above silly hack
                foreach (var action in attackEvent.CardActionEvents)
                    if (action.Effect != QuivalLogicEngine.Cards.Effect.AttackBuffRound)
                        await PlayCardActionAnimation(action, side);
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

                    foreach (var action in summonEvent.CardActionEvents)
                    {
                        await PlayCardActionAnimation(action, side);
                        //await Task.Delay(500);
                    }

                    await Task.Delay(500);
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
                    if (target == 0 || target == 1)
                    {
                        //handle the player thing
                        switch (actionEvent.Effect)
                        {
                            case QuivalLogicEngine.Cards.Effect.DirectDamage:
                                {
                                    if (target == MyPlayerId)
                                    {
                                        Point point = PlayerResources.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0)); 
                                        await Animation.DirectEffect(point.X, point.Y,Layout.Canvas, Brushes.Red);
                                        PlayerResources.TakeDamage(actionEvent.Value);
                                    }
                                    else
                                    {
                                        Point point = OpponentResources.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0)); 
                                        await Animation.DirectEffect(point.X, point.Y,Layout.Canvas, Brushes.Red);
                                        OpponentResources.TakeDamage(actionEvent.Value);
                                    }
                                }
                                break;
                            case QuivalLogicEngine.Cards.Effect.Heal:
                                {
                                    if (target == MyPlayerId)
                                    {
                                        Point point = PlayerResources.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
                                        await Animation.DirectEffect(point.X, point.Y,Layout.Canvas, Brushes.Green);
                                        PlayerResources.HealDamage(actionEvent.Value);
                                    }
                                    else
                                    {
                                        Point point = PlayerResources.TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));
                                        await Animation.DirectEffect(point.X, point.Y,Layout.Canvas, Brushes.Green);
                                        OpponentResources.HealDamage(actionEvent.Value);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"Can't find target with id of {target}");
                    }
                }
                else
                {
                    switch (actionEvent.Effect)
                    {
                        case QuivalLogicEngine.Cards.Effect.AttackBuffRound:
                            {
                                await targetCard.FlashUp(Brushes.Aquamarine);
                                targetCard.AttackLabel.Content = targetCard.GetAttackFromLabel() + actionEvent.Value;
                                targetCard.AttackLabel.Foreground = Brushes.Aquamarine;
                                await targetCard.FlashDown(Brushes.Aquamarine);
                            }
                            break;
                        case QuivalLogicEngine.Cards.Effect.AttackBuff:
                            {
                                //await targetCard.FlashUp(Brushes.Aquamarine);
                                targetCard.AttackLabel.Content = actionEvent.Value;
                                //await targetCard.FlashDown(Brushes.Aquamarine);
                            }
                            break;
                        case QuivalLogicEngine.Cards.Effect.AttackDebuff:
                            {
                                //await targetCard.FlashUp(Brushes.Aquamarine);
                                targetCard.AttackLabel.Content = actionEvent.Value;
                                //await targetCard.FlashDown(Brushes.Aquamarine);
                            }
                            break;
                        case QuivalLogicEngine.Cards.Effect.DamageAbsorbToken:
                            break;
                        case QuivalLogicEngine.Cards.Effect.DirectDamage:
                            {
                                targetCard.TakeDamage(actionEvent.Value);
                            }
                            break;
                        case QuivalLogicEngine.Cards.Effect.Heal:
                            {
                                await targetCard.FlashUp(Brushes.LimeGreen);

                                targetCard.HealthLabel.Content = targetCard.GetCurrentHealthFromLabel() + actionEvent.Value;
                                Card card = (Card)targetCard.Tag;
                                if (card != null && card is CreatureCard cc)
                                {
                                    if ((int)targetCard.HealthLabel.Content < cc.Health)
                                        targetCard.HealthLabel.Foreground = Brushes.Red;
                                }

                                await targetCard.FlashDown(Brushes.LimeGreen);
                            }
                            break;

                        case QuivalLogicEngine.Cards.Effect.DrawCard:
                            break;
                        case QuivalLogicEngine.Cards.Effect.RestoreAction:
                            break;
                        case QuivalLogicEngine.Cards.Effect.None:
                        default:
                            break;
                    }
                }
            }
        }

        private async Task PlayCastAnimation(CastEvent castEvent, Side side)
        {
            //TODO: fix all this hard coding!!!
            var fullCard = Mapper.MapToHandCard(castEvent.CastCard);

            Position blockAreaPos = Layout.BlockAreas[(int)side];

            Position summonPos = new()
            {
                Left = blockAreaPos.Left - 200,
                Top = blockAreaPos.Top - 50
            };

            fullCard.SetPos(summonPos);
            Layout.Canvas.Children.Add(fullCard);

            await fullCard.SummonIn(Brushes.Aquamarine);

            foreach (var action in castEvent.CardActionEvents)
            {
                await Task.Delay(500);
                await PlayCardActionAnimation(action, side);
            }

            await Task.Delay(500);

            Layout.Canvas.Children.Remove(fullCard);

        }
        #endregion

        public int OppositeSide(int i) => i == PlayerSide ? OpponentSide : PlayerSide;

        #region StateUpdates
        public async Task UpdateUIFromGameState()
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
                    Trigger = TriggerType.EndTurn
                };
                Client.SubmitTurn(turn);

                CastSpellButton.Content = "No Actions";
                SelectedCard = null;
            }

            //TODO: both sides
            PlayerSummonZone.Children.Clear();
            OpponentSummonZone.Children.Clear();

            //The new turn thing is a bit buggy and I think it looks better without it
            /*
            var newTurn = CurrentGameState.GameEvents.OfType<NewTurn>().SingleOrDefault();
            if (newTurn != null)
            {
                await Animation.DisplayTurn(Layout.Canvas);
            }
            */

            Animation.DisplayRound(CurrentGameState.RoundCount, Layout.Canvas);
        }

        private bool SummonedCardsCanMove()
        {
            foreach (var creature in CurrentGameState.BoardState.SummonedCreatures[CurrentGameState.PlayerState.Id])
            {
                if (!creature.HasActed)
                {
                    Debug.WriteLine($"[Player {CurrentGameState.PlayerState.Id}] {creature.Id} can still act!");
                    return true;
                }

            }

            Debug.WriteLine($"[Player {CurrentGameState.PlayerState.Id}] All creatures have acted!");
            return false;
        }

        private bool PlayerHasEnoughManaToPlayACard()
        {
            foreach (var card in CurrentGameState.PlayerState.Hand)
            {
                if (card.Cost <= CurrentGameState.PlayerState.ManaPoints)
                {
                    Debug.WriteLine($"[Player {CurrentGameState.PlayerState.Id}] {CurrentGameState.PlayerState.ManaPoints} is enough mana for card {card.Id} (cost: {card.Cost})");
                    return true;
                }
            }

            Debug.WriteLine($"[Player {CurrentGameState.PlayerState.Id}] can't summon anything!");
            return false;
        }

        public bool PlayerCanMove()
        {
            if (SummonedCardsCanMove())
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
        private void ClickBlocker_Click(object sender, MouseButtonEventArgs e)
        {
            if (ViewingCard)
            {
                var fullcards = Layout.Canvas.Children.OfType<FullCard>().ToArray();
                for (int i = 0; i < fullcards.Count(); i++)
                {
                    Layout.Canvas.Children.Remove(fullcards[i]);
                }

                ClickBlocker.IsHitTestVisible = false;
                ViewingCard = false;
            }
            else
            {

            }
            e.Handled = true;
        }

        private void EndTurnButton_Click(object sender, RoutedEventArgs e)
        {
            QuivalTurn turn = new()
            {
                Trigger = TriggerType.EndTurn
            };

            Client.SubmitTurn(turn);

            CastSpellButton.Content = "Cast";
            SelectedCard = null;
        }

        private void CastCard(HandCard hc)
        {
            CurrentTurn = new()
            {
                Trigger = TriggerType.Cast,
                CardToPlayId = hc.Id
            };

            Client.SubmitTurn(CurrentTurn);

            QuivalColour.ChangetoPurpleHighlights();
            CastSpellButton.Content = "Cast";
            SelectedCard = null;
        }

        private void BoardCard_Clicked(object? sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is BoardCard bc)
                {
                    if (!CanClickCards) return;

                    if (CardSelector != null)
                    {
                        HandleCardSelectionClick(bc.Id);

                        e.Handled = true; //So we don't trigger other click events
                    }
                    else if (IsInBlockZone(bc))
                    {

                    }
                }
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (sender is BoardCard bc)
                {
                    var fullcard = Mapper.MapToFullCard((Card)bc.Tag);
                    if (fullcard != null)
                    {
                        Position pos = new() { Top = 200, Left = 200 };
                        fullcard.SetPos(pos);
                        Layout.Canvas.Children.Add(fullcard);

                        ViewingCard = true;
                        ClickBlocker.IsHitTestVisible = true;
                    }
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
                if (CombatZones[PlayerSide].CardIsSummonedByPlayer(card))
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
            if (SelectedCard != null && SelectedCard is BoardCard bc)
            {
                QuivalTurn turn = new()
                {
                    Trigger = TriggerType.MoveToBlockZone,
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
            if (SelectedCard != null && SelectedCard is BoardCard bc)
            {
                if (CombatZones[PlayerSide].CardIsSummonedByPlayer(bc))
                {
                    QuivalTurn turn = new()
                    {
                        Trigger = TriggerType.Attack,
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
                    var originalCard = ((Card)card.Tag);
                    int cost = originalCard.Cost;
                    int mana = CurrentGameState.PlayerState.ManaPoints;

                    if (cost > mana)
                        return;

                    UnselectAll();

                    //cant summon card if summon zone is full
                    if (originalCard is CreatureCard)
                        if (CombatZones[PlayerSide].GetNumberOfSummonedCards() >= MaxSummonedCards)
                            return;

                    card.Highlight();
                    SelectedCard = card;
                }
            }
        }
        #endregion

        public void MessageSent()
        {
            ClickBlocker.IsHitTestVisible = true;
            CanClickCards = false; }

        public void MessageRecieved()
        {
            ClickBlocker.IsHitTestVisible = false;
            CurrentTurn = null;
            CanClickCards = true;
        }

        public void MakeSelections(MakeSelections ms)
        {
            CardSelector = new(ms.TargetSelections);
            Layout.Canvas.Children.Add(CardSelector);
            Canvas.SetLeft(CardSelector, 100);
            Canvas.SetTop(CardSelector, 100);

            ClickBlocker.IsHitTestVisible = false;
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
        }

        public HandCard? GetHandCard(int cardId)
        {
            foreach (var bc in Layout.Canvas.Children.OfType<HandCard>())
            {
                if (bc.Id == cardId)
                    return bc;
            }

            return null;
        }
    }
}
