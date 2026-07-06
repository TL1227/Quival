using QuivalCardDesigner.Views;
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
    }

    public void OpenCardDesignView()
    {
        CurrentView.Content = new CardDesignView(this);
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