using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine.Client
{
    public class ClientGameState
    {
        public Player Player { get; set; }
        public BoardState BoardState { get; set; }

        public int OpponentCardCount { get; set; }
        public List<ICardIntent> CardIntents { get; set; }

        public ClientGameState(Player player, BoardState boardState, int opponentCardCount, List<ICardIntent> cardIntents)
        {
            Player = player;
            BoardState = boardState;
            OpponentCardCount = opponentCardCount;
            CardIntents = cardIntents;
        }
    }
}
