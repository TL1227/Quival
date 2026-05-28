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
        Label[] Labels;

        public Counter()
        {
            InitializeComponent();

            Labels =
            [
                Round1.RoundNumberLabel,
                Round2.RoundNumberLabel,
                Round3.RoundNumberLabel,
                Round4.RoundNumberLabel,
                Round5.RoundNumberLabel
            ];

            for (int i = 0; i < Labels.Length; i++)
            {
                Labels[i].Content = i + 1;
            }
        }
    }
}
