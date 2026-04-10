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

    public Match()
    {
        Players = new Player[2];
        CurrentPhase = Phases.Summon; //TODO: change this after testing combat
    }

    public void SetSpellStream(List<Card> cards, int playerId)
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
        List<ICardIntent> intents = new();

        for (int i = 0; i < (int)SpellSlot.MAX; i++)
        {
            for (int p = 0; p < 2; p++)
            {
                Card? card = Players[p].SpellStream.GetCard(i);

                if (card != null)
                    intents.AddRange(Cast(card));
            }
        }
    }

    private List<ICardIntent> Cast(Card card)
    {
        List<ICardIntent> intents = new();

        if (card.Type == CardType.Attack)
        {
            intents.Add(new Attack(0, "Hello"));
        }
        if (card.Type == CardType.Creature)
        {
            intents.Add(new Summon(4));
        }

        return intents;
    }
}
