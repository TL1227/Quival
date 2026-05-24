using System.Windows.Controls;
using System.Windows.Media;

namespace QuivalCombatTestWPF
{
    public partial class BoardCard : UserControl
    {
        public required int CardId { get; set; }
        public required bool HasActed { get; set; }

        public BoardCard()
        {
            InitializeComponent();
        }

        public void MarkAsActed()
        {
            Overlay.Background = Brushes.Black;
            Overlay.Opacity = 0.5;
        }
    }
}
