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

    public void GetMessage(Message message)
    {

    }

    public Message SendMessage()
    {
        return new Message();
    }
}
