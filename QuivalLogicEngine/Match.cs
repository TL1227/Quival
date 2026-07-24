using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Client;
using QuivalLogicEngine.States;
using QuivalLogicEngine.Turns;

namespace QuivalLogicEngine;


public class Match
{
    public List<Player> Players { get; set; }
    public BoardState BoardState { get; set; }
    public List<Card> MatchCards;
    private List<EventMessage> CurrentRoundsEvents { get; set; }
    private List<EventMessage> CurrentTurnsEvents { get; set; }
    private int TurnCount { get; set; }
    private int RoundCount { get; set; }

    //NOTE: We start at 2 so we can use 0 and 1 as cards representing the player
    private int CardIdTotal = 2;

    private int MaxRounds = 5;

    private bool OnePlayerMode = false;

    public Match()
    {
        Players = new();
        BoardState = new();
        TurnCount = 1;
        RoundCount = 1;
        MatchCards = new();
        CurrentRoundsEvents = new();
        CurrentTurnsEvents = new();
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
            GameEvents = CurrentRoundsEvents
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

    public Player GetOpponent(int playerId) 
    {
        if (OnePlayerMode)
            return Players[playerId];

        int opponentId = playerId == 0 ? 1 : 0;
        return Players[opponentId];
    }

    public static int GetOpponentId(int playerId) 
    {
        return playerId == 0 ? 1 : 0;
    }

    public List<CreatureCard> GetAllCreaturesOnBoard()
    {
        List<CreatureCard> creatureCards = BoardState.GetAllSummonedCreatures();

        foreach (var player in Players)
            if (player.BlockingCreature != null)
                creatureCards.Add(player.BlockingCreature!);

        return creatureCards;
    }

    public void SetPlayer(int id, List<Card> deck)
    {
        SetCardIds(deck, id);
        MatchCards.AddRange(deck);

        PlayerCard pc = new() { Id = id, PlayerId = id };
        MatchCards.Add(pc);

        Player player = new Player(id, deck)
        {
            Mana = 1
        };


        Players.Add(player);
    }

    public void SubmitTurn(int playerId, QuivalTurn turn)
    {
        if (turn.TurnType == TurnType.EndTurn)
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

        if (turn.TurnType == TurnType.Cast)
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

        foreach (var ability in GetAbilitiesThatRequireSelection(cardToPlay, turn.TurnType))
        {
            if (ConditionalsMet(playerId, ability.Conditionals))
            {
                var targetSelection = GetTargetsForSelection(playerId, cardToPlay, ability);

                if (targetSelection != null)
                    Players[playerId].TargetSelections.Add(targetSelection);
            }
        }

        if (Players[playerId].TargetSelections.Count > 0)
        {
            Players[playerId].MakingSelections = true;
        }
    }

    public void SubmitTargetSelection(int playerId, List<TargetSelection> selections)
    {
        Players[playerId].MakingSelections = false;
        Players[playerId].TargetSelections = selections;
    }

    public List<Ability> GetAbilitiesThatRequireSelection(Card card, TurnType turnType)
    {
        List<Ability> abilities = new();

        Trigger? trigger = null;
        switch (turnType)
        {
            case TurnType.Cast:
                {
                    trigger = card.Triggers
                        .SingleOrDefault(a => a is CastTrigger);
                }
                break;
            case TurnType.Attack:
                {
                    trigger = card.Triggers
                        .SingleOrDefault(a => a is SelfTrigger st && st.SelfTriggerType == SelfTriggerType.Attack);
                }
                break;
            case TurnType.MoveToBlockZone:
                {
                    trigger = card.Triggers
                        .SingleOrDefault(a => a is SelfTrigger st && st.SelfTriggerType == SelfTriggerType.MoveToBlockZone);
                }
                break;
            case TurnType.EndTurn:
            default:
                break;
        }

        if (trigger != null)
        {
            return trigger.Abilities.Where(a => a.Target is SelectionTarget).ToList();
        }

        return abilities;
    }


    public TargetSelection? GetTargetsForSelection(int playerId, Card card, Ability ability)
    {
        if (ability.Target is SelectionTarget target)
        {
            TargetSelection ts = new()
            {
                TargetsToPickFrom = target.GetTargetPool(card, this),
                NumberToPick = target.NumberToPick,

                CardId = card.Id,

                AbilityId = ability.Id,
                Effect = ability.Effect
            };

            return ts;
        }

        return null;
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


    public void SetCardIds(List<Card> deck, int playerId)
    {
        foreach (var card in deck)
        {
            card.Id = CardIdTotal++;
            card.PlayerId = playerId;
            Console.WriteLine($"[EVENT] Card {card.Name} assigned Id {card.Id}");
        }
    }

    private Card? GetCardFromId(int cardId)
    {
        return MatchCards.SingleOrDefault(c => c.Id == cardId);
    }

    private bool BothPlayersHaveEndedTheirTurn()
    {
        return Players[0].SubmittedTurn!.TurnType == TurnType.EndTurn &&
                Players[1].SubmittedTurn!.TurnType == TurnType.EndTurn;
    }

    public void ProcessCards()
    {
        CurrentRoundsEvents.Clear();

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

        NextRound();
    }

    public void NextRound()
    {
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
                creature.AttackBuffRound = 0;

        CurrentTurnsEvents.AddRange(CurrentRoundsEvents);

        if (RoundCount > MaxRounds)
        {
            NextTurn();
        }
    }

    private void NextTurn()
    {
        TurnCount++;
        RoundCount = 1;

        CurrentTurnsEvents.Clear();

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
            var cardsDrawn = player.DrawCard(1);
            CurrentRoundsEvents.Add(new CardDrawEvent(cardsDrawn) { PlayerId = player.Id });
            player.MakingSelections = false;
        }

        BoardState.IncreaseManaClock();

        CheckPassiveAbilities();
    }

    public List<CreatureCard> GetAllCreatures()
    {
        List<CreatureCard> creatureCards = new List<CreatureCard>();
        creatureCards.AddRange(BoardState.GetAllSummonedCreatures());

        foreach (var player in Players)
            if (player.BlockingCreature != null)
                creatureCards.Add(player.BlockingCreature);

        return creatureCards;
    }

    private void CheckPassiveAbilities()
    {
        foreach (var creature in GetAllCreatures().Where(c => c.PassiveAbilities.Count > 0))
        {
            foreach (var ability in creature.PassiveAbilities)
            {
                ProcessAbility(creature, ability);
            }
        }
    }

    private void ProcessAbility(Card card, Ability ability)
    {
        var targets = GetTargets(card, ability);
        int value = GetValue(card.PlayerId, ability.Value);

        foreach (Card target in targets)
        {
            CardActionEvent message = new()
            {
                CardActionSource = card,
                PlayerId = card.PlayerId,
                Effect = ability.Effect,
                TargetsCardIds = targets.Select(c => c.Id).ToList(),
                Value = value
            };

            CurrentRoundsEvents.Last().CardActionEvents.Add(message);

            ApplyEffectToTarget(target, ability.Effect, value, ability.Id);

            if (ability.BonusEffect != null && 
                ability.BonusValue != null &&
                ability.BonusConditionals != null)
            {
                if (ConditionalsMet(card.PlayerId, ability.BonusConditionals))
                {
                    message = new()
                    {
                        CardActionSource = card,
                        PlayerId = card.PlayerId,
                        Effect = ability.Effect,
                        TargetsCardIds = targets.Select(c => c.Id).ToList(),
                        Value = value
                    };

                    CurrentRoundsEvents.Last().CardActionEvents.Add(message);

                    int bonusValue = GetValue(card.PlayerId, ability.BonusValue);
                    ApplyEffectToTarget(target, (Effect)ability.BonusEffect, bonusValue, ability.Id);
                }
            }
        }
    }

    private void ApplyEffectToTarget(Card target, Effect effect, int value, int abilityId)
    {
        if (target is CreatureCard targetCreature)
        {
            switch (effect)
            {
                case AttackBuffRoundEffect:
                    {
                        targetCreature.AttackBuffRound += value;
                        break;
                    }
                case AttackBuffEffect:
                    {
                        targetCreature.AttackModifiers[abilityId] = value;

                        //remove the card action event we've added
                        CurrentRoundsEvents.Last().CardActionEvents.Remove(CurrentRoundsEvents.Last().CardActionEvents.Last());
                        break;
                    }
                case AttackDebuffEffect:
                    {
                        targetCreature.AttackModifiers[abilityId] = -value;

                        //remove the card action event we've added
                        CurrentRoundsEvents.Last().CardActionEvents.Remove(CurrentRoundsEvents.Last().CardActionEvents.Last());
                        break;
                    }
                case DirectDamageEffect:
                    {
                        bool hasDied = targetCreature.DamageCreature(value);
                        if (hasDied)
                        {
                            EventMessage(new CreatureDeathEvent(targetCreature.Id, targetCreature.Name!));
                            CheckPassiveAbilities();
                        }
                        break;
                    }
                case HealEffect:
                    {
                        targetCreature.HealCreature(value);
                        break;
                    }
            }
        }
        else if (target is PlayerCard playerCard)
        {
            switch (effect)
            {
                //TODO: maybe wrap these in DamagePlayer and HealPlayer methods
                case DirectDamageEffect:
                    {
                        Players[playerCard.Id].HealthPoints -= value;
                        break;
                    }
                case HealEffect:
                    {
                        Players[playerCard.Id].HealthPoints += value;
                        break;
                    }
                case DrawCardEffect:
                    {
                        var cardsDrawn = Players[playerCard.Id].DrawCard(value);
                        CurrentRoundsEvents.Add(new CardDrawEvent(cardsDrawn){ PlayerId = playerCard.Id });
                        break;
                    }
            }
        }
    }

    private int GetValue(int playerId, Value value)
    {
        if (value is FixedValue fv)
        {
            return fv.Value;
        }
        else if (value is CountValue cv)
        {
            return cv.Get(playerId, this);
        }
        else
        {
            throw new Exception("Can't find Value Type");
        }
    }

    private void ProcessCasts()
    {
        foreach (var player in Players)
        {
            if (player.SubmittedTurn!.TurnType != TurnType.Cast)
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
                        CheckPassiveAbilities();

                        creature.CurrentHealth = creature.Health;
                        //creature.HasActed = true;
                        //creature.SummonedThisTurn = true;

                        ProcessCardTriggers(player.Id, creature, new CastTrigger());
                    }
                    else
                    {
                        Console.WriteLine($"[ERROR]: Not enough room on {creature.PlayerId} board for {creature.Id}");
                    }
                }
                else if (player.CardToPlay is SpellCard spell)
                {
                    EventMessage(new CastEvent(player.Id ,spell));
                    ProcessCardTriggers(player.Id, spell, new CastTrigger());
                }
            }
        }
    }
    
    private void ProcessAttacks()
    {
        foreach (var player in Players)
        {
            if (player.SubmittedTurn!.TurnType != TurnType.Attack)
                continue;

            if (player.CardToPlay is CreatureCard attackingCreature && attackingCreature.IsAlive())
            {
                Player otherPlayer = GetOpponent(player.Id);

                var attackEvent = new AttackEvent(player.Id, attackingCreature.Id, attackingCreature.Name!);
                EventMessage(attackEvent);

                ProcessCardTriggers(player.Id, player.CardToPlay, 
                    new SelfTrigger() { SelfTriggerType = SelfTriggerType.Attack });

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
                        CheckPassiveAbilities();
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
                        CheckPassiveAbilities();
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
            if (player.SubmittedTurn!.TurnType != TurnType.MoveToBlockZone)
                continue;

            if (player.CardToPlay is CreatureCard newBlocker && newBlocker.IsAlive())
            {
                if (player.BlockingCreature == null)
                {
                    player.BlockingCreature = newBlocker;
                    BoardState.SummonedCreatures[player.Id].Remove(newBlocker);

                    EventMessage(new MoveToBlockZoneEvent(player.Id, newBlocker.Id, newBlocker.Name!));

                    ProcessCardTriggers(player.Id, player.CardToPlay, 
                        new SelfTrigger() { SelfTriggerType = SelfTriggerType.MoveToBlockZone });
                }
                else
                {
                    int index = BoardState.SummonedCreatures[player.Id].IndexOf(newBlocker);
                    BoardState.SummonedCreatures[player.Id][index] = player.BlockingCreature;

                    var oldBlocker = player.BlockingCreature;
                    player.BlockingCreature = newBlocker;
                    BoardState.SummonedCreatures[player.Id].Remove(newBlocker);

                    EventMessage(new BlockSwapEvent(player.Id, newBlocker.Id, newBlocker.Name!, oldBlocker.Id, oldBlocker.Name!));

                    ProcessCardTriggers(player.Id, player.CardToPlay, 
                        new SelfTrigger() { SelfTriggerType = SelfTriggerType.BlockSwap });
                }
            }
        }
    }

    //abilities
    private void ProcessCardTriggers(int playerId, Card card, Trigger triggerToProcess)
    {
        var trigger = card.Triggers.SingleOrDefault(a => a.SameAs(triggerToProcess));

        if (trigger == null) 
        {
            Console.WriteLine($"Couldn't find trigger {triggerToProcess.ToString()} on the card {card.Name}");
            return;
        }

        foreach (var ability in GetAbilities(trigger))
        {
            if (ConditionalsMet(playerId, ability.Conditionals))
            {
                ProcessAbility(card, ability);
            }
        }
    }

    private List<Card> GetTargets(Card cardToPlay, Ability ability)
    {
        List<Card> targetsResult = new();

        if (ability.Target is SelfTarget)
        {
            targetsResult.Add(cardToPlay);
        }
        else if (ability.Target is PlayerTarget playerTarget)
        {
            int cardId = playerTarget.GetTargetId(cardToPlay);
            targetsResult.Add(GetCardFromId(cardId));
        }
        else if (ability.Target is OpponentTarget opponentTarget)
        {
            int cardId = opponentTarget.GetTargetId(cardToPlay);
            targetsResult.Add(GetCardFromId(cardId));
        }
        else if (ability.Target is SelectionTarget selectTarget)
        {
            if (Players[cardToPlay.PlayerId].TargetSelections != null)
            {
                var targetSelection = Players[cardToPlay.PlayerId].TargetSelections.SingleOrDefault(ts => ts.CardId == cardToPlay.Id && ts.AbilityId == ability.Id);

                if (targetSelection != null)
                {
                    foreach (var target in targetSelection.SelectedTargets)
                    {
                        var card = GetCardFromId(target);

                        if (card != null)
                        {
                            targetsResult.Add(card);
                        }
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

    private bool ConditionalsMet(int playerId, List<Conditional> conditionals)
    {
        if (conditionals.Count <= 0)
            return true;

        List<bool> passes = new();
        foreach (var condition in conditionals)
        {
            switch (condition)
            {
                case Conditional.Round1: passes.Add(RoundCount == 1); break;
                case Conditional.Round2: passes.Add(RoundCount == 2); break;
                case Conditional.Round3: passes.Add(RoundCount == 3); break;
                case Conditional.Round4: passes.Add(RoundCount == 4); break;
                case Conditional.Round5: passes.Add(RoundCount == 5); break;
                case Conditional.PlayerCreatureDiedThisTurn: passes.Add(CurrentTurnsEvents.Where(x => x is CreatureDeathEvent && x.PlayerId == playerId).Count() > 0); break;
                case Conditional.OpponentCreatureDiedThisTurn: passes.Add(CurrentTurnsEvents.Where(x => x is CreatureDeathEvent && x.PlayerId != playerId).Count() > 0); break;
                case Conditional.AnyCreatureDiedThisTurn: passes.Add(CurrentTurnsEvents.Where(x => x is CreatureDeathEvent).Count() > 0); break;
                default:
                    passes.Add(true); 
                    Console.WriteLine($"The condition {condition.ToString()} has not yet been implemented");
                break;
            }
        }

        return !passes.Contains(false); 
    }


    private void EventMessage(EventMessage message)
    {
        Console.WriteLine(message.GetString());
        CurrentRoundsEvents.Add(message);
    }
}
