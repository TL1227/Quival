using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using QuivalLogicEngine.States;
using QuivalLogicEngine.Turns;

namespace QuivalLogicEngine;

public class Match
{
    private List<Player> Players { get; set; }
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
            Console.WriteLine($"Moving to turn {TurnCount}");
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
            Mana = 1
            //Mana = 10
        };


        Players.Add(player);
    }

    public void SubmitTurn(int playerId, QuivalTurn turn)
    {
        if (turn.Trigger == TriggerType.EndTurn)
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

        if (turn.Trigger == TriggerType.Cast)
        {
            if (cardToPlay.Cost <= Players[playerId].Mana)
            {
                Players[playerId].SubmittedTurn = turn;
                Players[playerId].Hand.Remove(cardToPlay);
            }
            else
            {
                Console.WriteLine($"player {playerId} doesn't have enough mana for {turn.CardToPlayId}");
                Console.WriteLine($"they have {Players[playerId].Mana} and need {cardToPlay.Cost}");
            }
        }
        else
        {
            Players[playerId].SubmittedTurn = turn;
        }

        List<Ability> abilitiesThatRequireSelection = new();
        var abilities = GetSelectableAbilities(cardToPlay, turn.Trigger);
        if (abilities != null)
        {
            foreach (var ability in abilities)
            {
                if (ConditionalsMet(ability.Conditionals))
                {
                    abilitiesThatRequireSelection.AddRange(abilities);
                }
            }
        }

        if (abilitiesThatRequireSelection.Count > 0)
        {
            Players[playerId].MakingSelections = true;
            GetTargetsForSelection(playerId, cardToPlay.Id, abilitiesThatRequireSelection, turn.Trigger);
        }
    }

    public void SubmitTargetSelection(int playerId, List<TargetSelection> selections)
    {
        Players[playerId].MakingSelections = false;
        Players[playerId].TargetSelections = selections;
    }

    public List<Ability>? GetSelectableAbilities(Card card, TriggerType triggerType)
    {
        var trigger = card.Triggers.SingleOrDefault(a => a.TriggerType == triggerType);
        if (trigger != null)
        {
            return trigger.Abilities.Where(a => a.NumberOfTargets > 0).ToList();
        }
        else
        {
            return null;
        }
    }

    public void GetTargetsForSelection(int playerId, int cardId, List<Ability> abilities, TriggerType trigger)
    {
        foreach (var ability in abilities)
        {
            if (ability.TargetType == TargetType.Damageable)
            {
                TargetSelection ts = new();
                ts.TargetsToPickFrom.AddRange(BoardState.GetAllSummonedCreatures().Select(c => c.Id));

                if (!ability.CanTargetSelf)
                    ts.TargetsToPickFrom.Remove(cardId);

                ts.TargetType = TargetType.Damageable;
                ts.NumberToPick = ability.NumberOfTargets;
                ts.Trigger = trigger;
                ts.AbilityId = ability.Id;
                ts.CardId = cardId;
                ts.Effect = ability.Effect;
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
        return Players[0].SubmittedTurn!.Trigger == TriggerType.EndTurn &&
                Players[1].SubmittedTurn!.Trigger == TriggerType.EndTurn;
    }

    public void ProcessCards()
    {
        EventMessages.Clear();

        if (Players[0].SubmittedTurn == null || Players[1].SubmittedTurn == null)
            return;

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

        //NextRound()
        RoundCount++;
        foreach (var player in Players)
        {
            player.SubmittedTurn = null;
            player.TargetSelections.Clear();

            //TODO: I think that we should do the calculating which players can and can't move next turn here
            //currently it's all being done client side which is stupid
        }

        //remove attack buffs
        foreach (var creatures in BoardState.SummonedCreatures)
            foreach (var creature in creatures)
                creature.AttackBuff = 0;

        if (RoundCount > MaxRounds)
        {
            NextTurn();
        }
    }

    private void ProcessCasts()
    {
        foreach (var player in Players)
        {
            if (player.SubmittedTurn!.Trigger != TriggerType.Cast)
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
                        ProcessCardTriggers(player.Id, creature, player.SubmittedTurn.Trigger, targets);
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
                    ProcessCardTriggers(player.Id, spell, player.SubmittedTurn.Trigger, targets);
                }
            }
        }
    }
    
    private void ProcessAttacks()
    {
        foreach (var player in Players)
        {
            if (player.SubmittedTurn!.Trigger != TriggerType.Attack)
                continue;

            if (player.CardToPlay is CreatureCard attackingCreature && attackingCreature.IsAlive())
            {
                Player otherPlayer = GetOpponent(player.Id);

                var attackEvent = new AttackEvent(player.Id, attackingCreature.Id, attackingCreature.Name!);
                EventMessage(attackEvent);
                ProcessCardTriggers(player.Id, player.CardToPlay, player.SubmittedTurn.Trigger, player.TargetSelections);

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
                    else
                    {
                        //TODO: maybe trigger blocking ability?
                    }

                    attackingCreature.CurrentHealth -= blockingCreature.GetAttackDamage();
                    if (attackingCreature.CurrentHealth <= 0)
                    {
                        EventMessage(new CreatureDeathEvent(attackingCreature.Id, attackingCreature.Name!));
                        BoardState.SummonedCreatures[player.Id].Remove(attackingCreature);
                    }
                }
                attackingCreature.HasActed = true;
            }
        }
    }

    private void ProcessMoveToBlockZone()
    {
        foreach (var player in Players)
        {
            if (player.SubmittedTurn!.Trigger != TriggerType.MoveToBlockZone)
                continue;

            if (player.CardToPlay is CreatureCard newBlocker && newBlocker.IsAlive())
            {
                if (player.BlockingCreature == null)
                {
                    player.BlockingCreature = newBlocker;
                    BoardState.SummonedCreatures[player.Id].Remove(newBlocker);

                    EventMessage(new MoveToBlockZoneEvent(player.Id, newBlocker.Id, newBlocker.Name!));

                    //ProcessCardTriggers(player.Id, player.CardToPlay, TriggerType.MoveToBlockZone, player.SubmittedTurn.SelectedCardIds);
                }
                else
                {
                    int index = BoardState.SummonedCreatures[player.Id].IndexOf(newBlocker);
                    BoardState.SummonedCreatures[player.Id][index] = player.BlockingCreature;

                    var oldBlocker = player.BlockingCreature;
                    player.BlockingCreature = newBlocker;
                    BoardState.SummonedCreatures[player.Id].Remove(newBlocker);

                    EventMessage(new BlockSwapEvent(player.Id, newBlocker.Id, newBlocker.Name!, oldBlocker.Id, oldBlocker.Name!));

                    //ProcessCardTriggers(player.Id, player.CardToPlay, TriggerType.BlockSwap, player.SubmittedTurn.SelectedCardIds);
                }
            }
        }
    }

    //abilities
    private void ProcessCardTriggers(int playerId, Card card, TriggerType triggerType, List<TargetSelection> targetSelections)
    {
        var trigger = card.Triggers.SingleOrDefault(a => a.TriggerType == triggerType);

        if (trigger == null) 
        {
            Console.WriteLine($"Couldn't find trigger {triggerType.ToString()} on the card {card.Name}");
            return;
        }

        foreach (var ability in GetAbilities(trigger))
        {
            if (ConditionalsMet(ability.Conditionals))
            {
                List<Card> targets = GetTargets(card, ability, targetSelections);

                ProcessAbility(playerId, targets, ability.Value, ability.Effect);


                //EventMessage(message);
            }
        }
    }

    private List<Card> GetTargets(Card cardToPlay, Ability ability, List<TargetSelection> targetSelections)
    {
        List<Card> targetsResult = new();

        if (ability.TargetType == TargetType.Self)
        {
            targetsResult.Add(cardToPlay);
        }
        else
        {
            var targetselection = targetSelections.SingleOrDefault(ts => ts.CardId == cardToPlay.Id && ts.AbilityId == ability.Id);
            if (targetselection != null)
            {
                foreach (var target in targetselection.SelectedTargets)
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

    private List<Ability> GetAbilities(Trigger trigger)
    {
        List<Ability> actions = new();

        switch (trigger.ChoiceType)
        {
            case ChoiceType.And:
            {
                actions = trigger.Abilities;
                break;
            }
            default:
                Console.WriteLine($"The choice type {trigger.ChoiceType.ToString()} has not yet been implemented");
            break;
        }

        return actions;
    }

    private void ProcessAbility(int playerId, List<Card> targets, int value, Effect intent)
    {
        foreach (Card target in targets)
        {
            CardActionEvent message = new()
            {
                PlayerId = playerId,
                Effect = intent,
                TargetsCardIds = targets.Select(c => c.Id).ToList(),
                Value = value
            };

            EventMessages.Last().CardActionEvents.Add(message);

            if (target is CreatureCard targetCreature)
            {
                switch (intent)
                {
                    case Effect.AttackBuff:
                        {
                            targetCreature.AttackBuff += value;
                            break;
                        }
                    case Effect.DirectDamage:
                        {
                            bool hasDied = targetCreature.DamageCreature(value);

                            if (hasDied)
                            {
                                CreatureDeathEvent death = new(targetCreature.Id, targetCreature.Name!);
                                EventMessage(death);
                            }

                            break;
                        }
                    case Effect.Heal:
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
            {
                if (cc.CurrentHealth <= 0)
                {
                    BoardState.RemoveCreatureFromBoard(cc);
                }
                else
                {
                    cc.HasActed = false;
                }
            }
        }

        foreach (var player in Players)
        {
            player.Mana = BoardState.GetCurrentMana();
            player.DrawCard(1);
            player.MakingSelections = false;
        }

        BoardState.IncreaseManaClock();
    }

    private void EventMessage(EventMessage message)
    {
        Console.WriteLine(message.GetString());
        EventMessages.Add(message);
    }
}
