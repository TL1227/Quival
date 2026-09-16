using System;
using System.Windows;
using System.Windows.Controls;

//TODO: pull the shared references out of these and into some kind of quival core project
using QuivalLogicEngine.Cards;
using System.Windows.Media;
using System.Reflection;
using System.Diagnostics;
using QuivalCardDesigner.Controls;
using System.Windows.Media.Animation;
using System.IO;
using System.Text.Json;
using Trigger = QuivalLogicEngine.Cards.Trigger;

namespace QuivalCardDesigner.Views;

public partial class CardDesignView : UserControl
{
    private MainWindow MainWindow { get; set; }

    private CardDefinition CurrentCardDefinition { get; set; }

    private Config Config { get; set; }
    
    public CardDesignView(Config config)
    {
        InitializeComponent();
        Config = config;

        CardTypeComboBox.ItemsSource = Enum.GetValues<CardType>();
        CardTypeComboBox.SelectionChanged += CardTypeComboBox_SelectionChanged;

        CostComboBox.SelectionChanged += CostComboBox_SelectionChanged;

        CardNameTextBox.TextChanged += TextBoxChanged;
        DescriptionTextBox.TextChanged += TextBoxChanged;
        AttackComboBox.SelectionChanged += AttackComboBox_SelectionChanged;
        HealthComboBox.SelectionChanged += HealthComboBox_SelectionChanged;

        AddTriggerButton.Click += AddTriggerButton_Click;

        var baseType = typeof(Trigger);
        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => baseType.IsAssignableFrom(type) &&
                type != baseType &&
                !type.IsAbstract)
            .ToList();

        List<Trigger> triggerTypes = new();
        foreach (var type in types)
        {
            var triggerType = (Trigger)Activator.CreateInstance(type)!;
            triggerTypes.Add(triggerType);
        }

        TriggerTypeComboBox.ItemsSource = triggerTypes;
        TriggerTypeComboBox.DisplayMemberPath = "Name";

        SaveCardButton.Click += SaveCardButton_Click;

        HealthComboBox.ItemsSource = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        AttackComboBox.ItemsSource = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        CostComboBox.ItemsSource = new[]   { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        LoadBlankCard();
    }

    private void HealthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => CurrentCard.HealthLabel.Content = (int)HealthComboBox.SelectedItem;

    private void AttackComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => CurrentCard.AttackLabel.Content = (int)AttackComboBox.SelectedItem;

    private void CostComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => CurrentCard.CostContent.Content = (int)CostComboBox.SelectedItem;

    private void LoadBlankCard()
    {
        CurrentCardDefinition = new CardDefinition();

        CurrentCard.CardNameLabel.Content = "";
        CurrentCard.CardDescriptionLabel.Text = CurrentCardDefinition.Description;
        CurrentCard.CostContent.Content = 0;
        //fullCard.CardBackground.Background = GetColor(CurrentCard);

        //NOTE: 
        CurrentCard.CardBackground.Background = Brushes.CornflowerBlue;

        /*
        string imagePath = GetImagePath(CurrentCard.Name);
        if (imagePath != "")
            fullCard.CardImage.Source = new BitmapImage(new Uri(imagePath));
        */

        //We could get this by querying the card database?
        CurrentCard.GlobalId.Content = CurrentCardDefinition.UniqueId;

        CurrentCard.AttackLabel.Content = 0;
        CurrentCard.HealthLabel.Content = 0;
    }

    private void ToggleAtkDef(Visibility visibility)
    {
        if (visibility == Visibility.Collapsed)
        {
            AttackComboBox.IsEnabled = false;
            HealthComboBox.IsEnabled = false;
        }

        CurrentCard.AttackLabel.Visibility = visibility;
        CurrentCard.Divider.Visibility = visibility;
        CurrentCard.HealthLabel.Visibility = visibility;
    }

    #region Events
    private void CardTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CardTypeComboBox.SelectedItem is CardType type)
        {
            Visibility visibility = (type == CardType.Creature) ? Visibility.Visible : Visibility.Hidden;
            ToggleAtkDef(visibility);
        }
    }

    private void SaveCardButton_Click(object sender, RoutedEventArgs e)
    {
        CardDefinition? card = GetCardFromCurrentView();

        if (card != null)
        {
            string fileName = $"{Config.CardDirectory.FullName}\\{card.Name}.csv";

            File.Delete(fileName);

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            var json = JsonSerializer.Serialize(card, options);
            File.AppendAllText(fileName, json);
        }
    }

    private CardDefinition? GetCardFromCurrentView()
    {
        if (CardTypeComboBox.SelectedItem is CardType cardType)
        {
            CardDefinition card = new();

            try
            {
                card.CardType = cardType;

                if (string.IsNullOrWhiteSpace(CardNameTextBox.Text))
                {
                    MessageBox.Show("Please enter a card name!");
                    return null;
                }
                else
                {
                    card.Name = CardNameTextBox.Text;
                }

                card.Description = DescriptionTextBox.Text;

                if (CostComboBox.SelectedItem is int cost)
                {
                    card.Cost = cost;
                }

                if (card.CardType is CardType.Creature)
                {
                    card.Attack = (int)AttackComboBox.SelectedItem;
                    card.Health = (int)HealthComboBox.SelectedItem;
                }

                foreach (var item in TriggerListBox.Items) 
                    if (item is TriggerControl triggerControl)
                    {
                        var trigger = triggerControl.GetTrigger();

                        foreach (var ability in trigger.Abilities)
                        {
                            if (ability.BonusEffect != null)
                            {
                                if (ability.BonusConditionals == null || 
                                    ability.BonusConditionals.Count == 0)
                                {
                                    MessageBox.Show("A Bonus effect needs a conditional!");
                                    return null;
                                }
                            }
                        }

                        card.Triggers.Add(trigger);
                    }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return card;
        }
        else
        {
            return null;
        }
    }

    private void TextBoxChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (textBox.Name == "AttackTextBox")
            {
                CurrentCard.AttackLabel.Content = textBox.Text;
            }
            else if (textBox.Name == "HealthTextBox")
            {
                CurrentCard.HealthLabel.Content = textBox.Text;
            }
            else if (textBox.Name == "CardNameTextBox")
            {
                CurrentCard.CardNameLabel.Content = textBox.Text;
            }
            else if (textBox.Name == "DescriptionTextBox")
            {
                CurrentCard.CardDescriptionLabel.Text = textBox.Text;
            }
        }
    }

    private void AddTriggerButton_Click(object sender, RoutedEventArgs e)
    {
        if (TriggerTypeComboBox.SelectedItem is QuivalLogicEngine.Cards.Trigger trigger)
        {
            TriggerListBox.Items.Add(new TriggerControl(trigger));
        }
    }

    #endregion
}
