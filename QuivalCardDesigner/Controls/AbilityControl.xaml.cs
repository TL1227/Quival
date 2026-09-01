using System.Windows;
using System.Windows.Controls;

using QuivalLogicEngine.Cards;

namespace QuivalCardDesigner.Controls
{
    public partial class AbilityControl : UserControl
    {
        Ability CurrentAbility { get; set; } = new();

        public AbilityControl(Effect effect)
        {
            InitializeComponent();
            CurrentAbility.Effect = effect;
            EffectLabel.Content = effect.GetType().Name.Replace("Effect", "");


            /*
                NOTE: CreatureTarget class doesn't make sense in this context and should only be used for the 
                selection target pool. I'm not sure if this means we should go about the design a different
                or just live with the exception here. 
            */

            Type baseType = typeof(Target);
            var targets = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) &&
                    type != baseType &&
                    !type.IsAbstract)
                .ToList();

            TargetTypeComboBox.ItemsSource = targets;
            TargetTypeComboBox.DisplayMemberPath = "Name";
            TargetTypeComboBox.SelectionChanged += TargetTypeComboBox_SelectionChanged;

            SetSelectionTargetOptionsVisibility(Visibility.Hidden);
        }

        private void SetSelectionTargetOptionsVisibility(Visibility visibility)
        {
            SelfTargetLabel.Visibility = Visibility.Collapsed;
            SelfTargetCheckbox.Visibility = Visibility.Collapsed;

            CreatureTargetLabel.Visibility = Visibility.Collapsed;
            CreatureTargetCheckbox.Visibility = Visibility.Collapsed;

            /*
            OpponentTargetLabel.Visibility = Visibility.Collapsed;
            OpponentTargetCheckbox.Visibility = Visibility.Collapsed;

            PlayerTargetLabel.Visibility = Visibility.Collapsed;
            PlayerTargetCheckbox.Visibility = Visibility.Collapsed;
            */

            foreach (var validTarget in CurrentAbility.Effect.ValidTargets)
            {
                switch (validTarget)
                {
                    case TargetPool.Direct:
                        break;
                    case TargetPool.Creature:
                        SelfTargetLabel.Visibility = visibility;
                        SelfTargetCheckbox.Visibility = visibility;
                        break;
                    default:
                        break;
                }
            }


            SideLabel.Visibility = visibility;
            SideComboBox.Visibility = visibility;

            SelfTargetLabel.Visibility = visibility;
            SelfTargetCheckbox.Visibility = visibility;

            TargetNumberLabel.Visibility = visibility;
            TargetNumberCombobox.Visibility = visibility;
        }

        private void TargetTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedType = (Type)TargetTypeComboBox.SelectedItem;
            var target = (Target)Activator.CreateInstance(selectedType)!;

            if (target is SelectionTarget)
            {
                SetSelectionTargetOptionsVisibility(Visibility.Visible);
            }
            else
            {
                SetSelectionTargetOptionsVisibility(Visibility.Hidden);
            }
        }
    }
}
