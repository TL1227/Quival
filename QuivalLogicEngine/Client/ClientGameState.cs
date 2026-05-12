using QuivalLogicEngine.States;

namespace QuivalLogicEngine.Client
{
    public class ClientGameState
    {
        public PlayerState PlayerState { get; set; }
        public BoardState BoardState { get; set; }
        public int OpponentId { get; set; }
        public int OpponentCardCount { get; set; }
        public int OpponentHealthPoints { get; set; }
        public List<ICardIntent> CardIntents { get; set; }
    }
}
