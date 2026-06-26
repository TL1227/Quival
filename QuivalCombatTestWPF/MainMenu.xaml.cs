using QuivalLogicEngine.Messages;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace QuivalCombatTestWPF
{
    public partial class MainMenu : UserControl
    {
        MainWindow MainWindow { get; set; }

        public MainMenu(MainWindow window)
        {
            InitializeComponent();
            MainWindow = window;
            StartMatchButton.Click += StartMatchButton_Click;
            CreateRoomButton.Click += CreateRoomButton_Click;
        }

        private async void StartMatchButton_Click(object sender, RoutedEventArgs e)
        {
            StartMatchButton.IsEnabled = false;

            List<string> deckIds = new();

            string deckLocation = "..\\Decks\\Default.txt";
            string[] lines = File.ReadAllLines(deckLocation);

            foreach (var line in lines)
                deckIds.Add(line.Split('#')[0].Trim());

            JoinRoomRequest joinRoomRequest = new JoinRoomRequest();
            joinRoomRequest.JoinRandom = true;
            joinRoomRequest.CardIds = deckIds;

            await MainWindow.Client.SendMessageAsync(joinRoomRequest);
        }

        public void ProcessJoinRoomResponce(JoinRoomResponse response)
        {
            if (response.Success)
            {
                MainWindow.OpenMatchView();
            }
            else
            {
                DisplayMessage("Could not connect to ");
            }

            StartMatchButton.IsEnabled = true;
        }

        private void CreateRoomButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ViewContent.Content = new CreateRoomView();
        }

        public void DisplayMessage(string message)
        {
            MessageLabel.Content = message;

            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
            };

            MessageLabel.BeginAnimation(OpacityProperty, animation);
        }
    }
}
