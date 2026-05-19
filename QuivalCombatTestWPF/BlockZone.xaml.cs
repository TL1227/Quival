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
    public partial class BlockZone : UserControl
    {
        public event EventHandler ZoneClicked;

        public BlockZone()
        {
            InitializeComponent();
            BlockArea.MouseLeftButtonDown += HandleClick;
        }

        public void AddCardToBlockZone(BoardCard card)
        {
            BoardCard boardCard = new() { CardId = card.CardId };
            boardCard.CardBackground.Background = card.CardBackground.Background;
            boardCard.CardNameLabel.Content = card.CardNameLabel.Content;
            boardCard.AttackLabel.Content = card.AttackLabel.Content;
            boardCard.HealthLabel.Content = card.HealthLabel.Content;

            BlockArea.Children.Clear();
            BlockArea.Children.Add(boardCard);
        }

        public void SetHighlighted(bool value)
        {
            if (value)
            {
                BlockArea.Background = Brushes.MediumAquamarine;
                BlockArea.Opacity = 0.5;
            }
            else
            {
                BlockArea.Background = Brushes.Transparent;
                BlockArea.Opacity = 1.0;
            }
        }

        public void HandleClick(object obj, MouseButtonEventArgs args)
        {
            ZoneClicked?.Invoke(obj, args);
        }
    }
}
