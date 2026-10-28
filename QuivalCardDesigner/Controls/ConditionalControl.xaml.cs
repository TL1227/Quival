using System.Windows;
using System.Windows.Controls;
using QuivalLogicEngine.Cards;

namespace QuivalCardDesigner.Controls;

public partial class ConditionalControl : UserControl
{
    private List<Conditional> CurrentConditionals { get; set; } = new();

    public ConditionalControl()
    {
        InitializeComponent();

        ConditionalsComboBox.ItemsSource = Enum.GetValues<Conditional>();
        ConditionalTypeComboBox.ItemsSource = Enum.GetValues<ConditionalType>();
        AddConditionalButton.Click += AddConditionalButton_Click;
    }

    public void SetConditionals(List<Conditional>? conditionals)
    {
        if (conditionals != null)
        {
            CurrentConditionals = conditionals;

            foreach (Conditional conditional in CurrentConditionals)
            {
                ConditionalsListBox.Items.Add(conditional);
            }
        }
    }

    private void AddConditionalButton_Click(object sender, RoutedEventArgs e)
    {
        if (ConditionalsComboBox.SelectedItem is Conditional conditional)
        {
            if (ConditionalsListBox.Items.Contains(conditional))
            {
                MessageBox.Show($"Already added {conditional}");
            }
            else
            {
                ConditionalsListBox.Items.Add(conditional);
                CurrentConditionals.Add(conditional);
            }

            //set visibilities
            if (ConditionalsListBox.Items.Count > 0)
            {
                ConditionalsListBox.Visibility = Visibility.Visible;
            }

            if (ConditionalsListBox.Items.Count > 1)
            {
                ConditionalTypeComboBox.Visibility = Visibility.Visible;
                ConditionalTypeLabel.Visibility = Visibility.Visible;
            }
        }
    }
}
