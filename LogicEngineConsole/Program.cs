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

            Console.WriteLine("Sending player decks");

            List <ICard> TheDeck =
            [
                new CreatureCard(0, 1, 1),
                new CreatureCard(0, 1, 1),
                new CreatureCard(0, 2, 2),
                new CreatureCard(0, 2, 3),
                new CreatureCard(0, 3, 1),
                new CreatureCard(0, 2, 4),
                new CreatureCard(0, 4, 2),
                new CreatureCard(0, 2, 4),
                new CreatureCard(0, 2, 4),
                new CreatureCard(0, 2, 4)
            ];

            List <ICard> TheOtherDeck =
            [
                new CreatureCard(0, 1, 1),
                new CreatureCard(0, 1, 1),
                new CreatureCard(0, 2, 2),
                new CreatureCard(0, 2, 3),
                new CreatureCard(0, 3, 1),
                new CreatureCard(0, 2, 4),
                new CreatureCard(0, 4, 2),
                new CreatureCard(0, 2, 4),
                new CreatureCard(0, 2, 4),
                new CreatureCard(0, 2, 4)
            ];

            match.SetPlayer(PLAYER_1, TheDeck);
            match.SetPlayer(PLAYER_2, TheOtherDeck);

            List<List<ICard>> PlayerHands = new() { new (), new() };

            Console.WriteLine("Fetching player's opening hands");

            for (int i = 0; i < 2; i++)
                PlayerHands[i] = match.GetPlayerHand(i);

            Console.WriteLine($"Player 1 plays {PlayerHands[PLAYER_1][0].Id} card");
            match.SetCardToPlay(PLAYER_1, PlayerHands[PLAYER_1][0].Id);

            Console.WriteLine($"Player 2 plays {PlayerHands[PLAYER_2][2].Id} card");
            match.SetCardToPlay(PLAYER_2, PlayerHands[PLAYER_2][2].Id);

            if (match.BothCardsToPlayAreSet())
                match.ProcessCards();
        }
    }
}
