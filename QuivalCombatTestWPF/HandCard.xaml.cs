using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace QuivalCombatTestWPF
{
    public partial class HandCard : UserControl
    {
        public int CardId { get; set; }
        public HandCard(int cardId)
        {
            InitializeComponent();
            CardId = cardId;
        }


        public void RemoveHighlight()
        {
            Overlay.Background = Brushes.Transparent;
            Overlay.Opacity = 0.0;
        }
    }
}
