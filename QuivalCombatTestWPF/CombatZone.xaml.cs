using QuivalCombatTestWPF.Colours;
using QuivalLogicEngine.Cards;
using System.Diagnostics;
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
        public event EventHandler CardClicked;
        public event EventHandler PlayerZoneClicked;

        private Grid[] CombatZones { get; set; }

        public CombatZone()
        {
            InitializeComponent();
            PlayerCombatZone.MouseLeftButtonDown += HandleClick;
            CombatZones = [OpponentCombatZone, PlayerCombatZone];
        }


        public void ClearCombatZone()
        {
            foreach (var zone in CombatZones)
                zone.Children.Clear();
        }

        public void Highlight(bool highlight, Side side)
        {
            var theZone = CombatZones[(int)side];

            if (highlight)
            {
                theZone.Background = QuivalColour.HighlightColour;
                theZone.Opacity = 0.5;
            }
            else
            {
                theZone.Background = Brushes.Transparent;
                theZone.Opacity = 1;
            }
        }

        public void UpdateCombatZone(List<CreatureCard> cards, Side side)
        {
            var combatZone = CombatZones[(int)side];

            combatZone.Children.Clear();

            int i = 0;
            foreach (var card in cards)
            {
                var boardCard = Mapper.MapToBoardCard(card);
                boardCard.MouseLeftButtonDown += HandleClick;

                Grid.SetRow(boardCard, 0);
                Grid.SetColumn(boardCard, i++);
                combatZone.Children.Add(boardCard);
            }
        }

        public bool CardIsSummonedByPlayer(BoardCard bc, Side side)
        {
            foreach (BoardCard child in CombatZones[(int)side].Children)
                if (child == bc)
                    return true;

            return false;
        }

        public int GetNumberOfSummonedCards(Side side)
        {
            return CombatZones[(int)side].Children.Count;
        }

        public void ClearHighlightedCards()
        {
            foreach (var zone in CombatZones)
                foreach (BoardCard child in zone.Children)
                    child.Overlay.Opacity = 0.0;
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
