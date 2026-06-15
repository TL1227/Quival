using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using QuivalLogicEngine.States;
using QuivalLogicEngine.Turns;

namespace QuivalLogicEngine;

public class Match
{
    private List<Player> Players { get; set; }
    private List<ICardIntent> CardIntents { get; set; }
    private BoardState BoardState { get; set; }
    private List<EventMessage> EventMessages { get; set; }
    private int TurnCount { get; set; }
    private int RoundCount { get; set; }

    //NOTE: We start at 1 so we can keep '0' as an unassigned card id
    private int CardIdTotal = 1;

    private List<Card> MatchCards;
    private int MaxRounds = 5;

    private bool OnePlayerMode = false;

    public Match()
    {
        Players = new();
        CardIntents = new();
        BoardState = new();
        TurnCount = 1;
        RoundCount = 1;
        MatchCards = new();
        EventMessages = new();

        //TEMP for testing
        CardParser.ParseSomeCards("C:\\Users\\lavelle.t\\Projects\\Personal\\Quival\\cards.json");
    }

    public bool PlayerHasSetTurn(int playerId)
    {
        return Players[playerId].SubmittedTurn != null;
    }

    public ClientGameState GetGameState(int playerId)
    {
        PlayerState ps = new()
        {
            Id = Players[playerId].Id,
            HealthPoints = Players[playerId].HealthPoints,
            ManaPoints = Players[playerId].Mana,
            Hand = Players[playerId].Hand,
            Deck = Players[playerId].Deck,
            CardToPlay = Players[playerId].CardToPlay,
            BlockingCreature = Players[playerId].BlockingCreature
        };

        ClientGameState state = new()
        {
            PlayerState = ps,
            BoardState = BoardState,
            CardIntents = CardIntents,
            TurnCount= TurnCount,
            RoundCount= RoundCount,
            GameEvents = EventMessages
        };

        Player opponent = GetOpponent(playerId);
        state.OpponentId = opponent.Id;

        if (OnePlayerMode)
        {
            state.OpponentCardCount = 0;
            state.OpponentHealthPoints = 20;
        }
        else
        {
            state.OpponentCardCount = opponent.Hand.Count();
            state.OpponentHealthPoints = opponent.HealthPoints;
            state.OpponentManaPoints = opponent.Mana;
            state.OpponentBlockCard = opponent.BlockingCreature;
            state.OpponentCardToPlay = opponent.CardToPlay;
        }

        return state;
    }

    private Player GetOpponent(int playerId) 
    {
        if (OnePlayerMode)
            return Players[playerId];

        int opponentId = playerId == 0 ? 1 : 0;
        return Players[opponentId];
    }

    public void SetPlayer(int id, List<Card> deck)
    {
        SetCardIds(deck);
        MatchCards.AddRange(deck);

        Player player = new Player(id, deck)
        {
            //Mana = 1
            Mana = 10
        };


        Players.Add(player);
    }

    public void SubmitTurn(int playerId, QuivalTurn turn)
    {
        if (turn.Trigger == Trigger.EndTurn)
        {
            Players[playerId].SubmittedTurn = turn;
            return;
        }

        var cardToPlay = GetCardFromId(turn.CardToPlayId);

        if (cardToPlay == null)
        {
            Console.WriteLine($"Can't find player {playerId}'s card {turn.CardToPlayId} in current match");
            return;
        }

        List<CardAction> actionsThatRequireSelection = new();
        if (turn.Trigger == Trigger.Cast)
        {
            if (cardToPlay.Cost <= Players[playerId].Mana)
            {
                Players[playerId].SubmittedTurn = turn;
                Players[playerId].Hand.Remove(cardToPlay);
                Players[playerId].SubmittedTurn = turn;

                var actions = GetActionsThatRequireSelection(cardToPlay, Trigger.Cast);

                if (actions != null)
                    actionsThatRequireSelection.AddRange(actions);
            }
            else
            {
                Console.WriteLine($"player {playerId} doesn't have enough mana for {turn.CardToPlayId}");
                Console.WriteLine($"they have {Players[playerId].Mana} and need {cardToPlay.Cost}");
            }
        }
        else if (turn.Trigger == Trigger.Attack)
        {
            Players[playerId].SubmittedTurn = turn;

            actionsThatRequireSelection.AddRange(GetActionsThatRequireSelection(cardToPlay, Trigger.Attack)!);
        }

        if (actionsThatRequireSelection.Count > 0)
        {
            Players[playerId].MakingSelections = true;
            GetTargetsForSelection(playerId, actionsThatRequireSelection, turn.Trigger);
        }
    }

