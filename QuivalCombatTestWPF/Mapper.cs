using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuivalLogicEngine;

namespace QuivalCombatTestWPF
{
    internal static class Mapper
    {
        public static Queue<Card>? Map(Queue<BoardCard> boardCards)
        {
            Queue<Card>? queue = new();

            //TODO: this probably needs to look up cached card data to populate the type and such
            //We'll hardcode for now
            foreach (var bc in boardCards)
            {
                Card card = new(CardType.Creature, bc.Attack, bc.Defence);
                queue.Enqueue(card);
            }

            return queue;
        }

        public static Stack<Card>? Map(Stack<BoardCard> boardCards)
        {
            Stack<Card>? queue = new();

            //TODO: this probably needs to look up cached card data to populate the type and such
            //We'll hardcode for now
            foreach (var bc in boardCards)
            {
                Card card = new(CardType.Creature, bc.Attack, bc.Defence);
                queue.Push(card);
            }

            return queue;
        }

        public static List<Card>? MapToList(Stack<BoardCard> boardCards)
        {
            List<Card>? queue = new();

            //TODO: this probably needs to look up cached card data to populate the type and such
            //We'll hardcode for now
            foreach (var bc in boardCards)
            {
                Card card = new(CardType.Creature, bc.Attack, bc.Defence);
                queue.Add(card);
            }

            return queue;
        }
    }
}
