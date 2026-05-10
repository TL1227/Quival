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

using QuivalLogicEngine.Cards;

namespace QuivalCombatTestWPF
{
    public partial class MainWindow : Window
    {
        QuivalClient Client;

        BoardCard? FirstClickedCard;
        BoardCard? SecondClickedCard;

        public MainWindow()
        {
            InitializeComponent();
            Client = new QuivalClient(this);
            Client.ConnectToServer();

            HandZone.CardClicked += HandZone_CardClicked;
        }

        public void UpdateHand(List<Card> hand)
        {
            //HandZone.SetHand(hand);
        }

        private void HandZone_CardClicked(object? sender, EventArgs e)
        {
            if (sender is BoardCard card)
            {
                if (card.IsClickable() && !SpellstreamZone.SpellStreamIsFull())
                {
                    AddToSpellStream(card);
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
            //Client.SubmitCard();
        }
    }
}
