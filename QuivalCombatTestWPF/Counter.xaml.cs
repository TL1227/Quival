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
    /// Interaction logic for Counter.xaml
    /// </summary>
    public partial class Counter : UserControl
    {
        int[] Rounds = [1,2,3,4,5];
        RoundLabel[] Labels;

        public Counter()
        {
            InitializeComponent();

            Labels = [ Round1, Round2, Round3, Round4, Round5 ];

            for (int i = 0; i < Labels.Length; i++)
            {
                Labels[i].RoundNumberLabel.Content = i + 1;
            }
        }

        public void HiglightRound(int round)
        {
            foreach (var lab in Labels)
            {
                lab.UnHighlightRound();
            }

            Labels[round - 1].HighlightRound();
        }
    }
}
