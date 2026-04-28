using QuivalLogicEngine;
using QuivalLogicEngine.Cards;

namespace LogicEngineConsole
{
    internal class Program
    {
        public static int PLAYER_1 = 0;
        public static int PLAYER_2 = 1;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting match");

            Match match = new();

            match.SetPlayer(PLAYER_1);
            match.SetPlayer(PLAYER_2);

            List<ICard> spellstream1 = new() 
            { 
                new BlankCard(), //simulate the quickslot
                new CreatureCard(1, 1, 2),
                new CreatureCard(2, 1, 2),
                new CreatureCard(3, 1, 2),
                new CreatureCard(4, 1, 2),
                new CreatureCard(5, 1, 2),
                new BlankCard(), //simulate the slowslot
            };

            match.SetSpellStream(spellstream1, PLAYER_1);

            List<ICard> spellstream2 = new()

            { 
                new BlankCard(), //simulate the quickslot
                new CreatureCard(6, 1, 2),
                new CreatureCard(7, 1, 2),
                new CreatureCard(8, 1, 2),
                new CreatureCard(9, 1, 2),
                new CreatureCard(10, 1, 2),
                new BlankCard(), //simulate the slowslot
            };

            match.SetSpellStream(spellstream2, PLAYER_2);

            if (match.BothStreamsAreSet())
            {
                match.ProcessSpellStreams();
            }
        }
    }
}
