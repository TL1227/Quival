using QuivalCardDesigner.Controls;
using QuivalLogicEngine.Cards;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using Trigger = QuivalLogicEngine.Cards.Trigger;
using Key = QuivalLogicEngine.CardDescription.CardDescriptionKeys;

namespace QuivalCardDesigner.Views;

public partial class CardDesignView : UserControl
{
    private CardDefinitionViewModel CurrentCardDefinitionViewModel { get; set; }

    private Config Config { get; set; }

    private bool LoadingCard { get; set; }

    public CardDesignView()
    {
        InitializeComponent();

        CardTypeComboBox.ItemsSource = Enum.GetValues<CardType>();
        CardTypeComboBox.SelectionChanged += CardTypeComboBox_SelectionChanged;

        AddTriggerButton.Click += AddTriggerButton_Click;

        TriggerTypeComboBox.DisplayMemberPath = "Name";
        PopulateTriggerTypeComboBox();

        SaveCardButton.Click += SaveCardButton_Click;

        HealthComboBox.ItemsSource = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        AttackComboBox.ItemsSource = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        CostComboBox.ItemsSource = new[]   { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        GenerateDescriptionButton.Click += GenerateDescriptionButton_Click;

        AddHandler(Selector.SelectionChangedEvent, new RoutedEventHandler(OnAnySelectionChanged));
        //AddHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(OnAnySelectionChanged));
        //AddHandler(ToggleButton.UncheckedEvent, new RoutedEventHandler(OnAnySelectionChanged));
    }

    public CardDesignView(Config config, string json) : this()
    {
        Config = config;

        var cardDef = JsonSerializer.Deserialize<CardDefinition>(json);
        if (cardDef != null)
        {
            LoadCard(cardDef);
        }
    }

    public CardDesignView(Config config) : this()
    {
        Config = config;

        LoadBlankCard();
    }

    public void PopulateTriggerTypeComboBox(CardType cardType = CardType.Creature)
    {
        var baseType = typeof(Trigger);
        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => baseType.IsAssignableFrom(type) &&
                type != baseType &&
                !type.IsAbstract)
            .ToList();

        List<Trigger> triggerTypes = new();

        if (cardType == CardType.Creature)
        {
            foreach (var type in types)
            {
                var triggerType = (Trigger)Activator.CreateInstance(type)!;
                triggerTypes.Add(triggerType);
            }
        }
        else if (cardType == CardType.Spell)
        {
            triggerTypes.Add(new CastTrigger());
        }

        TriggerTypeComboBox.ItemsSource = triggerTypes;
    }

    private void OnAnySelectionChanged(object sender, RoutedEventArgs e)
    {
        GenerateCardDescription();
    }

    private void GenerateDescriptionButton_Click(object sender, RoutedEventArgs e)
    {
        GenerateCardDescription();
    }

    private void GenerateCardDescription()
    {
        StringBuilder triggerString = new();
        foreach (TriggerControl triggerControl in TriggerListBox.Items)
        {
            var trigger = triggerControl.GetTrigger();

            if (CurrentCardDefinitionViewModel.CardType == CardType.Creature)
            {
                triggerString.Append("Whenever ");
                triggerString.Append(trigger.GetTriggerDescription().Replace(Key.CardName, CardNameTextBox.Text));
                triggerString.Append(", ");
            }

            List<string> abilityTexts = new();
            foreach (var ability in trigger.Abilities)
            {
                abilityTexts.Add(CardDescription.Ability(ability, trigger));
            }

            if (abilityTexts.Count > 1)
            {
                for (int i = 0; i < abilityTexts.Count; i++)
                {
                    triggerString.Append(abilityTexts[i]);

                    if (i != abilityTexts.Count - 1)
                    {
                        triggerString.Append($" {trigger.ChoiceType.ToString().ToLower()} ");
                    }
                }
            }
            else if (abilityTexts.Count > 0)
            {
                triggerString.Append(abilityTexts[0]);
            }

            triggerString.Append('.');
        }

        DescriptionTextBox.Text = triggerString.ToString();
    }

    private void CardTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CardTypeComboBox.SelectedItem is CardType type)
        {
            //TODO: some pop-up that will explain that changing the card type will remove all current triggers
            Visibility visibility = (type == CardType.Creature) ? Visibility.Visible : Visibility.Hidden;
            ToggleAtkDef(visibility);
            PopulateTriggerTypeComboBox(type);
            TriggerListBox.Items.Clear();

            CurrentCardDefinitionViewModel.CardType = type;
        }
    }

    private void LoadBlankCard()
    {
        CurrentCardDefinitionViewModel = new CardDefinitionViewModel();
        DataContext = CurrentCardDefinitionViewModel;

        CurrentCardPreview.CardBackground.Background = Brushes.CornflowerBlue;
    }

    private void LoadCard(CardDefinition cardDefinition)
    {
        LoadingCard = true;

        CurrentCardDefinitionViewModel = new CardDefinitionViewModel(cardDefinition);
        DataContext = CurrentCardDefinitionViewModel;

        foreach (var trigger in CurrentCardDefinitionViewModel.Triggers)
        {
            TriggerListBox.Items.Add(new TriggerControl(trigger));
        }

        CurrentCardPreview.CardBackground.Background = Brushes.CornflowerBlue;

        LoadingCard = false;
    }

    private void ToggleAtkDef(Visibility visibility)
    {
        if (visibility == Visibility.Collapsed)
        {
            AttackComboBox.IsEnabled = false;
            HealthComboBox.IsEnabled = false;
        }

        CurrentCardPreview.AttackLabel.Visibility = visibility;
        CurrentCardPreview.Divider.Visibility = visibility;
        CurrentCardPreview.HealthLabel.Visibility = visibility;
    }


    private void SaveCardButton_Click(object sender, RoutedEventArgs e)
    {
        var currentCardDefinition = CurrentCardDefinitionViewModel.GetCardDefinition();
        currentCardDefinition.Triggers = GetTriggersFromControls();

        string fileName = $"{Config.CardDirectory.FullName}\\{currentCardDefinition.Name}.csv";

        File.Delete(fileName);

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true;
        var json = JsonSerializer.Serialize(currentCardDefinition, options);
        File.AppendAllText(fileName, json);
    }

    private List<Trigger> GetTriggersFromControls()
    {
        List<Trigger> triggers = [];
        foreach (TriggerControl triggerControl in TriggerListBox.Items)
        {
            triggers.Add(triggerControl.GetTrigger());
        }

        return triggers;
    }

    private void AddTriggerButton_Click(object sender, RoutedEventArgs e)
    {
        if (TriggerTypeComboBox.SelectedItem is Trigger trigger)
        {
            TriggerListBox.Items.Add(new TriggerControl(trigger));
        }
    }
}
