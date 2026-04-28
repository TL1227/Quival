using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuivalLogicEngine.Cards;

namespace QuivalCombatTestWPF
{
    internal static class Mapper
    {
        /*
        public static Queue<ICard>? Map(Queue<BoardCard> boardCards)
        {
            Queue<ICard>? queue = new();

            //TODO: this probably needs to look up cached card data to populate the type and such
            //We'll hardcode for now
            foreach (var bc in boardCards)
            {
                ICard card = new(CardType.Creature, bc.Attack, bc.Defence);
                queue.Enqueue(card);
            }

            return queue;
        }

        public static Stack<ICard>? Map(Stack<BoardCard> boardCards)
        {
            Stack<ICard>? queue = new();

            //TODO: this probably needs to look up cached card data to populate the type and such
            //We'll hardcode for now
            foreach (var bc in boardCards)
            {
                ICard card = new(CardType.Creature, bc.Attack, bc.Defence);
                queue.Push(card);
            }

            return queue;
        }


        public static List<ICard>? MapToList(Stack<BoardCard> boardCards)
        {
            List<ICard>? queue = new();

            //TODO: this probably needs to look up cached card data to populate the type and such
            //We'll hardcode for now
            foreach (var bc in boardCards)
            {
                ICard card = new(CardType.Creature, bc.Attack, bc.Defence);
                queue.Add(card);
            }

            return queue;
        }
        */
    }
}
