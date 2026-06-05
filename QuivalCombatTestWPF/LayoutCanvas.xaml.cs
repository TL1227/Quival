using QuivalLogicEngine.Cards;
using System.Windows.Controls;

namespace QuivalCombatTestWPF
{
    public partial class LayoutCanvas : UserControl
    {
        public double CenterWidth { get; set; }
        public double SummonSlotsWidth { get; set; }
        public double SummonSlotsCenter { get; set; }
        public double SummonSlotsStartLeft { get; set; }
        public double SummonSlotPadding { get; set; }
        public Position[] PlayerSummonSlots { get; set; }
        public Position[] OpponentSummonSlots { get; set; }
        public Position[] HandSlots { get; set; }

        public LayoutCanvas()
        {
            InitializeComponent();
            Loaded += LayoutCanvas_Loaded;
        }

        private void LayoutCanvas_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            //card slots layout
            SummonSlotPadding = 100;
            CenterWidth = Canvas.ActualWidth / 2;
            SummonSlotsWidth = (BoardCard.DefaultWidth * 5)  + (SummonSlotPadding * 4); //NOTE it's 4 paddings because there are 4 gaps between the 5 cards
            SummonSlotsCenter = SummonSlotsWidth / 2;
            SummonSlotsStartLeft = CenterWidth - SummonSlotsCenter;

            double CenterHeight = Canvas.ActualHeight / 2;
            OpponentSummonSlots = new Position[5];
            for (int i = 0; i < 5; i++)
            {
                OpponentSummonSlots[i] = new();
                OpponentSummonSlots[i].Left = SummonSlotsStartLeft + ((BoardCard.DefaultWidth + SummonSlotPadding) * i);
                OpponentSummonSlots[i].Top = CenterHeight - BoardCard.DefaultHeight - 90;
            }

            PlayerSummonSlots = new Position[5];
            for (int i = 0; i < 5; i++)
            {
                PlayerSummonSlots[i] = new();
                PlayerSummonSlots[i].Left = SummonSlotsStartLeft + ((BoardCard.DefaultWidth + SummonSlotPadding) * i);
                PlayerSummonSlots[i].Top = CenterHeight - 60;
            }


            HandSlots = new Position[7];
            for (int i = 0; i < 7; i++)
            {
                HandSlots[i] = new();
                HandSlots[i].Left = SummonSlotsStartLeft + ((BoardCard.DefaultWidth + 20) * i);
                HandSlots[i].Top = Canvas.ActualHeight - HandCard.DefaultHeight - 10;
            }

            /*
            List<BoardCard> cards = new();
            foreach (var slot in summonSlots)
            {
                BoardCard bc = new() { HasActed = false, Id = -1 };
                AnimationCanvas.Children.Add(bc);
                Canvas.SetTop(bc, 500);
                Canvas.SetLeft(bc, centerScreen - 90);
                cards.Add(bc);
            }
            */

            /*
            for (int i = 0; i < cards.Count; i++)
            {
                long animationSpeed = 2;
                DoubleAnimation yAnim = new()
                {
                    From = Canvas.GetTop(cards[i]),
                    To = summonSlots[i].Top,
                    Duration = TimeSpan.FromSeconds(animationSpeed),
                    AutoReverse = true
                };

                yAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };

                yAnim.Completed += (_, _) =>
                {
                };

                DoubleAnimation xAnim = new()
                {
                    From = Canvas.GetLeft(cards[i]),
                    To = summonSlots[i].Left,
                    Duration = TimeSpan.FromSeconds(animationSpeed),
                    AutoReverse = true
                };

                xAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };

                xAnim.Completed += (_, _) =>
                {
                };

                cards[i].BeginAnimation(Canvas.LeftProperty, xAnim);
                cards[i].BeginAnimation(Canvas.TopProperty, yAnim);
            }
            */
        }
    }
}
