using QuivalCombatTestWPF.Colours;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuivalCombatTestWPF
{
    public partial class BlockZone : UserControl
    {
        public event EventHandler ZoneClicked;

        public BoardCard? CurrentCard;

        public Side Side;

        public BlockZone()
        {
            InitializeComponent();
            BlockArea.MouseLeftButtonDown += HandleClick;
        }

        public void AddCardToBlockZone(BoardCard card, LayoutCanvas layout, Position blockArea)
        {
            card.SetPos(blockArea);
            layout.Canvas.Children.Add(card);
            CurrentCard = card;
        }

        public void RemoveCardFromBlockZone(LayoutCanvas layout)
        {
            layout.Canvas.Children.Remove(CurrentCard);
            CurrentCard = null;
        }

        public BoardCard? GetCardFromBlockZone()
        {
            return CurrentCard;
        }

        public void SetHighlighted(bool value)
        {
            if (value)
            {
                BlockArea.Background = QuivalColour.HighlightColour;
                BlockArea.Opacity = 1.0;
            }
            else
            {
                BlockArea.Background = Brushes.Transparent;
                BlockArea.Opacity = 1.0;
            }
        }

        public void HandleClick(object obj, MouseButtonEventArgs args)
        {
            if (obj is BoardCard bc && bc.HasActed)
            {
                return;
            }
            else
            {
                ZoneClicked?.Invoke(obj, args);
            }
        }
    }
}
