//using QuivalLogicEngine;
using System.Text.Json.Serialization;
//using static System.Net.Mime.MediaTypeNames;

namespace QuivalLogicEngine.Cards;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CreatureCard), "creaturecard")]
[JsonDerivedType(typeof(AttackCard), "attackcard")]
[JsonDerivedType(typeof(BlankCard), "blankcard")]
public abstract class Card
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int Cost { get; set; }
    public abstract List<ICardIntent> GetIntents();
}

public class BlankCard : Card
{
    public int PlayerId { get; set; }

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
    public bool HasActed { get; set; }

    public CreatureCard(int id, int attack, int health, int cost)
    {
        Id = id;
        Name = "";
        Description = "";
        Cost = cost;
        Attack = attack;
        Health = health;
        HasActed = true;
    }

    public override List<ICardIntent> GetIntents()
    {
        List<ICardIntent> intents = [ new Summon(Id) ];
        return intents;
    }
}

public class AttackCard : Card
{
    public int PlayerId { get; set; }

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

public class BlockCard : Card
{
    public int PlayerId { get; set; }

    public BlockCard(int playerId, int cardId)
    {
        Id = cardId;
        PlayerId = playerId;
    }

    public override List<ICardIntent> GetIntents()
    {
        List<ICardIntent> intents = [ new Block(PlayerId, Id) ];
        return intents;
    }
}
