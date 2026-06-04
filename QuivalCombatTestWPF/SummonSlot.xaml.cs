using System.Windows;
using System.Windows.Controls;

namespace QuivalCombatTestWPF
{
    public partial class SummonSlot : UserControl
    {
        public event EventHandler SlotCardClickedOn;

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
            bc.MouseLeftButtonDown += BoardCard_MouseLeftButtonDown;
            SlotGrid.Children.Clear();
            SlotGrid.Children.Add(bc);
        }

        private void BoardCard_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SlotCardClickedOn?.Invoke(sender, new EventArgs());
        }
    }
}
