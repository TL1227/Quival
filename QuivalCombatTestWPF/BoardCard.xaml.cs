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
    /// Interaction logic for BoardCard.xaml
    /// </summary>
    public partial class BoardCard : UserControl
    {
        public int Attack { get; set; }
        public int Defence { get; set; }
        private bool Clickable { get; set; }

        public BoardCard(int attack, int defence)
        {
            InitializeComponent();
            Attack = attack;
            Defence = defence;
            Stats.Content = $"{Attack}/{Defence}";
            Clickable = true;
        }

        public void SetClickable(bool value)
        {
            Clickable = value;
            Opacity = 0.5;
        }

        public bool IsClickable()
        {
            return Clickable;
        }
    }
}
