using System.Windows;

namespace QuivalCombatTestWPF;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ViewContent.Content = new MainMenu(this);
    }

    public void OpenMatchView()
    {
        ViewContent.Content = new MatchView();
    }
}
