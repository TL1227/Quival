using QuivalLogicEngine.Cards;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuivalCombatTestWPF
{
    public enum Side
    {
        Opponent,
        Player
    }

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

        public void ClearCombatZone()
        {
            CombatGrid.Children.Clear();
        }

        public void UpdatePlayerCombatZone(List<CreatureCard> cards)
        {
            PlayerCardCount = 0;

            foreach (var card in cards)
            {
                BoardCard boardCard = new() { CardId = card.Id, };
                boardCard.AttackLabel.Content = card.Attack;
                boardCard.HealthLabel.Content = card.Health;
                boardCard.CardNameLabel.Content = card.Name;

                boardCard.MouseLeftButtonDown += HandleClick;

                Grid.SetRow(boardCard, (int)Side.Player);
                Grid.SetColumn(boardCard, PlayerCardCount++);
                CombatGrid.Children.Add(boardCard);
            }
        }

        public void UpdateOpponentCombatZone(List<CreatureCard> cards)
        {
            OpponentCardCount = 0;

            foreach (var card in cards)
            {
                BoardCard boardCard = new() { CardId = card.Id, };
                boardCard.AttackLabel.Content = card.Attack;
                boardCard.HealthLabel.Content = card.Health;
                boardCard.CardNameLabel.Content = card.Name;

                boardCard.MouseLeftButtonDown += HandleClick;

                Grid.SetRow(boardCard, (int)Side.Opponent);
                Grid.SetColumn(boardCard, OpponentCardCount++);
                CombatGrid.Children.Add(boardCard);
            }
        }

        public void DeselectAllCards()
        {
            foreach (var card in CombatGrid.Children)
            {
                if (card is BoardCard hc)
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
