using System.Windows.Controls;

using QuivalLogicEngine.Cards;

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

            Type baseType = typeof(Target);

            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) &&
                    type != baseType &&
                    !type.IsAbstract)
                .ToList();


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
