using QuivalLogicEngine.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine;

public enum SpellSlot
{
    Quick,
    One,
    Two,
    Three,
    Four,
    Five,
    Slow,
    MAX
}

    public class SpellStream
    {
    private ICard?[] Stream { get; set; }

        public SpellStream()
        {
        Stream = new ICard?[(int)SpellSlot.MAX];
    }

    public void Set(List<ICard> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Stream[i] = cards[i];
        }
    }

    public void ClearStream()
    {
        for (int i = 0; i < Stream.Length; i++)
        { 
            Stream[i] = null;
        }
    }

    public void AddCard(int index, ICard card)
    {
        Stream[index] = card;
    }

    public ICard? GetCard(int slot)
    {
        return Stream[slot];
        }

    public bool ContainsCards()
    {
        foreach (var card in Stream)
        {
            if (card != null)
                return true;
        }

        return false;
    }
}