    public void SubmitTargetSelection(int playerId, List<TargetSelection> selections)
    {
        Players[playerId].MakingSelections = false;
        Players[playerId].TargetSelections = selections;
    }

    public List<CardAction>? GetActionsThatRequireSelection(Card card, Trigger trigger)
    {
        var ability = card.Abilities.SingleOrDefault(a => a.Trigger == trigger);
        if (ability != null)
        {
            return ability.Actions.Where(a => a.NumberOfTargets > 0).ToList();
        }
        else
        {
            return null;
        }
    }

    public void GetTargetsForSelection(int playerId, List<CardAction> actions, Trigger trigger)
    {
        foreach (var action in actions)
        {
            if (action.TargetType == TargetType.Damageable)
            {
                TargetSelection ts = new();
                ts.TargetsToPickFrom.AddRange(BoardState.GetAllSummonedCreatures().Select(c => c.Id));
                ts.TargetType = TargetType.Damageable;
                ts.NumberToPick = action.NumberOfTargets;
                ts.Trigger = trigger;
                ts.CardAction = action;
                Players[playerId].TargetSelections.Add(ts);
            }
        }
    }

    public List<TargetSelection>? GetSelectionsIfPlayerNeedsThem(int playerId)
    {
        if (Players[playerId].MakingSelections)
            return Players[playerId].TargetSelections;
        else
            return null;
    }

    public bool BothPlayersHaveSubmittedTurns()
    {
        if (OnePlayerMode)
            return true;

        foreach (var player in Players)
        {
            if (player.SubmittedTurn == null)
            {
                Console.WriteLine($"player {player.Id} hasn't submitted a turn!");
                return false;
            }
        }

        return true;
    }

    public bool BothPlayersHaveSubmittedTargets()
    {
        if (OnePlayerMode)
            return true;

        foreach (var player in Players)
        {
            if (player.MakingSelections == true)
                return false;
        }

        return true;
    }


    public void SetCardIds(List<Card> deck)
    {
        foreach (var card in deck)
        {
            card.Id = CardIdTotal++;
            Console.WriteLine($"[EVENT] Card {card.Name} assigned Id {card.Id}");
        }
    }

    private Card? GetCardFromId(int cardId)
    {
        return MatchCards.SingleOrDefault(c => c.Id == cardId);
    }

    private bool BothPlayersHaveEndedTheirTurn()
    {
        return Players[0].SubmittedTurn!.Trigger == Trigger.EndTurn &&
                Players[1].SubmittedTurn!.Trigger == Trigger.EndTurn;
    }

    public void ProcessCards()
    {
        Console.WriteLine("Processing submitted Turns!");
        
        EventMessages.Clear();
        CardIntents.Clear();

        if (Players[0].SubmittedTurn == null || Players[1].SubmittedTurn == null)
        {
            return;
        }

        if (BothPlayersHaveEndedTheirTurn())
        {
            EventMessage(new BothPlayersOutOfMovesEvent());
            Players[0].SubmittedTurn = null;
            Players[1].SubmittedTurn = null;
            NextTurn();
            return;
        }

        EventMessage(new NewRound(RoundCount));

        foreach (var player in Players)
        {
           player.CardToPlay = GetCardFromId(player.SubmittedTurn!.CardToPlayId);
        }

        ProcessCasts();
        ProcessMoveToBlockZone();
        ProcessAttacks();

        //round end
        RoundCount++;
        foreach (var player in Players)
        {
            player.SubmittedTurn = null;
            player.TargetSelections.Clear();
        }

        //remove attack buffs
        foreach (var creatures in BoardState.SummonedCreatures)
            foreach (var creature in creatures)
                creature.AttackBuff = 0;


        //TODO: I think that we should do the calculating which players can and can't move next turn here
        //currently it's all being done client side which is stupid

        if (RoundCount > MaxRounds)
        {
            NextTurn();
        }
    }

