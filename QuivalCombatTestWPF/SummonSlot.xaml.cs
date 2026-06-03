using System.Windows;
using System.Windows.Controls;

namespace QuivalCombatTestWPF
{
    public partial class SummonSlot : UserControl
    {
        public BoardCard? Card
        {
            get
            {
                if (SlotGrid.Children.Count > 0 && SlotGrid.Children[0] is BoardCard bc)
                    return bc;
                else
                    return null;
            }
        }

        public SummonSlot()
        {
            InitializeComponent();
        }

        public bool IsEmpty() => SlotGrid.Children.Count <= 0;

        public void ClearCard()
        {
            SlotGrid.Children.Clear();
        }

        public void SetCard(BoardCard bc)
        {
            SlotGrid.Children.Clear();
            SlotGrid.Children.Add(bc);
        }
    }
}
