//using QuivalLogicEngine;
using System.Text.Json.Serialization;
//using static System.Net.Mime.MediaTypeNames;

namespace QuivalLogicEngine.Cards;

public class Set
{
    public string Name { get; set; }
    public string SetCode { get; set; }
    public List<Card> Cards {  get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(CreatureCard), "creaturecard")]
[JsonDerivedType(typeof(AttackCard), "attackcard")]
[JsonDerivedType(typeof(PlayerCard), "playercard")]
[JsonDerivedType(typeof(SpellCard), "spellcard")]
public abstract class Card
{
    public string? SetCode { get; set; }
    public int UniqueId { get; set; }
    public int Id { get; set; }
    public int PlayerId { get; set; } = -1;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Cost { get; set; }
    public List<Trigger> Triggers { get; set; } = new();
    public List<Ability> PassiveAbilities { get; set; } = new();
}

public class PlayerCard : Card
{
}

public class CreatureCard : Card
{
    public int Attack { get; set; }
    public int Health { get; set; }
    public int CurrentHealth { get; set; }
    public bool HasActed { get; set; }
    public bool SummonedThisTurn { get; set; }
    public int AttackBuffRound { get; set; }
    public Dictionary<int, int> AttackModifiers { get; set; } = new();

    public CreatureCard() { }

    public CreatureCard(CreatureCard other) 
    {
        Id = 0;
        Name = other.Name;
        Description = other.Description;
        Cost = other.Cost;
        Attack = other.Attack;
        Health = other.Health;
        HasActed = true;
        CurrentHealth = Health;
    }

    public CreatureCard(int attack, int health, int cost)
    {
        Id = 0;
        Name = "";
        Description = "";
        Cost = cost;
        Attack = attack;
        Health = health;
        HasActed = true;
        CurrentHealth = Health;
    }

    public int GetAttackDamage()
    {
        int attack = 0;

        foreach (var modifiers in AttackModifiers)
        {
            attack += modifiers.Value;
        }

        attack += Attack + AttackBuffRound;

        //NOTE: we might want minus attack damage at some point
        return attack < 0 ? 0 : attack;
    }

    public bool DamageCreature(int dmg)
    {
        CurrentHealth -= dmg;
        return CurrentHealth <= 0; 
    }
    public void HealCreature(int points)
    {
        CurrentHealth += points;
    }

    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }

    public bool IsAlive()
    {
        return CurrentHealth > 0;
    }
}

public class AttackCard : Card
{
    public AttackCard(int playerId, int cardId)
    {
        Id = cardId;
        Name = "Attack!";
        Description = "Attack the opponent.";
        PlayerId = playerId;
    }
}

public class SpellCard : Card
{
    //TODO: considering this is literally nothing, maybe we should rethink this? 
}
