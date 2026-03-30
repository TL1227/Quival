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
    public partial class CombatZone : UserControl
    {
        public int PlayerCardCount { get; set; } = 0;
        public int OpponentCardCount { get; set; } = 0;
        public int MaxCards { get; set; } = 5;

        public event EventHandler CardClicked;

        public CombatZone()
        {
            InitializeComponent();
        }

        public void AddCard(int power, int toughness, Side side)
        {
            BoardCard card = new();
            card.Stats.Content = $"{power}/{toughness}";
            card.MouseLeftButtonDown += HandleClick;

            if (side == Side.Player)
            {
                if (PlayerCardCount < MaxCards)
                {
                    Grid.SetRow(card, 1);
                    Grid.SetColumn(card, PlayerCardCount++);
                    CombatGrid.Children.Add(card);
                }
            }
            else
            {
                if (OpponentCardCount < MaxCards)
                {
                    Grid.SetRow(card, 0);
                    Grid.SetColumn(card, OpponentCardCount++);
                    CombatGrid.Children.Add(card);
                }
            }
        }

        public void HandleClick(object boardCard, MouseButtonEventArgs args)
        {
            CardClicked?.Invoke(boardCard, args);
        }
    }
}
