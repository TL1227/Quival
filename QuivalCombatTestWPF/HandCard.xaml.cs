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
    /// Interaction logic for HandCard.xaml
    /// </summary>
    public partial class HandCard : UserControl
    {
        public int CardId { get; set; }
        public HandCard(int cardId)
        {
            InitializeComponent();
            CardId = cardId;
        }
    }
}
