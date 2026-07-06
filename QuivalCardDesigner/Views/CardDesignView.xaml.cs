using System;
using System.Windows;
using System.Windows.Controls;

//TODO: pull the shared references out of these and into some kind of quival core project
using QuivalLogicEngine.Cards;
using System.Windows.Media;
using System.Reflection;

namespace QuivalCardDesigner.Views;

public partial class CardDesignView : UserControl
{
    private MainWindow MainWindow { get; set; }
    private CardDefinition CurrentCardDefinition { get; set; }

    public CardDesignView(MainWindow mainWindow)
    {
        InitializeComponent();
        MainWindow = mainWindow;

        CostComboBox.SelectionChanged += CostComboBox_SelectionChanged;

        CardNameTextBox.TextChanged += TextBoxChanged;
        DescriptionTextBox.TextChanged += TextBoxChanged;
        AttackTextBox.TextChanged += TextBoxChanged;
        HealthTextBox.TextChanged += TextBoxChanged;

        LoadBlankCard();
    }


    private void LoadBlankCard()
    {
        CurrentCardDefinition = new CardDefinition();

        CurrentCard.CardNameLabel.Content = "";
        CurrentCard.CardDescriptionLabel.Text = CurrentCardDefinition.Description;
        CurrentCard.CostContent.Content = 0;
        //fullCard.CardBackground.Background = GetColor(CurrentCard);

        //NOTE: 
        CurrentCard.CardBackground.Background = Brushes.CornflowerBlue;

        /*
        string imagePath = GetImagePath(CurrentCard.Name);
        if (imagePath != "")
            fullCard.CardImage.Source = new BitmapImage(new Uri(imagePath));
        */

        //We could get this by querying the card database?
        CurrentCard.GlobalId.Content = CurrentCardDefinition.UniqueId;

        CurrentCard.AttackLabel.Content = 0;
        CurrentCard.HealthLabel.Content = 0;
    }

    #region Events

    private void CostComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CostComboBox.SelectedItem is ComboBoxItem item)
        {
            CurrentCard.CostContent.Content = item.Content;
        }
    }

    private void TextBoxChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (textBox.Name == "AttackTextBox")
            {
                CurrentCard.AttackLabel.Content = textBox.Text;
            }
            else if (textBox.Name == "HealthTextBox")
            {
                CurrentCard.HealthLabel.Content = textBox.Text;
            }
            else if (textBox.Name == "CardNameTextBox")
            {
                CurrentCard.CardNameLabel.Content = textBox.Text;
            }
            else if (textBox.Name == "DescriptionTextBox")
            {
                CurrentCard.CardDescriptionLabel.Text = textBox.Text;
            }
        }
    }
    #endregion
}
