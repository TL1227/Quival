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
        };

        Players.Add(player);
    }

    public void SubmitTurn(int playerId, QuivalTurn turn)
    {
        if (turn.TurnType == TurnType.EndTurn)
        {
            Players[playerId].SubmittedTurn = turn;
        }
        else if (turn.TurnType == TurnType.Cast)
        {
            var card = GetCardFromId(turn.CardToPlayId);

            if (card != null)
            {
                if (card.Cost <= Players[playerId].Mana)
                {
                    Players[playerId].SubmittedTurn = turn;
                    Players[playerId].Hand.Remove(card);
                }
                else
                {
                    Console.WriteLine($"player {playerId} doesn't have enough mana for {turn.CardToPlayId}");
                    Console.WriteLine($"they have {Players[playerId].Mana} and need {card.Cost}");
                }
            }
            else
            {
                Console.WriteLine($"Can't find player {playerId}'s card {turn.CardToPlayId} in current match");
            }
        }
        else
        {
            var card = GetCardFromId(turn.CardToPlayId);
            if (card != null)
            {
                Players[playerId].SubmittedTurn = turn;
            }
            else
            {
                Console.WriteLine($"Can't find player {playerId}'s card {turn.CardToPlayId} in current match");
            }
        }
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
        return Players[0].SubmittedTurn!.TurnType == TurnType.EndTurn &&
                Players[1].SubmittedTurn!.TurnType == TurnType.EndTurn;
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

                        creature.HasActed = true;
                        creature.CurrentHealth = creature.Health;

                        ProcessTriggeredAbilities(player.CardToPlay, Trigger.Cast);
                    }
                    else
                    {
                        Console.WriteLine($"[ERROR]: Not enough room on {creature.PlayerId} board for {creature.Id}");
                    }
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

            Player otherPlayer = GetOpponent(player.Id);

            if (player.CardToPlay is CreatureCard attackingCreature)
            {
                ProcessTriggeredAbilities(player.CardToPlay, Trigger.Attack);

                EventMessage(new AttackEvent(player.Id, attackingCreature.Id, attackingCreature.Name!));

                if (otherPlayer.BlockingCreature == null)
                {
                    otherPlayer.HealthPoints -= attackingCreature.GetAttackDamage();
                }
                else
                {
                    var blockingCreature = otherPlayer.BlockingCreature;

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

                //Possible after damage trigger could go here
            }
        }
    }

    private void ProcessMoveToBlockZone()
    {
        foreach (var player in Players)
        {
            if (player.SubmittedTurn!.TurnType != TurnType.MoveToBlock)
                continue;

            if (player.CardToPlay is CreatureCard newBlocker)
            {
                if (player.BlockingCreature == null)
                {
                    player.BlockingCreature = newBlocker;
                    BoardState.SummonedCreatures[player.Id].Remove(newBlocker);

                    EventMessage(new MoveToBlockZoneEvent(player.Id, newBlocker.Id, newBlocker.Name!));

                    ProcessTriggeredAbilities(player.CardToPlay, Trigger.MoveToBlockZone);
                }
                else
                {
                    int index = BoardState.SummonedCreatures[player.Id].IndexOf(newBlocker);
                    BoardState.SummonedCreatures[player.Id][index] = player.BlockingCreature;

                    var oldBlocker = player.BlockingCreature;
                    player.BlockingCreature = newBlocker;
                    BoardState.SummonedCreatures[player.Id].Remove(newBlocker);

                    EventMessage(new BlockSwapEvent(player.Id, newBlocker.Id, newBlocker.Name!, oldBlocker.Id, oldBlocker.Name!));

                    ProcessTriggeredAbilities(player.CardToPlay, Trigger.BlockSwap);
                }
            }
        }
    }

    //abilities
    private void ProcessTriggeredAbilities(Card card, Trigger trigger)
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

                //TODO: I think the targets List will probably get filled by the user.
                //      We could just get it from the player's submitted turn.
                List<Card> targets = new();
                if (action.TargetType == TargetType.Self)
                {
                    targets.Add(card);
                }

                ProcessAction(targets, action.Value, action.Intent);

                CardActionEvent message = new()
                {
                    Intent = action.Intent,
                    TargetsCardIds = targets.Select(c => c.Id).ToList(),
                    Value = action.Value
                };

                EventMessage(message);
            }
        }
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

    private void ProcessAction(List<Card> targets, int value, Intent intent)
    {
        switch (intent)
        {
            case Intent.AttackBuff: 
            {
                foreach (var target in targets)
                    if (target is CreatureCard creature)
                        creature.AttackBuff += value;
                break;
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
