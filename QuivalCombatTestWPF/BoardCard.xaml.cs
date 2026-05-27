using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace QuivalCombatTestWPF
{
    public partial class BoardCard : UserControl
    {
        public required int CardId { get; set; }
        public required bool HasActed { get; set; }
        public required Side Side { get; set; }

        public BoardCard()
        {
            InitializeComponent();
        }

        public Task AnimateAttack(Visual visual, Visual opponentsBZ, Visual playersBZ)
        {
            Point start = TransformToVisual(visual).Transform(new Point(0, 0));

            Point end = (Side == Side.Player) ? opponentsBZ.TransformToVisual(visual).Transform(new Point(0, 0))
                : playersBZ.TransformToVisual(visual).Transform(new Point(0, 0));

            double deltaX = end.X - start.X;
            double deltaY = end.Y - start.Y;

            TranslateTransform transform = new();
            RenderTransform = transform;

            DoubleAnimation xAnim = new() { To = deltaX, Duration = TimeSpan.FromSeconds(0.4), AutoReverse = true };
            xAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            DoubleAnimation yAnim = new() { To = deltaY, Duration = TimeSpan.FromSeconds(0.4), AutoReverse = true};
            yAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };

            TaskCompletionSource tsc = new();
            yAnim.Completed += (_, _) => tsc.SetResult();

            transform.BeginAnimation(TranslateTransform.XProperty, xAnim);
            transform.BeginAnimation(TranslateTransform.YProperty, yAnim);

            return tsc.Task;
        }

        public Task AnimateMoveToBlockZone(Visual visual, Visual opponentsBZ, Visual playersBZ)
        {
            Point start = TransformToVisual(visual).Transform(new Point(0, 0));

            Point end = (Side == Side.Opponent) ? opponentsBZ.TransformToVisual(visual).Transform(new Point(0, 0))
                : playersBZ.TransformToVisual(visual).Transform(new Point(0, 0));

            double deltaX = end.X - start.X;
            double deltaY = end.Y - start.Y;

            TranslateTransform transform = new();
            RenderTransform = transform;

            DoubleAnimation xAnim = new() { To = deltaX, Duration = TimeSpan.FromSeconds(0.4)};
            xAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            DoubleAnimation yAnim = new() { To = deltaY, Duration = TimeSpan.FromSeconds(0.4)};
            yAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };

            TaskCompletionSource tsc = new();
            yAnim.Completed += (_, _) => tsc.SetResult();

            transform.BeginAnimation(TranslateTransform.XProperty, xAnim);
            transform.BeginAnimation(TranslateTransform.YProperty, yAnim);

            return tsc.Task;
        }



        public void MarkAsActed()
        {
            Overlay.Background = Brushes.Black;
            Overlay.Opacity = 0.5;
        }
    }
}
