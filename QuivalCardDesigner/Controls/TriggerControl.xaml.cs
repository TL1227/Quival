using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Cards.Effects;
using System.Windows.Controls;

namespace QuivalCardDesigner.Controls
{
    public partial class TriggerControl : UserControl
    {
        private Trigger CurrentTrigger { get; set; }

        public TriggerControl(Trigger trigger)
        {
            InitializeComponent();

            CurrentTrigger = trigger;
            TriggerNameLabel.Content = trigger.GetType().Name;
                
            if (trigger is CastTrigger)
            {
                TriggerTypeStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                TriggerNameLabel.Content = trigger.GetType().Name;
                TriggerTypeComboBox.ItemsSource = trigger.GetEnums();
            }

            if (trigger is ListeningTrigger)
            {
                TriggerSideLabel.Visibility = System.Windows.Visibility.Visible;
                TriggerSideComboBox.Visibility = System.Windows.Visibility.Visible;
                TriggerSideComboBox.ItemsSource = Enum.GetValues<Side>();
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

            return CurrentTrigger;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string thign = Parent.GetType().ToString();
            if (Parent is ItemsControl control)
            {
                control.Items.Remove(this);
            }
        }

        private void AddAbilityButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectedType = (Type)AbilitiyEffectComboBox.SelectedItem;
            var effect = (Effect)Activator.CreateInstance(selectedType)!;
            AbilitiesListBox.Items.Add(new AbilityControl(effect));
        }

        private void ToggleCollapse_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ContentPanel.Visibility == System.Windows.Visibility.Visible)
                ContentPanel.Visibility = System.Windows.Visibility.Collapsed;
            else
                ContentPanel.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
