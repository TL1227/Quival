using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuivalCombatTestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        QuivalClient Client;

        BoardCard? FirstClickedCard;
        BoardCard? SecondClickedCard;

        public MainWindow()
        {
            InitializeComponent();
            Client = new QuivalClient();
            Client.ConnectToServer();

            CombatZone.AddCard(1, 1, Side.Player);
            CombatZone.AddCard(1, 1, Side.Player);
            CombatZone.AddCard(1, 1, Side.Player);

            CombatZone.AddCard(2, 2, Side.Opponent);
            CombatZone.AddCard(2, 2, Side.Opponent);

            CombatZone.CardClicked += CombatZone_CardClicked;
        }

        private void CombatZone_CardClicked(object? sender, EventArgs e)
        {
            if (sender is BoardCard card)
            {
                if (FirstClickedCard == null)
                {
                    FirstClickedCard = card;
                }
                else
                {
                    SecondClickedCard = card;

                    Client.WriteMessage($"{FirstClickedCard.Stats} attacks {SecondClickedCard.Stats}");

                    FirstClickedCard = null;
                    SecondClickedCard = null;
                }
            }
        }
    }
}