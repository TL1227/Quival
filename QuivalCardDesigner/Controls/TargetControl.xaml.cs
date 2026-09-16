using System.Windows;
using System.Windows.Controls;

using QuivalLogicEngine.Cards;

namespace QuivalCardDesigner.Controls;

public partial class TargetControl : UserControl
{
    public EventHandler ChangeToTarget;

    public TargetControl()
    {
        InitializeComponent();
    }

    public void Initialise(Ability ability)
    {
        List<Target> targets = new();
        List<TargetPool> targetPools = new();
        switch (ability.Effect.ValidTargetPool)
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

        TargetTypeComboBox.ItemsSource = targets;
        TargetTypeComboBox.DisplayMemberPath = "DisplayName";
        TargetTypeComboBox.SelectionChanged += TargetTypeComboBox_SelectionChanged;

        TargetPoolComboBox.ItemsSource = targetPools;
        TargetPoolComboBox.SelectionChanged += TargetPoolComboBox_SelectionChanged;

        TargetNumberCombobox.ItemsSource = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        SideComboBox.ItemsSource = Enum.GetValues<Side>();

        SetSelectionTargetOptionsVisibility();
    }

    public void PopulateTarget(Ability ability)
    {
        if (TargetTypeComboBox.SelectedItem is Target target)
        {
            if (target is SelectionTarget st)
            {
                st.TargetPool = (TargetPool)TargetPoolComboBox.SelectedItem;
                st.Side = (Side)SideComboBox.SelectedItem;
                st.CanTargetSelf = (bool)SelfTargetCheckbox.IsChecked!;
                st.NumberToPick = (int)TargetNumberCombobox.SelectedItem;
            }

            ability.Target = target;
        }

        //ValueControl.PopulateValue(ability);
    }

    private void ChangeSelectionTargetOptionsVisibility(Visibility visibility)
    {
        TargetPoolLabel.Visibility = visibility;
        TargetPoolComboBox.Visibility = visibility;

        SideLabel.Visibility = visibility;
        SideComboBox.Visibility = visibility;

        SelfTargetLabel.Visibility = visibility;
        SelfTargetCheckbox.Visibility = visibility;

        TargetNumberLabel.Visibility = visibility;
        TargetNumberCombobox.Visibility = visibility;
    }

    private void SetSelectionTargetOptionsVisibility()
    {
        ChangeSelectionTargetOptionsVisibility(Visibility.Collapsed);

        if (TargetTypeComboBox.SelectedItem is SelectionTarget)
        {
            ApplyTargetPoolSelection();
        }
    }

    private void ApplyTargetPoolSelection()
    {
        ChangeSelectionTargetOptionsVisibility(Visibility.Visible);

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
            //SelfTargetCheckbox.Visibility = Visibility.Visible;
            //SelfTargetLabel.Visibility = Visibility.Visible;

            SideComboBox.IsEnabled = true;
            TargetNumberCombobox.IsEnabled = true;
        }
    }

    private void TargetPoolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyTargetPoolSelection();
        ChangeToTarget.Invoke(sender, e);
    }

    private void TargetTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SetSelectionTargetOptionsVisibility();
        ChangeToTarget.Invoke(sender, e);
    }
}
