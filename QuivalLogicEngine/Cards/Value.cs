using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Cards
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "valuetype")]
    [JsonDerivedType(typeof(FixedValue), "fixed")]
    [JsonDerivedType(typeof(CountValue), "count")]
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
        public CountValueSource CountSource { get; set; }
        public Side Side { get; set; } //NOTE: This isn't needed for Cards in hand

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
