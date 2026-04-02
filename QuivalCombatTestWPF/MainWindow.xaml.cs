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

using QuivalLogicEngine;

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
        Queue<BoardCard> CurrentSpellStream = new Queue<BoardCard>();

        public MainWindow()
        {
            InitializeComponent();
            Client = new QuivalClient(this);
            Client.ConnectToServer();

            HandZone.CardClicked += HandZone_CardClicked;
            /*
            CombatZone.AddCard(1, 1, Side.Player);
            CombatZone.AddCard(1, 1, Side.Player);
            CombatZone.AddCard(1, 1, Side.Player);

            CombatZone.AddCard(2, 2, Side.Opponent);
            CombatZone.AddCard(2, 2, Side.Opponent);

            CombatZone.CardClicked += CombatZone_CardClicked;
            */
        }

        public void UpdateHand(List<BoardCard> hand)
        {
            HandZone.SetHand(hand);
        }

        private void HandZone_CardClicked(object? sender, EventArgs e)
        {
            if (sender is BoardCard card)
            {
                if (card.IsClickable())
                {
                    CurrentSpellStream.Enqueue(card);
                    card.SetClickable(false);
                }
            }
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

                    Client.SendString($"{FirstClickedCard.Stats} attacks {SecondClickedCard.Stats}");

                    FirstClickedCard = null;
                    SecondClickedCard = null;
                }
            }
        }

        private void SpellStreamCastButton_Click(object sender, RoutedEventArgs e)
        {
            Message message = new();
            message.Type = MessageType.SpellStream;
            message.SpellStream = Mapper.Map(CurrentSpellStream); //TODO: make the dang mapper

            Client.SendMessage(message);
        }
    }
}