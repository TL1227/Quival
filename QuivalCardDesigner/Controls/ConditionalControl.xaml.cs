using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using QuivalLogicEngine.Cards;

namespace QuivalCardDesigner.Controls;

public partial class ConditionalControl : UserControl
{
    public ConditionalControl()
    {
        InitializeComponent();

        ConditionalsComboBox.ItemsSource = Enum.GetValues<Conditional>();
        ConditionalTypeComboBox.ItemsSource = Enum.GetValues<ConditionalType>();
        AddConditionalButton.Click += AddConditionalButton_Click;
    }

    public void PopulateConditional(Ability ability, bool bonusConditional = false)
    {
        foreach (var item in ConditionalsListBox.Items)
        {
            if (item is Conditional conditional)
            {
                if (bonusConditional)
                {
                    ability.BonusConditionals.Add(conditional);
                }
                else
                {
                    ability.Conditionals.Add(conditional);
                }
            }
        }

        if (ConditionalTypeComboBox.SelectedItem is ConditionalType conType)
        {
            if (bonusConditional)
            {
                ability.BonusConditionalType = conType;
            }
            else
            {
                ability.ConditionalType = conType;
            }
        }
    }

    private void AddConditionalButton_Click(object sender, RoutedEventArgs e)
    {
        ConditionalsListBox.Visibility = Visibility.Visible;

        if (ConditionalsListBox.Items.Contains(ConditionalsComboBox.SelectedItem))
        {
            MessageBox.Show($"Already added {ConditionalsComboBox.SelectedItem}");
        }
        else
        {
            ConditionalsListBox.Items.Add(ConditionalsComboBox.SelectedItem);
        }

        if (ConditionalsListBox.Items.Count > 1)
        {
            ConditionalTypeComboBox.Visibility = Visibility.Visible;
            ConditionalTypeLabel.Visibility = Visibility.Visible;
        }
    }
}
