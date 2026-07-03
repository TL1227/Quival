using System.Windows;

namespace QuivalCombatTestWPF;

public partial class MainWindow : Window
{
    public QuivalClient Client { get; set; }
    private bool FullScreen = false;

    public MainWindow()
    {
        InitializeComponent();
        ViewContent.Content = new MainMenu(this);
        Client = new(this);

        KeyDown += MainWindow_KeyDown;

        WindowStyle = WindowStyle.None;
        WindowState = WindowState.Maximized;
        FullScreen = true;
    }

    private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.F11)
        {
            if (FullScreen)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                FullScreen = false;
            }
            else
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
                FullScreen = true;
            }
        }
        else if (e.Key == System.Windows.Input.Key.Escape)
        {
            Environment.Exit(0);
        }

            e.Handled = true;
    }

    public void OpenMatchView()
    {
        ViewContent.Content = new MatchView(Client);
    }
}
