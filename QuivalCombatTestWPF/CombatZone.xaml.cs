using QuivalLogicEngine.Cards;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuivalCombatTestWPF
{
    public enum Side
    {
        Opponent,
        Player
    }

    public partial class CombatZone : UserControl
    {
        public int PlayerCardCount { get; set; } = 0;
        public int OpponentCardCount { get; set; } = 0;
        public int MaxCards { get; set; } = 5;

        public event EventHandler CardClicked;
        public event EventHandler PlayerZoneClicked;

        public CombatZone()
        {
            InitializeComponent();
            PlayerCombatZone.MouseLeftButtonDown += HandleClick;
        }


        public void ClearCombatZone()
        {
            CombatGrid.Children.Clear();
        }

        public void Highlight(bool value, Side side)
        {
            if (value)
            {
                if (side == Side.Player)
                {
                    PlayerCombatZone.Background = Brushes.MediumAquamarine;
                    PlayerCombatZone.Opacity = 0.5;
                }
                else if (side == Side.Opponent)
                {
                    OpponentCombatZone.Background = Brushes.MediumAquamarine;
                    OpponentCombatZone.Opacity = 0.5;
                }
            }
            else
            {
                if (side == Side.Player)
                {
                    PlayerCombatZone.Background = Brushes.Transparent;
                }
                else if (side == Side.Opponent)
                {
                    OpponentCombatZone.Background = Brushes.Transparent;
                }
            }
        }

        public void UpdatePlayerCombatZone(List<CreatureCard> cards)
        {
            PlayerCardCount = 0;

            foreach (var card in cards)
            {
                var boardCard = Mapper.MapToBoardCard(card);
                boardCard.MouseLeftButtonDown += HandleClick;

                Grid.SetRow(boardCard, (int)Side.Player);
                Grid.SetColumn(boardCard, PlayerCardCount++);
                CombatGrid.Children.Add(boardCard);
            }
        }

        public void UpdateOpponentCombatZone(List<CreatureCard> cards)
        {
            OpponentCardCount = 0;

            foreach (var card in cards)
            {
                var boardCard = Mapper.MapToBoardCard(card);
                boardCard.MouseLeftButtonDown += HandleClick;

                Grid.SetRow(boardCard, (int)Side.Opponent);
                Grid.SetColumn(boardCard, OpponentCardCount++);
                CombatGrid.Children.Add(boardCard);
            }
        }

        public void DeselectAllCards()
        {
            foreach (var card in CombatGrid.Children)
            {
                if (card is BoardCard hc)
                {
                    hc.Overlay.Opacity = 0.0;
                }
            }
        }

        private void HandleClick(object obj, MouseButtonEventArgs args)
        {
            switch (obj)
            {
                case BoardCard:
                    CardClicked?.Invoke(obj, args);
                    break;
                default:
                    PlayerZoneClicked?.Invoke(obj, args);
                    break;
            }
        }
    }
}
