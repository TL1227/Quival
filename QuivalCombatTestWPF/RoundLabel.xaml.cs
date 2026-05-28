using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuivalCombatTestWPF
{
    /// <summary>
    /// Interaction logic for RoundLabel.xaml
    /// </summary>
    public partial class RoundLabel : UserControl
    {
        public RoundLabel()
        {
            InitializeComponent();
        }

        public void HighlightRound()
        {
            RoundBorder.Background = Brushes.Bisque;
            RoundNumberLabel.Foreground = Brushes.Blue;
        }

        public void UnHighlightRound()
        {
            RoundBorder.Background = Brushes.CadetBlue;
            RoundNumberLabel.Foreground = Brushes.Black;
        }
    }
}
