using QuivalLogicEngine.States;
using QuivalLogicEngine.Cards;

namespace QuivalLogicEngine.Client
{
    public class ClientGameState
    {
        public PlayerState PlayerState { get; set; }
        public BoardState BoardState { get; set; }
        //TODO: make opponentstate class
        public int OpponentId { get; set; }
        public int OpponentCardCount { get; set; }
        public int OpponentHealthPoints { get; set; }
        public int OpponentManaPoints { get; set; }
        public Card? OpponentBlockCard { get; set; }
        public List<ICardIntent> CardIntents { get; set; }
    }
}
