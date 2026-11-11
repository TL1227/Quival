using System.Windows;
using System.Windows.Controls;

using QuivalLogicEngine.Cards;

namespace QuivalCardDesigner.Controls;

public partial class ValueControl : UserControl
{
    private Ability CurrentAbility;

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
        CountValueSourceComboBox.SelectionChanged += CountValueSourceComboBox_SelectionChanged;

        FixedValueComboBox.ItemsSource = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        FixedValueComboBox.SelectionChanged += FixedValueComboBox_SelectionChanged;

        SetValueTypeOptionsVisibility();
    }

    public void Initialise(Ability ability)
    {
        CurrentAbility = ability;
    }


    private void ValueTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SetValueTypeOptionsVisibility();
        UpdateValue();
    }

    private void CountValueSourceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateValue();
    }
    private void FixedValueComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        UpdateValue();
    }

    private void UpdateValue()
    {
        if (ValueTypeComboBox.SelectedItem is Value value)
        {
            if (value is Count count)
            {
                count.CountSource = (CountValueSource)CountValueSourceComboBox.SelectedItem;
                count.CountSide = (CountSide)CountSideComboBox.SelectedItem;
                CurrentAbility.Value = count;
            }
            else if (value is Fixed fix)
            {
                fix.Amount = (int)FixedValueComboBox.SelectedItem;
                CurrentAbility.Value = fix;
            }
        }
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
