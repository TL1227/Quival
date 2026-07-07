using System.Windows.Controls;

using QuivalLogicEngine.Cards;

namespace QuivalCardDesigner.Controls
{
    public partial class TriggerControl : UserControl
    {
        public Trigger CurrentTrigger { get; set; } = new();

        public TriggerControl(TriggerType triggerType)
        {
            InitializeComponent();

            CurrentTrigger = new()
            {
                TriggerType = triggerType
            };

            TriggerTypeLabel.Content = triggerType;

            SideComboBox.ItemsSource = Enum.GetValues<Side>();
            ChoiceTypeComboBox.ItemsSource = Enum.GetValues<ChoiceType>();
            AbilitiyEffectComboBox.ItemsSource = Enum.GetValues<Effect>();

            ToggleCollapse.Click += ToggleCollapse_Click;

            AddAbilityButton.Click += AddAbilityButton_Click;
        }

        private void AddAbilityButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (AbilitiyEffectComboBox.SelectedItem is Effect effect)
            {
                AbilityControl ab = new(effect);
                AbilitiesListBox.Items.Add(ab);
            }
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
