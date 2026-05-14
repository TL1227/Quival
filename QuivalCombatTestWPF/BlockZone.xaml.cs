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
    /// Interaction logic for BlockZone.xaml
    /// </summary>
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
            boardCard.CardNameLabel.Content = card.CardNameLabel.Content;
            boardCard.AttackLabel.Content = card.AttackLabel.Content;
            boardCard.HealthLabel.Content = card.HealthLabel.Content;

            BlockArea.Children.Clear();
            BlockArea.Children.Add(boardCard);
        }

        public void HandleClick(object obj, MouseButtonEventArgs args)
        {
            ZoneClicked?.Invoke(obj, args);
        }
    }
}
