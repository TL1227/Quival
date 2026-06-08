using QuivalCombatTestWPF.Colours;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using QuivalLogicEngine.Turns;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

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

        public MainWindow()
        {
            InitializeComponent();

            PlayerBlockZone.Side = Side.Player;
            OpponentBlockZone.Side = Side.Opponent;

            HandZone.CardClicked += HandZone_CardClicked;
            PlayerCombatZone.CardClicked += CombatZone_CardClicked;
            PlayerCombatZone.PlayerZoneClicked += CombatZone_PlayerZoneClicked;
            PlayerBlockZone.ZoneClicked += PlayerBlockZone_ZoneClicked;
            OpponentBlockZone.ZoneClicked += OpponentBlockZone_ZoneClicked;
            ClickBlocker.MouseLeftButtonDown += ClickBlocker_MouseLeftButtonDown;
            EndTurnButton.Click += EndTurnButton_Click;

            CombatZones = [PlayerCombatZone, OpponentCombatZone];
            BlockZones = [PlayerBlockZone, OpponentBlockZone];
            SummonZones = [PlayerSummonZone, OpponentSummonZone];

            Layout.Loaded += (_, _) =>
            {
                Client = new QuivalClient(this);
                Client.ConnectToServer();
            };
        }

        #region Animation
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
            List<List<BlockSwapEvent>> events = [
                blockSwapEvents.Where(e => e.PlayerId == MyPlayerId).ToList(),
                blockSwapEvents.Where(e => e.PlayerId != MyPlayerId).ToList()
            ];

            for (int i = 0; i < events.Count; i++)
            {
                foreach (var blockSwapEvent in events[i])
                {
                    var newCreature = CombatZones[i].GetBoardCard(blockSwapEvent.CreatureId);
                    var oldCreature = BlockZones[i].GetCardFromBlockZone();

                    if (newCreature != null && oldCreature != null)
                    {
                        List<Task> tasks = new();

                        tasks.Add(Animation.MoveToPoint(newCreature, newCreature.GetPos(), Layout.BlockAreas[i]));
                        int removedCardIndex = CombatZones[i].RemoveCardFromZone(newCreature.Id, Layout);
                        BlockZones[i].AddCardToBlockZone(newCreature, Layout, Layout.BlockAreas[i]);

                        tasks.Add(Animation.MoveToPoint(oldCreature, oldCreature.GetPos(), newCreature.GetPos()));
                        CombatZones[i].SummonedCards[removedCardIndex] = oldCreature;

                        await Task.WhenAll(tasks);
                    }
                }
            }
        }

        private async Task PlayBlockAnimations(List<MoveToBlockZoneEvent> blockEvents)
        {
            List<List<MoveToBlockZoneEvent>> events = [
                blockEvents.Where(e => e.PlayerId == MyPlayerId).ToList(),
                blockEvents.Where(e => e.PlayerId != MyPlayerId).ToList()
            ];

            for (int i = 0; i < events.Count; i++)
            {
                foreach (var blockEvent in events[i])
                {
                    foreach (var summonedCard in CombatZones[i].SummonedCards)
                    {
                        if (summonedCard != null && summonedCard.Id == blockEvent.CreatureId)
                        {
                            await Animation.MoveToPoint(summonedCard, summonedCard.GetPos(), Layout.BlockAreas[i]);
                            CombatZones[i].RemoveCardFromZone(summonedCard.Id, Layout);
                            BlockZones[i].AddCardToBlockZone(summonedCard, Layout, Layout.BlockAreas[i]);
                        }
                    }
                }
            }
        }

        private async Task PlayAttackAnimations(List<AttackEvent> attackEvents)
        {
            List<List<AttackEvent>> events = [
                attackEvents.Where(e => e.PlayerId == MyPlayerId).ToList(),
                attackEvents.Where(e => e.PlayerId != MyPlayerId).ToList()
            ];

            for (int i = 0; i < events.Count; i++)
            {
                foreach (var eve in events[i])
                {
                    foreach (var summonedCard in CombatZones[i].SummonedCards)
                    {
                        if (summonedCard != null && summonedCard.Id == eve.CreatureId)
                        {
                            await Animation.MoveToPointAndBack(summonedCard, summonedCard.GetPos(), Layout.BlockAreas[OppositeSide(i)]);
                        }
                    }
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

        private async Task PlaySummonAnimations(List<SummonEvent> summonEvents)
        {
            List<Task> tasks = new();

            foreach (var se in summonEvents.Where(e => e.PlayerId == MyPlayerId).ToList())
            {
                BoardCard? cardToSummon = null;
                foreach (CreatureCard card in CurrentGameState.BoardState.SummonedCreatures[(int)MyPlayerId]) 
                {
                    if (card.Id == se.CreatureId)
                        cardToSummon = Mapper.MapToBoardCard(card);
                }

                if (cardToSummon != null)
                {
                    Position handcardPos = new();
                    foreach (var child in Layout.Canvas.Children)
                    {
                        if (child is HandCard handCard)
                        {
                            if (handCard.Id == cardToSummon.Id)
                            {
                                handcardPos.Left = Canvas.GetLeft(handCard);
                                handcardPos.Top = Canvas.GetTop(handCard);
                                handCard.Visibility = Visibility.Hidden;
                            }
                        }
                    }

                    cardToSummon.SetPos(handcardPos);
                    int summonIndex = CombatZones[0].AddCardToNextFreeSlot(cardToSummon, Layout);
                    tasks.Add(Animation.MoveToPoint(cardToSummon, handcardPos, Layout.PlayerSummonSlots[summonIndex]));
                }
            }

            await Task.WhenAll(tasks);
            tasks.Clear();

            foreach (var se in summonEvents.Where(e => e.PlayerId != MyPlayerId).ToList())
            {
                BoardCard? cardToSummon = null;
                foreach (CreatureCard card in CurrentGameState.BoardState.SummonedCreatures[OppositeSide((int)MyPlayerId)]) 
                {
                    if (card.Id == se.CreatureId)
                        cardToSummon = Mapper.MapToBoardCard(card);
                }

                if (cardToSummon != null)
                {
                    Position opponentCardStartPos = new() { Top = -200, Left = Layout.ActualWidth / 2 };
                    cardToSummon.SetPos(opponentCardStartPos);
                    int summonIndex = CombatZones[0].AddCardToNextFreeSlot(cardToSummon, Layout);
                    tasks.Add(Animation.MoveToPoint(cardToSummon, opponentCardStartPos, Layout.OpponentSummonSlots[summonIndex]));
                }
            }

            await Task.WhenAll(tasks);

        }
        #endregion

        public int OppositeSide(int i) => i == PlayerSide ? OpponentSide : PlayerSide;

        #region StateUpdates
        public void UpdateUIFromGameState()
        {
            Debug.WriteLine($"Gamestate update: Turn {CurrentGameState.TurnCount} round {CurrentGameState.RoundCount}");
            var gs = CurrentGameState;

            //TODO: rework this with the new layout canvas
            CombatZones[PlayerSide].UpdateCombatZone(gs.BoardState.SummonedCreatures[gs.PlayerState.Id], Layout);
            CombatZones[OpponentSide].UpdateCombatZone(gs.BoardState.SummonedCreatures[gs.OpponentId], Layout);

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
                    var blockingCreature = Mapper.MapToBoardCard(cc);
                    BlockZones[i].AddCardToBlockZone(blockingCreature, Layout, Layout.BlockAreas[i]);
                }
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

            //use the smallest number for the for loop
            int length = cards.Count >= Layout.HandSlots.Length 
                ? Layout.Name.Length 
                : cards.Count;

            for (int i = 0; i < length; i++)
            {
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

        private void CombatZone_PlayerZoneClicked(object? sender, EventArgs e)
        {
            if (SelectedCard != null && SelectedCard is HandCard hc)
            {
                QuivalTurn turn = new()
                {
                    TurnType = TurnType.Cast,
                    CardToPlayId = hc.Id
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
                else if (CombatZones[PlayerSide].CardIsSummonedByPlayer(card))
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
                //TODO: get this from some card lookup, not the client UI
                int cost = int.Parse(card.CostContent.Content.ToString()!);
                int mana = CurrentGameState.PlayerState.ManaPoints;

                if (cost > mana)
                    return;

                UnselectAll();

                if (CombatZones[PlayerSide].GetNumberOfSummonedCards() < MaxSummonedCards)
                {
                    card.Overlay.Opacity = 0.4;
                    SelectedCard = card;

                    CombatZones[PlayerSide].Highlight(true);
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
                        if (boardCard != null && boardCard.Id == cardId)
                            return boardCard;

            List<BoardCard> combatZoneCards = new();
            combatZoneCards.AddRange(PlayerCombatZone.GetBoardCards());
            combatZoneCards.AddRange(OpponentCombatZone.GetBoardCards());

            foreach (var card in combatZoneCards)
                        if (card != null && card.Id == cardId)
                            return card;

            return null;
        }
        public HandCard? GetHandCard(int cardId)
        {
            foreach (var card in HandZone.HandGrid.Children)
                    if (card is HandCard handCard)
                        if (handCard != null && handCard.Id == cardId)
                            return handCard;

            return null;
        }
    }
}
