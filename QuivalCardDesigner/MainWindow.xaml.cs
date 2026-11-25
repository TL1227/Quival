using QuivalCardDesigner.Views;
using QuivalLogicEngine.Cards;
using System.Windows.Input;
using System.Windows;

namespace QuivalCardDesigner;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        CurrentView.Content = new MainMenuView(this);

        KeyDown += MainWindow_KeyDown;

        Config.CardDirectory = new("Cards");

        NewCardButton.Click += NewCardButton_Click;
        SaveCardButton.Click += SaveCardButton_Click;
        LoadCardButton.Click += LoadCardButton_Click;
        MenuButton.Click += MenuButton_Click;
    }

    private void ToggleMenuVisibility(Visibility visibility)
    {
        NewCardButton.Visibility = visibility;
        SaveCardButton.Visibility = visibility;
        LoadCardButton.Visibility = visibility;
        MenuButton.Visibility = visibility;
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        ToggleMenuVisibility(Visibility.Collapsed);
        CurrentView.Content = new MainMenuView(this);
    }

    private void LoadCardButton_Click(object sender, RoutedEventArgs e)
    {
        ViewCardList();
    }

    public void ViewCardList()
    {
        ToggleMenuVisibility(Visibility.Visible);
        CurrentView.Content = new CardListView(this);
    }

    private void SaveCardButton_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentView.Content is CardDesignView view)
        {
            view.SaveCard();
        }
    }

    private void NewCardButton_Click(object sender, RoutedEventArgs e)
    {
        OpenNewCardView();
    }

    public void OpenNewCardView()
    {
        ToggleMenuVisibility(Visibility.Visible);
        CurrentView.Content = new CardDesignView();
    }

    public void OpenExistingCardView(CardDefinition card)
    {
        ToggleMenuVisibility(Visibility.Visible);
        CurrentView.Content = new CardDesignView(card);
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                Environment.Exit(-1);
                e.Handled = true;
                break;
            default:
                break;
        }
    }
}