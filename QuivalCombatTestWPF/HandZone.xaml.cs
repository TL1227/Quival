using QuivalLogicEngine;
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
    /// Interaction logic for HandZone.xaml
    /// </summary>
    public partial class HandZone : UserControl
    {
        public event EventHandler CardClicked;

        public HandZone()
        {
            InitializeComponent();
        }

        public void SetHand(List<HandCard> hand)
        {
            HandGrid.Children.Clear();

            for (int i = 0; i < hand.Count; i++)
            {
                Grid.SetColumn(hand[i], i);
                hand[i].MouseLeftButtonDown += HandleClick;
                HandGrid.Children.Add(hand[i]);
            }
        }

        public void DeselectAllCards()
        {
            foreach (var card in HandGrid.Children)
            {
                if (card is HandCard hc)
                {
                    hc.Overlay.Opacity = 0.0;
                }
            }
        }

        public void HandleClick(object boardCard, MouseButtonEventArgs args)
        {
            CardClicked?.Invoke(boardCard, args);
        }
    }
}
