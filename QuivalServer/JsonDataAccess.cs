using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var json = File.ReadAllText("QuivalCards.json");
            if (json == null)
            {
                Console.WriteLine("Can't find QuivalCards.json!");
                return;
            }

            JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };
            options.Converters.Add(new JsonStringEnumConverter());
            CurrentSet = JsonSerializer.Deserialize<Set>(json, options)!;
        }

        public Card? GetCard(string uniqueId)
        {
            var card = CurrentSet.Cards.SingleOrDefault(c => c.SetCode + c.UniqueId == uniqueId);
            if (card != null)
            {
                //NOTE this should be a copy constructor really but it works for now 
                string? json = JsonSerializer.Serialize(card);
                return JsonSerializer.Deserialize<Card>(json);
            }

            return null;
        }
    }
}
