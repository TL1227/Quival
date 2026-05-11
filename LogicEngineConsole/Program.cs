using QuivalLogicEngine; using QuivalLogicEngine.Cards;

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

            Console.WriteLine("Sending player decks");

            List <Card> TheDeck =
            [
                new CreatureCard(0, 1, 1, 3),
                new CreatureCard(0, 1, 1, 3),
                new CreatureCard(0, 2, 2, 3),
                new CreatureCard(0, 2, 3, 3),
                new CreatureCard(0, 3, 1, 3),
                new CreatureCard(0, 2, 4, 3),
                new CreatureCard(0, 4, 2, 3),
                new CreatureCard(0, 2, 4, 3),
                new CreatureCard(0, 2, 4, 3),
                new CreatureCard(0, 2, 4, 3)
            ];

            List <Card> TheOtherDeck =
            [
                new CreatureCard(0, 1, 1, 3),
                new CreatureCard(0, 1, 1, 3),
                new CreatureCard(0, 2, 2, 3),
                new CreatureCard(0, 2, 3, 3),
                new CreatureCard(0, 3, 1, 3),
                new CreatureCard(0, 2, 4, 3),
                new CreatureCard(0, 4, 2, 3),
                new CreatureCard(0, 2, 4, 3),
                new CreatureCard(0, 2, 4, 3),
                new CreatureCard(0, 2, 4, 3)
            ];

            match.SetPlayer(PLAYER_1, TheDeck);
            match.SetPlayer(PLAYER_2, TheOtherDeck);

            List<List<Card>> PlayerHands = new() { new (), new() };

            Console.WriteLine("Fetching player's opening hands");

            for (int i = 0; i < 2; i++)
                PlayerHands[i] = match.GetPlayerHand(i);

            int round = 0;
            for (int i = 0; i < 5; i++)
            {
                match.SetCardToPlay(PLAYER_1, PlayerHands[PLAYER_1][i].Id);
                match.SetCardToPlay(PLAYER_2, PlayerHands[PLAYER_2][i].Id);

                if (match.BothCardsToPlayAreSet())
                {
                    round = match.ProcessCards();
                }

                foreach (var action in match.SuccessfulIntents)
                {
                    //Return these actions to the client for animation and such
                }
                match.SuccessfulIntents.Clear();
            }


            match.SetCardToAttack(PLAYER_1, 1);
            match.SetCardToAttack(PLAYER_2, 13);

            if (match.BothCardsToPlayAreSet())
            {
                round = match.ProcessCards();
            }

            foreach (var action in match.SuccessfulIntents)
            {
                //Return these actions to the client for animation and such
            }
            match.SuccessfulIntents.Clear();

            Console.WriteLine("Next Turn...");
        }
    }
}
