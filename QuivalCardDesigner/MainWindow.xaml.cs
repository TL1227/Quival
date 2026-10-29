using QuivalCardDesigner.Views;
using System.Windows.Input;
using System.Windows;
using System.IO;

namespace QuivalCardDesigner;

public partial class MainWindow : Window
{
    private Config CurrentConfig { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        CurrentView.Content = new MainMenuView(this);

        KeyDown += MainWindow_KeyDown;

        //this will be moved to some kind of options menu
        CurrentConfig = new("Cards");
    }

    public void OpenCardDesignView()
    {
        CurrentView.Content = new CardDesignView(CurrentConfig);
    }

    public void EditCard()
    {
        var cards = CurrentConfig.CardDirectory.GetFiles();
        string json = File.ReadAllText(cards[0].FullName);

        CurrentView.Content = new CardDesignView(CurrentConfig, json);
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