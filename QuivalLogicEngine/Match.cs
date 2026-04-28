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

    public void SetSpellStream(List<ICard> cards, int playerId)
    {
        Players[playerId].SpellStream.Set(cards);
    }

    public bool BothStreamsAreSet()
    {
        foreach (var player in Players)
        {
            if (player.SpellStreamSet() == false)
                return false;
        }

        return true;
    }

    public void ProcessSpellStreams()
    {
        Console.WriteLine($"[EVENT]: TURN {TurnCount}");

        for (int i = 0; i < (int)SpellSlot.MAX; i++)
        {
            Console.WriteLine($"[EVENT]: SPELLSLOT {i}");

            for (int p = 0; p < 2; p++)
            {
                ICard? card = Players[p].SpellStream.GetCard(i);

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

            CardIntents[0].Clear();
            CardIntents[1].Clear();
        }

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
