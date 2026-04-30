using QuivalLogicEngine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace QuivalLogicEngine;

enum Phases
{
    Draw,
    Summon,
    Combat,
    SecondSummon,
    Last
}

public class Match
{
    private Player[] Players { get; set; }
    private Phases CurrentPhase { get; set; }
    private List<ICardIntent>[] CardIntents { get; set; }
    private BoardState BoardState { get; set; }
    private int TurnCount { get; set; }

    private int CardIdTotal = 1;

    public Match()
    {
        Players = new Player[2];
        CurrentPhase = Phases.Summon; //TODO: change this after testing combat
        CardIntents = new List<ICardIntent>[2]{ new(), new() } ;
        BoardState = new();
        TurnCount = 1;
    }

    public void SetPlayer(int id, List<ICard> deck)
    {
        if (id > Players.Length - 1) 
            return;

        Players[id] = new Player(deck);
    }

    public void SetCardToPlay(int playerId, int cardId)
    {
        Players[playerId].CardToPlay = cardId;
    }

    public bool BothCardsToPlayAreSet()
    {
        foreach (var player in Players)
        {
            if (player.CardToPlay == 0)
                return false;
        }

        return true;
    }

    public List<ICard> GetPlayerHand(int id)
    {
        return Players[id].Hand;
    }

    public void SetCardIds()
    {
        foreach (var player in Players)
            if (player.Deck.Count == 0)
                return;

        foreach (var player in Players)
            foreach (var card in player.Deck)
                card.Id = CardIdTotal++;
    }

    //TODO: probably set all the cards to a dictionary at the start of the match
    private ICard? GetCardFromId(int cardId)
    {
        foreach (var player in Players)
            foreach (var card in player.Deck)
                if (card.Id == cardId)
                    return card;

        return null;
    }

    public void ProcessCards(int spellSlot)
    {
        Console.WriteLine($"[EVENT]: Round {spellSlot}");

        for (int p = 0; p < 2; p++)
        {
            if (Players[p].CardToPlay == 0) 
                continue;

            ICard? card = GetCardFromId(Players[p].CardToPlay);

            if (card != null)
            {
                var intents = card.GetIntents();

                foreach (var intent in intents)
                    intent.PlayerId = p;

                CardIntents[p].AddRange(intents);
            }
        }

        List<Summon> Summons = new();
        List<Block> Blocks = new();
        List<Attack> Attacks = new();
        List<DamageMultiply> DamageMultiplies = new();

        foreach (var intent in CardIntents)
        {
            Summons.AddRange(intent.OfType<Summon>().ToList());
            Blocks.AddRange(intent.OfType<Block>().ToList());
            Attacks.AddRange(intent.OfType<Attack>().ToList());
        }

        //handle the intents
        foreach (var summon in Summons)
        {
            if (BoardState.CreatureSlotFree(summon.PlayerId))
            {
                BoardState.SummonCreature(summon.PlayerId, summon.CardId);
            }
        }

        foreach (var cardintent in CardIntents)
            cardintent.Clear();

        foreach (var player in Players)
            player.CardToPlay = 0;

        TurnCount++;
    }

    private void AssignBlocks(ICardIntent[]? blocks)
    {
        if (blocks == null || blocks.Length == 0) return;

        foreach (var block in blocks)
        {
            if (block is Block b)
            {
                //NOTE: currently this might change depending on rules and if we want to be 
                //able to stack blocking creatures and whatnot
                /*
                if (!BoardState.PlayerHasBlockingCreatures(b.PlayerId))
                {
                    BoardState.SetBlocker(b.PlayerId, b.CardId);
                }
                */
            }
        }
    }
}
