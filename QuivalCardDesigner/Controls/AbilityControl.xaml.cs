using System.Windows;
using System.Windows.Controls;

using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Cards.Effects;

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
        CountSideComboBox.ItemsSource = Enum.GetValues<CountSide>();

        TargetTypeComboBox.ItemsSource = targets;
        TargetTypeComboBox.DisplayMemberPath = "DisplayName";
        TargetTypeComboBox.SelectionChanged += TargetTypeComboBox_SelectionChanged;

        TargetPoolComboBox.ItemsSource = targetPools;
        TargetPoolComboBox.SelectionChanged += TargetPoolComboBox_SelectionChanged;

        SetSelectionTargetOptionsVisibility();

        Type baseType = typeof(Value);
        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => baseType.IsAssignableFrom(type) &&
                type != baseType &&
                !type.IsAbstract)
            .ToList();

        List<Value> valueItems = new();
        foreach (var type in types)
        {
            var valueType = (Value)Activator.CreateInstance(type)!;
            valueItems.Add(valueType);
        }

        ValueTypeComboBox.ItemsSource = valueItems;
        ValueTypeComboBox.DisplayMemberPath = "Name";
        ValueTypeComboBox.SelectionChanged += ValueTypeComboBox_SelectionChanged;

        CountValueSourceComboBox.ItemsSource = Enum.GetValues<CountValueSource>();


        SetValueTypeOptionsVisibility();
    }

    private void ValueTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SetValueTypeOptionsVisibility();
    }

    private void TargetPoolComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyTargetPoolSelection();
    }

    private void SetValueTypeOptionsVisibility()
    {
        if (ValueTypeComboBox.SelectedItem is Count value)
        {
            CountValueSourceLabel.Visibility = Visibility.Visible;
            CountValueSourceComboBox.Visibility = Visibility.Visible;

            CountSideLabel.Visibility = Visibility.Visible;
            CountSideComboBox.Visibility = Visibility.Visible;
        }
        else
        {
            CountValueSourceLabel.Visibility = Visibility.Collapsed;
            CountValueSourceComboBox.Visibility = Visibility.Collapsed;

            CountSideLabel.Visibility = Visibility.Collapsed;
            CountSideComboBox.Visibility = Visibility.Collapsed;
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

    private void SetSelectionTargetOptionsVisibility()
    {
        ChangeSelectionTargetOptionsVisibility(Visibility.Collapsed);

        if (TargetTypeComboBox.SelectedItem is SelectionTarget)
        {
            ApplyTargetPoolSelection();
        }
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

    private void TargetTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SetSelectionTargetOptionsVisibility();
    }
}
