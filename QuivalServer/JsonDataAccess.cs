using QuivalLogicEngine.Cards;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuivalServer
{
    internal class JsonDataAccess : IDataAccessor
    {
        public bool UseDebugCards { get; set; } = false;

        public Set CurrentSet { get; set; }

        public JsonDataAccess() 
        {
            string cardsLocation = "..\\Cards\\QuivalCards.json";
            var json = File.ReadAllText(cardsLocation);
            if (json == null)
            {
                Console.WriteLine($"Can't find {cardsLocation}");
                return;
            }

            JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };
            options.Converters.Add(new JsonStringEnumConverter());
            CurrentSet = JsonSerializer.Deserialize<Set>(json, options)!;
        }

        public Card? GetCard(string uniqueId)
        {
            var cd = CurrentSet.Cards.SingleOrDefault(c => c.UniqueId == uniqueId);
            if (cd != null)
            {
                if (cd.CardType == CardType.Creature)
                {
                    CreatureCard creature = new()
                    {
                        UniqueId = cd.UniqueId,
                        Id = -1,
                        PlayerId = -1,
                        Name = cd.Name,
                        Description = cd.Description,
                        Cost = cd.Cost,
                        Triggers = cd.Triggers,
                        PassiveAbilities = cd.PassiveAbilities,

                        Attack = cd.Attack,
                        Health = cd.Health
                    };
                }
                else
                {
                    SpellCard card = new()
                    { 
                        UniqueId = cd.UniqueId,
                        Id = -1,
                        PlayerId = -1,
                        Name = cd.Name,
                        Description = cd.Description,
                        Cost = cd.Cost,
                        Triggers = cd.Triggers,
                        PassiveAbilities = cd.PassiveAbilities,
                    };
                }
            }

            return null;
        }
    }
}
