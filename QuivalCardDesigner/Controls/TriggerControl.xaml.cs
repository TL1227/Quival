using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Cards.Effects;
using System.Windows.Controls;
using System.Windows;

using Trigger = QuivalLogicEngine.Cards.Trigger;

namespace QuivalCardDesigner.Controls
{
    public partial class TriggerControl : UserControl
    {
        public Trigger CurrentTrigger { get; set; }

        public TriggerControl(Trigger trigger)
        {
            InitializeComponent();

            CurrentTrigger = trigger;
            TriggerNameLabel.Content = trigger.GetType().Name;
                
            if (trigger is CastTrigger)
            {
                TriggerTypeStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                TriggerNameLabel.Content = trigger.GetType().Name;
                TriggerTypeComboBox.ItemsSource = trigger.GetEnums();
                TriggerTypeComboBox.SelectionChanged += TriggerTypeComboBox_SelectionChanged;
            }

            if (trigger is ListeningTrigger)
            {
                TriggerSideLabel.Visibility = Visibility.Visible;
                TriggerSideComboBox.Visibility = Visibility.Visible;
                TriggerSideComboBox.ItemsSource = Enum.GetValues<Side>();
                TriggerSideComboBox.SelectionChanged += TriggerSideComboBox_SelectionChanged;
            }

            Type baseType = typeof(Effect);
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) &&
                    type != baseType &&
                    !type.IsAbstract)
                .ToList();

            AbilitiyEffectComboBox.ItemsSource = types;
            AbilitiyEffectComboBox.DisplayMemberPath = "Name";

            ToggleCollapse.Click += ToggleCollapse_Click;
            AddAbilityButton.Click += AddAbilityButton_Click;

            ContextMenu menu = new();
            MenuItem menuItem = new MenuItem() { Header = "Delete" };
            menuItem.Click += Delete_Click;
            menu.Items.Add(menuItem);
            TriggerControlHeader.ContextMenu = menu;

            EffectChoiceTypeComboBox.SelectionChanged += EffectChoiceTypeComboBox_SelectionChanged;
        }

        private void TriggerSideComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentTrigger is ListeningTrigger listeningTrigger && TriggerSideComboBox.SelectedItem != null)
            {
                listeningTrigger.Side = (Side)TriggerSideComboBox.SelectedIndex;
            }
        }

        private void TriggerTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentTrigger is SelfTrigger selfTrigger)
            {
                if (TriggerTypeComboBox.SelectedItem != null)
                    selfTrigger.SelfTriggerType = (SelfTriggerType)TriggerTypeComboBox.SelectedIndex;
            }
            else if (CurrentTrigger is ListeningTrigger listeningTrigger)
            {
                if (TriggerTypeComboBox.SelectedItem != null)
                    listeningTrigger.ListeningTriggerType = (ListeningTriggerType)TriggerTypeComboBox.SelectedIndex;
            }
        }

        private void EffectChoiceTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EffectChoiceTypeComboBox.SelectedItem is ChoiceType type)
            {
                switch (type)
                {
                    case ChoiceType.And:
                    case ChoiceType.Or:
                        EffectChoiceNumberComboBox.Visibility = Visibility.Collapsed;
                        EffectChoiceNumberComboBox.SelectedIndex = 0;
                        break;
                    case ChoiceType.PickNumber:
                    case ChoiceType.PickUpTo:
                        EffectChoiceNumberComboBox.ItemsSource = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        EffectChoiceNumberComboBox.SelectedIndex = 1;
                        EffectChoiceNumberComboBox.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
        }

        public void HideEffectChoices()
        {
            EffectChoiceNumberComboBox.Visibility = Visibility.Collapsed;
            EffectChoiceNumberComboBox.SelectedIndex = 0;

            EffectChoiceTypeLabel.Visibility = Visibility.Collapsed;
            EffectChoiceTypeComboBox.Visibility = Visibility.Collapsed;
            EffectChoiceTypeComboBox.SelectedIndex = 0;
        }

        public Trigger GetTrigger()
        {
            CurrentTrigger.Abilities.Clear();

            int abilityId = 0;
            foreach (var item in AbilitiesListBox.Items)
            {
                if (item is AbilityControl abilityControl)
                {
                    var ability = abilityControl.GetAbility();
                    ability.Id = abilityId++;
                    CurrentTrigger.Abilities.Add(ability);
                }
            }

            if (EffectChoiceTypeComboBox.SelectedItem != null)
                CurrentTrigger.ChoiceType = (ChoiceType)EffectChoiceTypeComboBox.SelectedItem;

            if (EffectChoiceNumberComboBox.SelectedItem != null)
                CurrentTrigger.ChoiceNumber = (int)EffectChoiceNumberComboBox.SelectedItem;

            return CurrentTrigger;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string thign = Parent.GetType().ToString();
            if (Parent is ItemsControl control)
            {
                control.Items.Remove(this);
            }
        }

        private void AddAbilityButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedType = (Type)AbilitiyEffectComboBox.SelectedItem;
            var effect = (Effect)Activator.CreateInstance(selectedType)!;
            AbilitiesListBox.Items.Add(new AbilityControl(effect));

            if (AbilitiesListBox.Items.Count > 1)
            {
                EffectChoiceTypeLabel.Visibility = Visibility.Visible;
                EffectChoiceTypeComboBox.Visibility = Visibility.Visible;
                EffectChoiceTypeComboBox.ItemsSource = Enum.GetValues<ChoiceType>();
            }
        }

        private void ToggleCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (ContentPanel.Visibility == Visibility.Visible)
                ContentPanel.Visibility = Visibility.Collapsed;
            else
                ContentPanel.Visibility = Visibility.Visible;
        }
    }
}
