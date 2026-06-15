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
[JsonDerivedType(typeof(BlankCard), "blankcard")]
[JsonDerivedType(typeof(SpellCard), "spellcard")]
public abstract class Card
{
    public string? SetCode { get; set; }
    public int UniqueId { get; set; }
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Cost { get; set; }
    public List<Trigger> Abilities { get; set; } = new();

    public virtual List<ICardIntent> GetIntents()
    {
        return new List<ICardIntent>();
    }
}

public class BlankCard : Card
{
    public BlankCard(int playerId)
    {
        PlayerId = playerId;
    }

    public override List<ICardIntent> GetIntents()
    {
        return new List<ICardIntent>();
    }
}

public class CreatureCard : Card
{
    public int Attack { get; set; }
    public int Health { get; set; }
    public int CurrentHealth { get; set; }
    public bool HasActed { get; set; }
    public int AttackBuff { get; set; }

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
        return Attack + AttackBuff;
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

    public override List<ICardIntent> GetIntents()
    {
        List<ICardIntent> intents = [ new Summon(Id) ];
        return intents;
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

    public override List<ICardIntent> GetIntents()
    {
        //TODO: this should look up the Id of the attacking creature and get any "if attacking" 
        //abilities. So like DoubleUp
        //Maybe a function called List<ICardIntent> GetAttackingIntents(int CardId);

        List<ICardIntent> intents = [ new Attack(PlayerId, Id) ];
        return intents;
    }
}

public class SpellCard : Card
{
    public override List<ICardIntent> GetIntents()
    {
        return new List<ICardIntent>();
    }
}
