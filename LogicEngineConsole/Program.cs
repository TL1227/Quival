using QuivalLogicEngine; 
using QuivalLogicEngine.Cards;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LogicEngineConsole
{
    internal class Program
    {
        /*
         * CARD IDEAS
         * [Cowardly Knight]   - 4/3 Gets -1 attack for every enemy creature
         * [Rallying Knight]   - 0/3 Gets +1 attack for every ally creature
         * [Battering Ram]     - 0/1 Attacks for 5 if attacking a blocking creature
         * [Perfect Balance]   - SPL You sac a creature and your opponent has to 
         * [Backup Soldier]    - 1/2 When blockwapping with a creature, heal 2 to that creature
         * [Bubble Shield]     - SPL Add one shield to the blockzone. A shield absorbs 1 hit of damage
         * [Overdrive Ork]     - 1/3 When you attack you can discard cards. For every card add 2 attack
         * [Graveyard Shift]   - SPL Return a creature from the gravyard with a mana cost equal to or less than the current round number (e.g on round 2 you could return a creature with a cost of 1 or 2 but not 3+)
         * [Draw the Curtains] - SPL Draw as many cards as the current Round minus 1. (So on round 1 it does nothing, round 2 draws 1 card etc)
         * [Necro Wizard]      - 2/3 As long as Necro Wizard is on the battlefield. All heal spells deal damage instead.
         */
        
        static void Main(string[] args)
        {
            List<CardDefinition> cards =
            [
                new (){
                    CardType = CardType.Creature,
                    Name = "Desmond Future Knight",
                    Description = "If Desmond Future Knight attacks on round 2, heal any other target by 2",
                    FlavourText = "One of these days Samson, I'll be a kight. You'll see! - Desmond",
                    Cost = 2,
                    Attack = 2,
                    Health = 2,
                    Triggers = [
                        new Trigger()
                        {
                            TriggerType = TriggerType.Attack,
                            Abilities =
                            {
                                new Ability()
                                {
                                    Effect = Effect.Heal,
                                    Target = new SelectionTarget()
                                    {
                                        TargetsPool = TargetPool.Damagable,
                                        Side = Side.Any,
                                        CanTargetSelf = false,
                                        NumberToPick = 1,
                                    },
                                    Value = new FixedValue(){ Value = 2 },
                                    Conditionals =
                                    {
                                        Conditional.Round2
                                    }
                                }
                            },
                        }
                    ]
                },
                new (){
                    CardType = CardType.Creature,
                    Name = "Token",
                    Cost = 1,
                    Attack = 1,
                    Health = 1,
                },
                new (){
                    CardType = CardType.Creature,
                    Name = "Defender",
                    Description = "",
                    Cost = 2,
                    Attack = 1,
                    Health = 3,
                },
                new (){
                    CardType = CardType.Creature,
                    Name = "Aggression",
                    Description = "",
                    Cost = 2,
                    Attack = 3,
                    Health = 1,
                },
                new (){
                    CardType = CardType.Creature,
                    Name = "BFG",
                    Description = "If BFG attacks on round 3, BFG deals 4 damage",
                    Cost = 3,
                    Attack = 2,
                    Health = 4,
                    Triggers =
                    {
                        new Trigger()
                        {
                            TriggerType = TriggerType.Attack,
                            Abilities =
                            {
                                new Ability()
                                {
                                    Effect = Effect.AttackBuffRound,
                                    Target = new SelfTarget(),
                                    Value = new FixedValue(){ Value = 2 },
                                    Conditionals = { Conditional.Round3 }
                                }
                            },
                        }
                    }
                },
                new (){
                    CardType = CardType.Creature,
                    Name = "The Wall",
                    Description = "",
                    Cost = 4,
                    Attack = 1,
                    Health = 5,
                },
                new ()
                {
                    CardType = CardType.Spell,
                    Name = "Zap",
                    Description = "Deal 2 damage to any target",
                    Cost = 1,
                    Triggers =
                    {
                        new Trigger()
                        {
                            TriggerType = TriggerType.Cast,
                            Abilities =
                            {
                                new Ability()
                                {
                                    Effect = Effect.DirectDamage,
                                    Target = new SelectionTarget()
                                    {
                                        TargetsPool = TargetPool.Damagable,
                                        Side = Side.Any,
                                        CanTargetSelf = true,
                                        NumberToPick = 1
                                    },
                                    Value = new FixedValue(){ Value = 2 },
                                }
                            }
                        }
                    }
                },
                new ()
                {
                    CardType = CardType.Spell,
                    Name = "Spring Water",
                    Description = "Heal 2 health to any target",
                    Cost = 2,
                    Triggers =
                    {
                        new Trigger()
                        {
                            TriggerType = TriggerType.Cast,
                            Abilities =
                            {
                                new Ability()
                                {
                                    Effect = Effect.Heal,
                                    Target = new SelectionTarget()
                                    {
                                        TargetsPool = TargetPool.Damagable,
                                        Side = Side.Any,
                                        CanTargetSelf = true,
                                        NumberToPick = 1
                                    },
                                    Value = new FixedValue(){ Value = 2 },
                                }
                            }
                        }
                    }
                },
                new ()
                {
                    CardType = CardType.Spell,
                    Name = "Mega Flare",
                    Description = "Deal 1 damage to any target. If cast on Round 4, deal 3 more damage",
                    Cost = 4,
                    Triggers =
                    {
                        new Trigger()
                        {
                            TriggerType = TriggerType.Cast,
                            Abilities =
                            {
                                new Ability()
                                {
                                    Effect = Effect.DirectDamage,
                                    Target = new SelectionTarget()
                                    {
                                        TargetsPool = TargetPool.Damagable,
                                        Side = Side.Any,
                                        NumberToPick = 1
                                    },
                                    Value = new FixedValue(){ Value = 1 },

                                    BonusEffect = Effect.DirectDamage,
                                    BonusValue = new FixedValue() { Value = 3 },
                                    BonusConditionals = [ Conditional.Round4 ]
                                },
                            },
                        }
                    }
                },
                new (){
                    CardType = CardType.Creature,
                    Name = "Ping Boy",
                    Description = "When Ping Boy enters the battlefield, opponent takes 1 damage",
                    Cost = 2,
                    Attack = 1,
                    Health = 1,
                    Triggers =
                    [
                        new Trigger()
                        {
                            TriggerType = TriggerType.Cast,
                            Abilities =
                            [
                                new Ability()
                                {
                                    Effect = Effect.DirectDamage,
                                    Target = new DirectTarget()
                                    {
                                        AutoTargetType = DirectTargetType.Opponent,
                                    },
                                    Value = new FixedValue(){ Value = 1 }
                                }
                            ]
                        }
                    ]
                },
                new (){
                    CardType = CardType.Creature,
                    Name = "Rally Knight",
                    Description = "Rally Knight gets +1 attack for every creature you control",
                    Cost = 3,
                    Attack = 0,
                    Health = 3,
                    PassiveAbilities =
                    [
                        new Ability()
                        {
                            Effect = Effect.AttackBuff,
                            Target = new SelfTarget(),
                            Value = new CountValue()
                            {
                                CountSource = CountValueSource.CreaturesOnTheBoard,
                                Side = Side.Player
                            }
                        }
                    ]
                },
                new (){
                    CardType = CardType.Creature,
                    Name = "Cowardly Knight",
                    Description = "Cowardly Knight gets -1 attack for every creature your opponent controls",
                    Cost = 3,
                    Attack = 4,
                    Health = 3,
                    PassiveAbilities =
                    [
                        new Ability()
                        {
                            Effect = Effect.AttackDebuff,
                            Target = new SelfTarget(),
                            Value = new CountValue()
                            {
                                CountSource = CountValueSource.CreaturesOnTheBoard,
                                Side = Side.Opponent
                            }
                        }
                    ]
                },
                new (){
                    CardType = CardType.Creature,
                    Name = "Curious Dealer",
                    Description = "Curious Dealer Gets +1 attack for every card in your hand",
                    Cost = 5,
                    Attack = 0,
                    Health = 4,
                    PassiveAbilities =
                    [
                        new Ability()
                        {
                            Effect = Effect.AttackBuff,
                            Target = new SelfTarget(),
                            Value = new CountValue()
                            {
                                CountSource = CountValueSource.CardsInHand,
                                Side = Side.Player
                            }
                        }
                    ]
                },
            ];

            Set set = new Set()
            {
                Version = new Version(0,2,0),
                Name = "Alpha",
                SetCode = "ALP",
                Cards = cards
            };

            int IdCounter = 1;
            foreach (var card in cards)
            {
                card.UniqueId = $"{set.SetCode}{IdCounter++}";

                //assign ability Ids
                int abilityId = 0;
                foreach (var trigger in card.Triggers)
                    foreach (var ability in trigger.Abilities)
                        ability.Id = abilityId++;

                if (card.CardType == CardType.Creature)
                    Console.WriteLine($"{card.UniqueId} Attack {card.Attack} Health {card.Health} Cost {card.Cost}");
            }

            JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };
            options.Converters.Add(new JsonStringEnumConverter());

            string json = JsonSerializer.Serialize(set, options);
            File.WriteAllText("..\\..\\..\\..\\QuivalServer\\QuivalCards.json", json);
        }
    }
}
