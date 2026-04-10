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

    public Match()
    {
        Players = new Player[2];
        CurrentPhase = Phases.Summon; //TODO: change this after testing combat
        BoardState = new();
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
        for (int i = 0; i < (int)SpellSlot.MAX; i++)
        {
            for (int p = 0; p < 2; p++)
            {
                ICard? card = Players[p].SpellStream.GetCard(i);

                if (card != null)
                {
                    CardIntents[i].AddRange(card.GetIntents());
                }
            }
        }

        foreach (var slot in CardIntents)
        {
            var blocks = slot.Where(i => i is Block).ToArray();
            AssignBlocks(blocks);

            var attacks = slot.Where(i => i is Attack).ToArray();
        }
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
                if (!BoardState.PlayerHasBlockingCreatures(b.PlayerId))
                {
                    BoardState.SetBlocker(b.PlayerId, b.CardId);
                }
            }
        }
    }
}
