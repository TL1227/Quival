using System.Windows.Controls;

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
    }
}
