//using QuivalLogicEngine;
namespace QuivalLogicEngine.Cards;

public interface ICard
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public List<ICardIntent> GetIntents();
}

public class CreatureCard : ICard
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public int Attack { get; set; }
    public int Health { get; set; }

    public CreatureCard(int id)
    {
        Id = id;
        Name = "Attack!";
        Description = "Attack the opponent.";
    }

    public List<ICardIntent> GetIntents()
    {
        List<ICardIntent> intents = [ new Attack(Id) ];
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

        return new List<ICardIntent>();
    }
}
