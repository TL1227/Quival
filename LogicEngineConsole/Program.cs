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
                    Description = "If Desmond Future Knight attacks on round 2, heal any target by 2",
                    Cost = 2,
                    Attack = 2,
                    Health = 2,
                    Abilities = [
                        new Trigger()
                        {
                            TriggerType = TriggerType.Attack,
                            Abilities =
                            {
                                new Ability()
                                {
                                    Effect = Effect.Heal,
                                    TargetType = TargetType.Damageable,
                                    NumberOfTargets = 1,
                                    CanTargetSelf = false,
                                    Value = 2,
                                    Conditionals =
                                    {
                                        Conditional.Round2
                                    }
                                }
                            },
                        }
                    ]
                },
                new CreatureCard(){
                    Name = "Token",
                    Cost = 1,
                    Attack = 1,
                    Health = 1,
                },
                new CreatureCard(){
                    Name = "Defender",
                    Description = "",
                    Cost = 2,
                    Attack = 1,
                    Health = 3,
                },
                new CreatureCard(){
                    Name = "Aggression",
                    Description = "",
                    Cost = 2,
                    Attack = 3,
                    Health = 1,
                },
                new CreatureCard(){
                    Name = "BFG",
                    Description = "If BFG attacks on round 3, BFG deals 4 damage",
                    Cost = 3,
                    Attack = 2,
                    Health = 4,
                    Abilities =
                    {
                        new Trigger()
                        {
                            TriggerType = TriggerType.Attack,
                            Abilities =
                            {
                                new Ability()
                                {
                                    Effect = Effect.AttackBuff,
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
                    Description = "Deal 2 damage to any target",
                    Cost = 1,
                    Abilities =
                    {
                        new Trigger()
                        {
                            TriggerType = TriggerType.Cast,
                            Abilities =
                            {
                                new Ability()
                                {
                                    Effect = Effect.DirectDamage,
                                    TargetType = TargetType.Damageable,
                                    NumberOfTargets = 1,
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

            JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };
            options.Converters.Add(new JsonStringEnumConverter());

            string json = JsonSerializer.Serialize(set, options);
            Console.WriteLine(json);
            File.WriteAllText("..\\..\\..\\..\\QuivalCards.json", json);
        }
    }
}
