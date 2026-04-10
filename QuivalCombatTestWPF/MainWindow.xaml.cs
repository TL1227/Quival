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
        Stack<BoardCard> CurrentSpellStream = new();

        public MainWindow()
        {
            InitializeComponent();
            Client = new QuivalClient(this);
            Client.ConnectToServer();

            HandZone.CardClicked += HandZone_CardClicked;
            SpellstreamZone.CardClicked += SpellStreamZone_CardClicked;
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
                if (card.IsClickable() && !SpellstreamZone.SpellStreamIsFull())
                {
                    AddToSpellStream(card);
                    card.SetClickable(false);
                }
            }
        }

        private void SpellStreamZone_CardClicked(object? sender, EventArgs e)
        {
            if (sender is BoardCard card)
            {
                RemoveFromSpellStream(card);
            }
        }

        private void AddToSpellStream(BoardCard card)
        {
            CurrentSpellStream.Push(card);
            SpellstreamZone.AddCard(card);
        }

        private void RemoveFromSpellStream(BoardCard card)
        {
            if (card.Tag == CurrentSpellStream.Peek())
            {
                CurrentSpellStream.Pop();

                if (card.Tag is BoardCard handCard)
                {
                    handCard.SetClickable(true);
                }

                SpellstreamZone.RemoveCard(card);
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
            message.Cards = Mapper.MapToList(CurrentSpellStream);

            Client.SendMessage(message);
        }
    }
}