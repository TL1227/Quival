using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    public void SetSpellStream(Queue<Card> spellStream, int playerId)
    {
        Players[playerId].SetSpellStream(spellStream);
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

    }
}
