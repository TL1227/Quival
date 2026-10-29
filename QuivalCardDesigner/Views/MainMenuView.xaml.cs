using System.Windows;
using System.Windows.Controls;

namespace QuivalCardDesigner.Views;

public partial class MainMenuView : UserControl
{
    private MainWindow MainWindow { get; set; }

    public MainMenuView(MainWindow mainWindow)
    {
        InitializeComponent();
        MainWindow = mainWindow;
        NewCardButton.Click += NewCardButton_Click;
        EditCardButton.Click += EditCardButton_Click;
    }

    private void EditCardButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.EditCard();
    }

    private void NewCardButton_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.OpenCardDesignView();
    }
}
