namespace QuivalCardDesigner;

using QuivalLogicEngine.Cards;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class CardDefinitionViewModel : INotifyPropertyChanged
{
    private string _uniqueId = string.Empty;
    private CardType _cardType;
    private string _name = string.Empty;
    private string? _description;
    private string? _flavourText;
    private int _cost;
    private int _attack;
    private int _health;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string UniqueId
    {
        get => _uniqueId;
        set => SetProperty(ref _uniqueId, value);
    }

    public CardType CardType
    {
        get => _cardType;
        set => SetProperty(ref _cardType, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string? FlavourText
    {
        get => _flavourText;
        set => SetProperty(ref _flavourText, value);
    }

    public int Cost
    {
        get => _cost;
        set => SetProperty(ref _cost, value);
    }

    public int Attack
    {
        get => _attack;
        set => SetProperty(ref _attack, value);
    }

    public int Health {
        get => _health;
        set => SetProperty(ref _health, value);
    }

    public ObservableCollection<Trigger> Triggers { get; } = new();

    public ObservableCollection<Ability> PassiveAbilities { get; } = new();

    public CardDefinitionViewModel()
    {
    }

    public CardDefinition GetCardDefinition()
    {
        CardDefinition cardDefinition = new();

        cardDefinition.UniqueId = UniqueId;
        cardDefinition.CardType = CardType;
        cardDefinition.Name = Name;
        cardDefinition.Description = Description;
        cardDefinition.FlavourText = FlavourText;
        cardDefinition.Cost = Cost;
        cardDefinition.Attack = Attack;
        cardDefinition.Health = Health;

        //cardDefinition.Triggers = Triggers.ToList();
        //cardDefinition.PassiveAbilities = PassiveAbilities.ToList();

        return cardDefinition;
    }

    public CardDefinitionViewModel(CardDefinition definition)
    {
        UniqueId = definition.UniqueId;
        CardType = definition.CardType;
        Name = definition.Name;
        Description = definition.Description;
        FlavourText = definition.FlavourText;
        Cost = definition.Cost;
        Attack = definition.Attack;
        Health = definition.Health;

        foreach (var trigger in definition.Triggers)
            Triggers.Add(trigger);

        foreach (var ability in definition.PassiveAbilities)
            PassiveAbilities.Add(ability);
    }

    protected virtual void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
