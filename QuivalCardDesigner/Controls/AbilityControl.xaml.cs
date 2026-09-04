using System.Windows;
using System.Windows.Controls;

using QuivalLogicEngine.Cards;

namespace QuivalCardDesigner.Controls;
public partial class AbilityControl : UserControl
{
    Ability CurrentAbility { get; set; } = new();

    public AbilityControl(Effect effect)
    {
        InitializeComponent();
        CurrentAbility.Effect = effect;
        EffectLabel.Content = effect.GetType().Name.Replace("Effect", "");

        List<Target> targets = new();
        List<TargetPool> targetPools = new();
        switch (CurrentAbility.Effect.ValidTargetPool)
        {
            case TargetPool.Creatures:
                targets.AddRange([new SelfTarget(), new SelectionTarget()]);
                targetPools.AddRange([TargetPool.Creatures]);
                break;
            case TargetPool.Controllers:
                targets.AddRange([new PlayerTarget(), new OpponentTarget(), new SelectionTarget()]);
                targetPools.AddRange([TargetPool.Controllers]);
                break;
            case TargetPool.Damagables:
                targets.AddRange([new SelfTarget(), new PlayerTarget(), new OpponentTarget(), new SelectionTarget()]);
                targetPools.AddRange([TargetPool.Damagables, TargetPool.Creatures, TargetPool.Controllers]);
                break;
            default:
                break;
        }

        SideComboBox.ItemsSource = Enum.GetValues<Side>();

        TargetTypeComboBox.ItemsSource = targets;
        TargetTypeComboBox.DisplayMemberPath = "DisplayName";
        TargetTypeComboBox.SelectionChanged += TargetTypeComboBox_SelectionChanged;

        TargetPoolComboBox.ItemsSource = targetPools;
        TargetPoolComboBox.SelectionChanged += TargetPoolComboBox_SelectionChanged;

        SetSelectionTargetOptionsVisibility(Visibility.Hidden);
    }

    private void TargetPoolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TargetPoolComboBox.SelectedItem is TargetPool.Controllers)
        {
            SelfTargetCheckbox.IsChecked = false;
            SelfTargetCheckbox.Visibility = Visibility.Collapsed;
            SelfTargetLabel.Visibility = Visibility.Collapsed;
            SideComboBox.SelectedIndex = 0;
            SideComboBox.IsEnabled = false;
        }
        else
        {
            SelfTargetCheckbox.Visibility = Visibility.Visible;
            SelfTargetLabel.Visibility = Visibility.Visible;
            SideComboBox.IsEnabled = true;
            TargetNumberCombobox.IsEnabled = true;
        }
    }

    private void SetSelectionTargetOptionsVisibility(Visibility visibility)
    {
        SelfTargetLabel.Visibility = Visibility.Collapsed;
        SelfTargetCheckbox.Visibility = Visibility.Collapsed;

        switch (CurrentAbility.Effect.ValidTargetPool)
        {
            case TargetPool.Creatures:
            case TargetPool.Damagables:
                SelfTargetLabel.Visibility = visibility;
                SelfTargetCheckbox.Visibility = visibility;
                break;
            default:
                break;
        }

        TargetPoolLabel.Visibility = visibility;
        TargetPoolComboBox.Visibility = visibility;

        SideLabel.Visibility = visibility;
        SideComboBox.Visibility = visibility;

        SelfTargetLabel.Visibility = visibility;
        SelfTargetCheckbox.Visibility = visibility;

        TargetNumberLabel.Visibility = visibility;
        TargetNumberCombobox.Visibility = visibility;
    }

    private void TargetTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        if (TargetTypeComboBox.SelectedItem is SelectionTarget)
        {
            SetSelectionTargetOptionsVisibility(Visibility.Visible);
        }
        else
        {
            SetSelectionTargetOptionsVisibility(Visibility.Collapsed);
        }
    }
}
