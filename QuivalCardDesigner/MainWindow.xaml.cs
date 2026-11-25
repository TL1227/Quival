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
    }

    public void OpenNewCardView()
    {
        BackButton.Visibility = Visibility.Visible;
        CurrentView.Content = new CardDesignView();

        BackButton.Click += ReturnToMainMenu;
    }

    public void ViewCardList()
    {
        BackButton.Visibility = Visibility.Visible;
        CurrentView.Content = new CardListView(this);

        BackButton.Click += ReturnToMainMenu;
    }

    private void ViewCardList(object sender, RoutedEventArgs e)
    {
        ViewCardList();
    }

    public void OpenExistingCardView(CardDefinition card)
    {
        BackButton.Visibility = Visibility.Visible;
        CurrentView.Content = new CardDesignView(card);

        BackButton.Click += ViewCardList;
    }


    private void ReturnToMainMenu(object sender, RoutedEventArgs e)
    {
        BackButton.Visibility = Visibility.Collapsed;
        CurrentView.Content = new MainMenuView(this);
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