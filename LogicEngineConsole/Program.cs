using QuivalLogicEngine; 
using QuivalLogicEngine.Cards;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LogicEngineConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Card> cards =
            [
                new CreatureCard(){
                    Name = "Desmond Future Knight",
                    Description = "\"One of these days Samson, I'll be a knight. Just gotta muck out the stable first!\" - Desmond",
                    Cost = 2,
                    Attack = 2,
                    Health = 2,
                },
                new CreatureCard(){
                    Name = "Token",
                    Cost = 1,
                    Attack = 1,
                    Health = 1,
                },
                new CreatureCard(){
                    Name = "Defender",
                    Description = "\"I'm gonna block your ass!\" - Defender",
                    Cost = 2,
                    Attack = 1,
                    Health = 3,
                },
                new CreatureCard(){
                    Name = "Aggression",
                    Description = "\"GRAAAAAAAAAAAAHHHHH\" - Aggression",
                    Cost = 2,
                    Attack = 3,
                    Health = 1,
                },
                new CreatureCard(){
                    Name = "BFG",
                    Description = "If BFG attacks on round 3, increase attack by 2 for the rest of the round.",
                    Cost = 3,
                    Attack = 2,
                    Health = 4,
                    Abilities =
                    {
                        new Ability()
                        {
                            Trigger = Trigger.Attack,
                            Actions =
                            {
                                new CardAction()
                                {
                                    Intent = Intent.AttackBuff,
                                    TargetType = TargetType.Self,
                                    Value = 2,
                                    Conditionals =
                                    {
                                        Conditional.Round3
                                    }
                                }
                            },
                        }
                    }
                },
                new CreatureCard(){
                    Name = "The Wall",
                    Description = "",
                    Cost = 4,
                    Attack = 1,
                    Health = 5,
                },
                new SpellCard()
                {
                    Name = "Zap",
                    Description = "",
                    Cost = 1,
                    Abilities =
                    {
                        new Ability()
                        {
                            Trigger = Trigger.Cast,
                            Actions =
                            {
                                new CardAction()
                                {
                                    Intent = Intent.DirectDamage,
                                    TargetType = TargetType.Damageable,
                                    Side = Side.Any,
                                    Value = 2,
                                }
                            }
                        }
                    }
                },
            ];

            Set set = new Set()
            {
                Name = "Alpha",
                SetCode = "ALP",
                Cards = cards
            };

            int IdCounter = 1;
            foreach (Card card in cards)
            {
                card.UniqueId = IdCounter++;
                card.SetCode = set.SetCode;
            }

            JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true, };
            string json = JsonSerializer.Serialize(set, options);
            Console.WriteLine(json);
            File.WriteAllText("..\\..\\..\\..\\QuivalCards.json", json);
        }
    }
}
