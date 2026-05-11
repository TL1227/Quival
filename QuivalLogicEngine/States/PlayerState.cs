using QuivalLogicEngine.Cards;

namespace QuivalLogicEngine.States
{
    public record PlayerState
    {
        public required int Id { get; set; }
        public required int HealthPoints { get; set; }
        public required List<Card> Hand { get; set; }
        public required List<Card> Deck { get; set; }
        public Card? CardToPlay { get; set; }
    }
}
