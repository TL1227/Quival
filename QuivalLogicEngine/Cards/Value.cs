using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Cards
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "valuetype")]
    [JsonDerivedType(typeof(Fixed), "fixed")]
    [JsonDerivedType(typeof(Count), "count")]
    public abstract class Value
    {
        public abstract string Name { get; set; }
        public int Amount { get; set; }
    }

    public class Fixed : Value
    {
        public override string Name { get; set; } = "Fixed";

        public Fixed() { }
        public Fixed(int amount)
        {
            Amount = amount;
        }
    }

    public enum CountValueSource
    {
        CreaturesOnTheBoard,
        CardsInHand,
    }

    public enum CountSide
    {
        Controller,
        Opponent,
        All
    }

    public class Count : Value
    {
        public override string Name { get; set; } = "Count";
        public CountValueSource CountSource { get; set; }
        public CountSide CountSide { get; set; }

        public int Get(int playerId, Match match)
        {
            int count = 0;
            switch (CountSource)
            {
                case CountValueSource.CreaturesOnTheBoard:
                    {
                        if (CountSide == CountSide.Controller)
                            count = match.BoardState.GetAllSummonedCreaturesByPlayerId(playerId).Count();
                        else if (CountSide == CountSide.Opponent)
                            count = match.BoardState.GetAllSummonedCreaturesByPlayerId(match.GetOpponent(playerId).Id).Count();
                        else
                            count = match.GetAllCreatures().Count;

                        break;
                    }
                case CountValueSource.CardsInHand:
                    {
                        int playerHand = match.Players[playerId].Hand.Count;
                        int opponentHand = match.Players[match.GetOpponent(playerId).Id].Hand.Count;

                        if (CountSide == CountSide.Controller)
                            count = playerHand;
                        else if (CountSide == CountSide.Opponent)
                            count = opponentHand;
                        else
                            count = playerHand + opponentHand;

                        break;
                    }
                default:
                    throw new NotImplementedException($"Haven't Implemented {CountSource}");
            }

            return count + Amount;
        }
    }
}
