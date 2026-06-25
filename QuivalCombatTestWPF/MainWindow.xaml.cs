using System.Windows;

namespace QuivalCombatTestWPF;

public partial class MainWindow : Window
{
    public QuivalClient Client { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        ViewContent.Content = new MainMenu(this);
        Client = new(this);
    }

    public void OpenMatchView()
    {
        ViewContent.Content = new MatchView(Client);
    }
}
