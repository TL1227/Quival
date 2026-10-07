using QuivalCardDesigner.Views;
using System.Windows.Input;
using System.Windows;
using System.IO;

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
        Config config = new("Cards");
        CurrentView.Content = new CardDesignView(config);
    }

    public void EditCard()
    {
        //TODO: we will move this to some kind of card select screen first but for now we just want to load the most recent to work on loading cards!
        Config config = new("Cards");

        var cards = config.CardDirectory.GetFiles();
        string json = File.ReadAllText(cards[0].FullName);

        CurrentView.Content = new CardDesignView(json);
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