    private void ProcessCasts()
    {
        foreach (var player in Players)
        {
            if (player.SubmittedTurn!.Trigger != Trigger.Cast)
                continue;

            if (player.CardToPlay != null)
            {
                player.Mana -= player.CardToPlay.Cost;

                if (player.CardToPlay is CreatureCard creature)
                {
                    if (BoardState.CreatureSlotFree(player.Id))
                    {
                        BoardState.SummonCreature(player.Id, creature);

                        EventMessage(new SummonEvent(player.Id, creature.Id, creature.Name!));

                        creature.HasActed = true;
                        creature.CurrentHealth = creature.Health;

                        var targets = player.TargetSelections.Where(ts => ts.Trigger == player.SubmittedTurn.Trigger).ToList();
                        ProcessTriggeredAbilities(player.Id, player.CardToPlay, player.SubmittedTurn.Trigger, targets);
                    }
                    else
                    {
                        Console.WriteLine($"[ERROR]: Not enough room on {creature.PlayerId} board for {creature.Id}");
                    }
                }
                else if (player.CardToPlay is SpellCard spell)
                {
                    EventMessage(new CastEvent(player.Id ,spell));
                    var targets = player.TargetSelections.Where(ts => ts.Trigger == player.SubmittedTurn.Trigger).ToList();
                    ProcessTriggeredAbilities(player.Id, spell, Trigger.Cast, targets);
                }
            }
        }
    }
    
    private void ProcessAttacks()
    {
        foreach (var player in Players)
        {
            if (player.SubmittedTurn!.Trigger != TurnType.Attack)
                continue;

            if (player.CardToPlay is CreatureCard attackingCreature && attackingCreature.IsAlive())
            {
                Player otherPlayer = GetOpponent(player.Id);

                //ProcessTriggeredAbilities(player.Id, player.CardToPlay, Trigger.Attack, player.TargetSelections);

                var attackEvent = new AttackEvent(player.Id, attackingCreature.Id, attackingCreature.Name!);

                if (otherPlayer.BlockingCreature == null || otherPlayer.BlockingCreature.IsDead())
                {
                    otherPlayer.HealthPoints -= attackingCreature.GetAttackDamage();
                }
                else
                {
                    var blockingCreature = otherPlayer.BlockingCreature;

                    attackEvent.BlockingCreatureId = blockingCreature.Id;

                    blockingCreature.CurrentHealth -= attackingCreature.GetAttackDamage();
                    if (blockingCreature.CurrentHealth <= 0)
                    {
                        EventMessage(new CreatureDeathEvent(blockingCreature.Id, blockingCreature.Name!));
                        otherPlayer.BlockingCreature = null;
                    }

                    attackingCreature.CurrentHealth -= blockingCreature.GetAttackDamage();
                    if (attackingCreature.CurrentHealth <= 0)
                    {
                        EventMessage(new CreatureDeathEvent(attackingCreature.Id, attackingCreature.Name!));
                        BoardState.SummonedCreatures[player.Id].Remove(attackingCreature);
                    }
                }
                attackingCreature.HasActed = true;

                EventMessage(attackEvent);
                //Possible after damage trigger could go here
            }
        }
    }

    private void ProcessMoveToBlockZone()
    {
        foreach (var player in Players)
        {
            if (player.SubmittedTurn!.Trigger != TurnType.MoveToBlock)
                continue;

            if (player.CardToPlay is CreatureCard newBlocker && newBlocker.IsAlive())
            {
                if (player.BlockingCreature == null)
                {
                    player.BlockingCreature = newBlocker;
                    BoardState.SummonedCreatures[player.Id].Remove(newBlocker);

                    EventMessage(new MoveToBlockZoneEvent(player.Id, newBlocker.Id, newBlocker.Name!));

                    ProcessTriggeredAbilities(player.Id, player.CardToPlay, Trigger.MoveToBlockZone, player.SubmittedTurn.SelectedCardIds);
                }
                else
                {
                    int index = BoardState.SummonedCreatures[player.Id].IndexOf(newBlocker);
                    BoardState.SummonedCreatures[player.Id][index] = player.BlockingCreature;

                    var oldBlocker = player.BlockingCreature;
                    player.BlockingCreature = newBlocker;
                    BoardState.SummonedCreatures[player.Id].Remove(newBlocker);

                    EventMessage(new BlockSwapEvent(player.Id, newBlocker.Id, newBlocker.Name!, oldBlocker.Id, oldBlocker.Name!));

                    ProcessTriggeredAbilities(player.Id, player.CardToPlay, Trigger.BlockSwap, player.SubmittedTurn.SelectedCardIds);
                }
            }
        }
    }

