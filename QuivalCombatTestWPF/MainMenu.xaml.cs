using System.Windows;
using System.Windows.Controls;

namespace QuivalCombatTestWPF
{
    public partial class MainMenu : UserControl
    {
        MainWindow Window { get; set; }

        public MainMenu(MainWindow window)
        {
            InitializeComponent();
            Window = window;
            StartMatchButton.Click += StartMatchButton_Click;
            CreateRoomButton.Click += CreateRoomButton_Click;
        }

        private void StartMatchButton_Click(object sender, RoutedEventArgs e)
        {
            Window.OpenMatchView();
        }

        private void CreateRoomButton_Click(object sender, RoutedEventArgs e)
        {
            Window.ViewContent.Content = new CreateRoomView();
        }
    }
}
