using System.Windows;
using System.Windows.Controls;

using QuivalLogicEngine.Cards;

namespace QuivalCardDesigner.Controls;

public partial class ValueControl : UserControl
{
    public ValueControl()
    {
        InitializeComponent();

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

        CountSideComboBox.ItemsSource = Enum.GetValues<CountSide>();
        CountValueSourceComboBox.ItemsSource = Enum.GetValues<CountValueSource>();

        FixedValueComboBox.ItemsSource = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        SetValueTypeOptionsVisibility();
    }

    public void PopulateValue(Ability ability, bool bonusValue = false)
    {
        if (ValueTypeComboBox.SelectedItem is Value value)
        {
            if (value is Count countValue)
            {
                countValue.CountSource = (CountValueSource)CountValueSourceComboBox.SelectedItem;
                countValue.CountSide = (CountSide)CountSideComboBox.SelectedItem;
            }

            if (bonusValue)
            {
                ability.BonusValue = value;
                ability.BonusValue.Amount = (int)FixedValueComboBox.SelectedItem;
            }
            else
            {
                ability.Value = value;
                ability.Value.Amount = (int)FixedValueComboBox.SelectedItem;
            }
        }
    }

    public void PopulateBonusValue(Ability ability)
    {
        if (ValueTypeComboBox.SelectedItem is Value value)
        {
            if (value is Count countValue)
            {
                countValue.CountSource = (CountValueSource)CountValueSourceComboBox.SelectedItem;
                countValue.CountSide = (CountSide)CountSideComboBox.SelectedItem;
            }

            ability.BonusValue = value;
            ability.BonusValue.Amount = (int)FixedValueComboBox.SelectedItem;
        }
    }

    private void ValueTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SetValueTypeOptionsVisibility();
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
}
