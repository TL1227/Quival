using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuivalLogicEngine.Cards
{
    public abstract class Value
    {
    }

    public class FixedValue : Value
    {
        public int Value { get; set; }
    }

    public enum CountValueSource
    {
        CreaturesOnTheBoard,
        CardsInHand,
    }

    public class CountValue : Value
    {
        private CountValueSource CountSource { get; set; }
        private Side Side { get; set; }

        public int Get(int playerId, Match match)
        {
            switch (CountSource)
            {
                case CountValueSource.CreaturesOnTheBoard:
                    {
                        if (Side == Side.Player)
                            return match.BoardState.GetAllSummonedCreaturesByPlayerId(playerId).Count();
                        else if (Side == Side.Opponent)
                            return match.BoardState.GetAllSummonedCreaturesByPlayerId(match.GetOpponent(playerId).Id).Count();
                        else
                            return match.GetAllCreatures().Count;
                    }
                case CountValueSource.CardsInHand:
                    {
                        int playerHand = match.Players[playerId].Hand.Count;
                        int opponentHand = match.Players[match.GetOpponent(playerId).Id].Hand.Count;

                        if (Side == Side.Player)
                            return playerHand;
                        else if (Side == Side.Opponent)
                            return opponentHand;
                        else
                            return playerHand + opponentHand;
                    }
                default:
                    throw new NotImplementedException($"Haven't Implemented {CountSource}");
            }
        }
    }
}
