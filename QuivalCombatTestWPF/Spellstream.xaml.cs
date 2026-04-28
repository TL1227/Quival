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
    /// Interaction logic for Spellstream.xaml
    /// </summary>
    public partial class Spellstream : UserControl
    {
        public int SlotMax { get; set; }

        public event EventHandler CardClicked;

        public Spellstream()
        {
            SlotMax = 5;
            InitializeComponent();
        }

        public void AddCard(BoardCard card)
        {
            BoardCard streamCard = new(card.CardId, card.Attack, card.Defence);
            streamCard.Tag = card;

            streamCard.MouseLeftButtonDown += HandleClick;

            if (SpellStreamGrid.Children.Count < SlotMax)
            {
                Grid.SetColumn(streamCard, SpellStreamGrid.Children.Count + 1);
                SpellStreamGrid.Children.Add(streamCard);
            }
        }

        public void RemoveCard(BoardCard card)
        {
            SpellStreamGrid.Children.Remove(card);
        }

        public bool SpellStreamIsFull()
        {
            return SpellStreamGrid.Children.Count >= SlotMax;
        }

        public void HandleClick(object boardCard, MouseButtonEventArgs args)
        {
            CardClicked?.Invoke(boardCard, args);
        }
    }
}
