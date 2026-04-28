//using QuivalLogicEngine;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace QuivalLogicEngine.Cards;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CreatureCard), "creaturecard")]
[JsonDerivedType(typeof(AttackCard), "attackcard")]
public interface ICard
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<ICardIntent> GetIntents();
}

public class BlankCard : ICard
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public List<ICardIntent> GetIntents()
    {
        return new List<ICardIntent>();
    }
}

public class CreatureCard : ICard
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public int Attack { get; set; }
    public int Health { get; set; }

    public CreatureCard(int id, int attack, int health)
    {
        Id = id;
        Name = "My Creature";
        Description = "Some description of the creature";
        Attack = attack;
        Health = health;
    }

    public List<ICardIntent> GetIntents()
    {
        List<ICardIntent> intents = [ new Summon(Id) ];
        return intents;
    }
}

public class AttackCard : ICard
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public AttackCard()
    {
        Id = 0;
        Name = "Attack!";
        Description = "Attack the opponent.";
    }

    public List<ICardIntent> GetIntents()
    {
        //TODO: this should look up the Id of the attacking creature and get any "if attacking" 
        //abilities. So like DoubleUp
        //Maybe a function called List<ICardIntent> GetAttackingIntents(int CardId);

        List<ICardIntent> intents = [ /*new Attack(Id)*/ ];
        return new List<ICardIntent>();
    }
}


//TODO: maybe inheritance would make more sense here, like the below 
public abstract class GameCard
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public abstract List<ICardIntent> GetIntents();
}

public class Attack : GameCard
{
    public Attack()
    {
        Id = 0;
        Name = "Attack!";
        Description = "Attack the opponent.";
    }

    public override List<ICardIntent> GetIntents()
    {
        //TODO: this should look up the Id of the attacking creature and get any "if attacking" 
        //abilities. So like DoubleUp
        //Maybe a function called List<ICardIntent> GetAttackingIntents(int CardId);

        List<ICardIntent> intents = [ /*new Attack(Id)*/ ];
        return new List<ICardIntent>();
    }
}