    //abilities
    private void ProcessTriggeredAbilities(int playerId, Card card, Trigger trigger, List<TargetSelection> targetSelections)
    {
        var ability = card.Abilities.SingleOrDefault(a => a.Trigger == trigger);

        if (ability == null) 
        {
            Console.WriteLine($"Couldn't find trigger {trigger.ToString()} on the card {card.Name}");
            return;
        }

        foreach (var action in GetActions(ability))
        {
            if (ConditionalsMet(action.Conditionals))
            {
                List<Card> targets = GetTargets(card, action, targetSelections);

                ProcessAction(playerId, targets, action.Value, action.Intent);


                //EventMessage(message);
            }
        }
    }

    private List<Card> GetTargets(Card self, CardAction cardAction, List<TargetSelection> targetSelections)
    {
        List<Card> targetsResult = new();

        if (cardAction.TargetType == TargetType.Self)
        {
            targetsResult.Add(self);
        }
        else
        {
            foreach (var ts in targetSelections)
            {
                foreach (var target in ts.SelectedTargets.Where(st => st.))
                {
                    var card = GetCardFromId(target);

                    if (card != null)
                    {
                        targetsResult.Add(card);
                    }
                }
            }
        }

        return targetsResult;
    }

    private List<CardAction> GetActions(Ability ability)
    {
        List<CardAction> actions = new();

        switch (ability.ChoiceType)
        {
            case ChoiceType.And:
            {
                actions = ability.Actions;
                break;
            }
            default:
                Console.WriteLine($"The choice type {ability.ChoiceType.ToString()} has not yet been implemented");
            break;
        }

        return actions;
    }

    private void ProcessAction(int playerId, List<Card> targets, int value, Intent intent)
    {
        foreach (var target in targets)
        {
            Card? targetedCard = GetCardFromId(target.Id);

            if (targetedCard == null)
                continue;

            CardActionEvent message = new()
            {
                PlayerId = playerId,
                Intent = intent,
                TargetsCardIds = targets.Select(c => c.Id).ToList(),
                Value = value
            };

            EventMessages.Last().CardActionEvents.Add(message);

            if (targetedCard is CreatureCard targetCreature)
            {
                switch (intent)
                {
                    case Intent.AttackBuff:
                        {
                            targetCreature.AttackBuff += value;
                            break;
                        }
                    case Intent.DirectDamage:
                        {
                            bool hasDied = targetCreature.DamageCreature(value);

                            if (hasDied)
                            {
                                CreatureDeathEvent death = new(targetCreature.Id, targetCreature.Name!);
                                EventMessage(death);
                            }

                            break;
                        }
                    case Intent.Heal:
                        {
                            targetCreature.HealCreature(value);
                            break;
                        }
                }
            }
        }
    }

    private bool ConditionalsMet(List<Conditional> conditionals)
    {
        if (conditionals.Count <= 0)
            return true;

        List<bool> passes = new();
        foreach (var condition in conditionals)
        {
            switch (condition)
            {
                case Conditional.None: passes.Add(true); break;
                case Conditional.Round1: passes.Add(RoundCount == 1); break;
                case Conditional.Round2: passes.Add(RoundCount == 2); break;
                case Conditional.Round3: passes.Add(RoundCount == 3); break;
                case Conditional.Round4: passes.Add(RoundCount == 4); break;
                case Conditional.Round5: passes.Add(RoundCount == 5); break;
                default: 
                    passes.Add(true); 
                    Console.WriteLine($"The condition {condition.ToString()} has not yet been implemented");
                break;
            }
        }

        return !passes.Contains(false); 
    }

    private void NextTurn()
    {
        TurnCount++;
        RoundCount = 1;
        EventMessage(new NewTurn(TurnCount, RoundCount)); //TODO: do we need this?

        foreach (var card in MatchCards)
        {
            if (card is CreatureCard cc)
                cc.HasActed = false;
        }

        foreach (var player in Players)
        {
            player.Mana = BoardState.GetCurrentMana();
            player.DrawCard(1);
        }

        BoardState.IncreaseManaClock();
    }

    private void EventMessage(EventMessage message)
    {
        Console.WriteLine(message.GetString());
        EventMessages.Add(message);
    }
}
