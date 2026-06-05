using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace QuivalCombatTestWPF
{

    public partial class HandCard : UserControl
    {
        public static double DefaultWidth { get; set; } = 200;
        public static double DefaultHeight { get; set; } = 250;
        public int Id { get; set; }

        public HandCard(int cardId)
        {
            InitializeComponent();
            Id = cardId;
            Width = DefaultWidth;
            Height = DefaultHeight;
        }


        public void RemoveHighlight()
        {
            Overlay.Background = Brushes.Transparent;
            Overlay.Opacity = 0.0;
        }

        public void SetPos(Position p)
        {
            Canvas.SetTop(this, p.Top);
            Canvas.SetLeft(this, p.Left);
        }
    }
}
