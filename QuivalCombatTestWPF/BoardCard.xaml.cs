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

        public required bool HasActed { get; set; }

        public static int BlankId = -1;

        public static double DefaultWidth { get; set; } = 180;
        public static double DefaultHeight { get; set; } = 130;

        public BoardCard()
        {
            InitializeComponent();

            Width = DefaultWidth;
            Height = DefaultHeight;
        }

        #region Animation
        public Task AnimateAttack(Visual visual, Visual destination)
        {
            RemoveHighlight();

            Point start = TransformToVisual(visual).Transform(new Point(0, 0));
            Point end = destination.TransformToVisual(visual).Transform(new Point(0, 0));

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

        public Task AnimateMoveToBlockZone(Visual visual, Visual destination)
        {
            RemoveHighlight();

            Point start = TransformToVisual(visual).Transform(new Point(0, 0));
            Point end = destination.TransformToVisual(visual).Transform(new Point(0, 0));

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

        public Task AnimateReturnFromBlockZone(Visual visual, Visual blockArea, Point endPoint)
        {
            RemoveHighlight();

            Panel.SetZIndex(this, 9999); //TODO: test if this is needed

            Point start = blockArea.TransformToVisual(visual).Transform(new Point(0, 0));
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

            Panel.SetZIndex(this, 9999); //TODO: test if this is needed

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
            yAnim.Completed += (_, _) =>
            {
                tsc.SetResult();

                Point finalPos = TransformToVisual(visual)
                        .Transform(new Point(0, 0));
            };

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
            };
        }

        public void SetPos(Position p)
        {
            Canvas.SetTop(this, p.Top);
            Canvas.SetLeft(this, p.Left);
        }
    }
}
