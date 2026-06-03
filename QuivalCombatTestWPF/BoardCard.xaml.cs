using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace QuivalCombatTestWPF
{
    public partial class BoardCard : UserControl
    {
        public required int Id { get; set; }

        private bool hasActed;
        public required bool HasActed 
        {
            get => hasActed;
            set
            {
                Debug.WriteLine($"Card {Id} has been set to {value}");
                hasActed = value;
            }
        }

        public required Side Side { get; set; }

        public static int BlankId = -1;

        public BoardCard()
        {
            InitializeComponent();
        }

        #region Animation
        public Task AnimateAttack(Visual visual, Visual opponentsBZ, Visual playersBZ)
        {
            RemoveHighlight();

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
            RemoveHighlight();

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

        public Task AnimateReturnFromBlockZone(Visual visual, Visual opponentsBZ, Visual playersBZ, Point endPoint)
        {
            RemoveHighlight();
            Panel.SetZIndex(this, 9999);

            Point start = (Side == Side.Opponent) ? opponentsBZ.TransformToVisual(visual).Transform(new Point(0, 0))
                : playersBZ.TransformToVisual(visual).Transform(new Point(0, 0));

            Point end = endPoint;

            double deltaX = end.X - start.X;
            double deltaY = end.Y - start.Y;

            TranslateTransform transform = new();
            RenderTransform = transform;

            DoubleAnimation xAnim = new() { To = deltaX, Duration = TimeSpan.FromSeconds(0.4)};
            xAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };
            DoubleAnimation yAnim = new() { To = deltaY, Duration = TimeSpan.FromSeconds(0.4)};
            yAnim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn };

            TaskCompletionSource tsc = new();
            yAnim.Completed += (_, _) =>
            {
                tsc.SetResult();
            };

            transform.BeginAnimation(TranslateTransform.XProperty, xAnim);
            transform.BeginAnimation(TranslateTransform.YProperty, yAnim);

            return tsc.Task;
        }

        public Task AnimateSummon(Point end, Visual visual)
        {
            RemoveHighlight();

            Point start = TransformToVisual(visual).Transform(new Point(0, 0));

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

        public Task AnimateDeath()
        {
            RemoveHighlight();
            MarkRed();

            var tcs = new TaskCompletionSource();

            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
            };

            animation.Completed += (_, _) =>
            {
                MarkAsInvisible();
                tcs.SetResult();
            };

            BeginAnimation(UIElement.OpacityProperty, animation);

            return tcs.Task;
        }
        #endregion

        public void MarkAsActed()
        {
            Overlay.Background = Brushes.Black;
            Overlay.Opacity = 0.5;
        }

        public void MarkRed()
        {
            Overlay.Background = Brushes.Red;
            Overlay.Opacity = 0.6;
        }

        public void RemoveHighlight()
        {
            Overlay.Background = Brushes.Transparent;
            Overlay.Opacity = 0.0;
        }

        public void MarkAsInvisible()
        {
            Opacity = 0.0;
        }

        public static BoardCard GetBlankCard()
        {
            return new BoardCard()
            {
                Id = -1,
                HasActed = false,
                Side = Side.Player
            };
        }
    }
}
