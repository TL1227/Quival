using System;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
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
    private Card?[] Stream { get; set; }

    public SpellStream()
    {
        Stream = new Card?[(int)SpellSlot.MAX];
    }

    public void Set(List<Card> cards)
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

    public void AddCard(int index, Card card)
    {
        Stream[index] = card;
    }

    public Card? GetCard(int slot)
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
=======
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine
{
    public class SpellStream
    {
        private Card? QuickSlotCard { get; set; }
        private Queue<Card> Stream { get; set; }
        private Card? SlowSlotCard { get; set; }

        public SpellStream()
        {
            Stream = new();
        }

        public int GetStreamCount()
        {
            return Stream.Count();
        }

>>>>>>> a0127bc342dc66b9a4cdda07ba28fa54093a61be
    }
}
