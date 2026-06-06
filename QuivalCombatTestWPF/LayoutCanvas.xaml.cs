using QuivalLogicEngine.Cards;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace QuivalCombatTestWPF
{
    public partial class LayoutCanvas : UserControl
    {
        public double CenterWidth { get; set; }
        public double SummonSlotsWidth { get; set; }
        public double SummonSlotsCenter { get; set; }
        public double SummonSlotsStartLeft { get; set; }
        public double SummonSlotPadding { get; set; }
        public SummonSlot[] PlayerSummonSlots { get; set; }
        public SummonSlot[] OpponentSummonSlots { get; set; }
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
            SummonSlotsWidth = (BoardCard.DefaultWidth * 5) + (SummonSlotPadding * 4); //NOTE it's 4 paddings because there are 4 gaps between the 5 cards
            SummonSlotsCenter = SummonSlotsWidth / 2;
            SummonSlotsStartLeft = CenterWidth - SummonSlotsCenter;

            double CenterHeight = Canvas.ActualHeight / 2;
            OpponentSummonSlots = new SummonSlot[5];
            for (int i = 0; i < 5; i++)
            {
                OpponentSummonSlots[i] = new(Canvas);
                OpponentSummonSlots[i].Position.Left = SummonSlotsStartLeft + ((BoardCard.DefaultWidth + SummonSlotPadding) * i);
                OpponentSummonSlots[i].Position.Top = CenterHeight - BoardCard.DefaultHeight - 90;
            }

            PlayerSummonSlots = new SummonSlot[5];
            for (int i = 0; i < 5; i++)
            {
                PlayerSummonSlots[i] = new(Canvas);
                PlayerSummonSlots[i].Position.Left = SummonSlotsStartLeft + ((BoardCard.DefaultWidth + SummonSlotPadding) * i);
                PlayerSummonSlots[i].Position.Top = CenterHeight - 60;
            }


            HandSlots = new Position[7];
            for (int i = 0; i < 7; i++)
            {
                HandSlots[i] = new();
                HandSlots[i].Left = SummonSlotsStartLeft + ((BoardCard.DefaultWidth + 20) * i);
                HandSlots[i].Top = Canvas.ActualHeight - HandCard.DefaultHeight - 10;
            }
        }

        public Task MoveToPoint(BoardCard boardCard, Position start, Position end)
        {
            double animationSpeed = 0.6;

            DoubleAnimation yAnim = new()
            {
                From = start.Top,
                To = end.Top,
                Duration = TimeSpan.FromSeconds(animationSpeed),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
            };

            DoubleAnimation xAnim = new()
            {
                From = start.Left,
                To = end.Left,
                Duration = TimeSpan.FromSeconds(animationSpeed),
                EasingFunction = new BackEase() { Amplitude = 0.05, EasingMode = EasingMode.EaseIn },
            };

            TaskCompletionSource tsc = new();
            xAnim.Completed += (_, _) =>
            {
                boardCard.SetPos(end);
                tsc.SetResult();
            };

            boardCard.BeginAnimation(Canvas.LeftProperty, xAnim);
            boardCard.BeginAnimation(Canvas.TopProperty, yAnim);

            return tsc.Task;
        }

        public void ClearHand()
        {
            List<HandCard> handCards = new();
            foreach (var card in Canvas.Children)
            {
                if (card is HandCard hc)
                {
                    handCards.Add(hc);
                }
            }

            foreach (var card in handCards)
            {
                Canvas.Children.Remove(card);
            }
        }
    }

}
