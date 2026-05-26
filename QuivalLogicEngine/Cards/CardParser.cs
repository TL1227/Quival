using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuivalLogicEngine.Cards
{
    public static class CardParser
    {
        public static void ParseSomeCards(string jsonPath)
        {
            /*
            List<Card> cards = [];
            string thestring = File.ReadAllText(jsonPath);
            JsonDocument json = JsonDocument.Parse(thestring);

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());

            SpellCard sp = new();
            string somejson = JsonSerializer.Serialize(sp);
            
            foreach (var jsonCard in json.RootElement.GetProperty("Cards").EnumerateArray())
            {
                Console.WriteLine(jsonCard.ToString());
                var card = JsonSerializer.Deserialize<Card>(jsonCard, options);

                if (card != null)
                {
                    cards.Add(card);
                }
            }
            */
        }
    }
}
