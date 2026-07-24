using QuivalCombatTestWPF.Colours;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace QuivalCombatTestWPF
{
    public partial class HandCard : UserControl
    {
        public static double DefaultWidth { get; set; } = 243;
        public static double DefaultHeight { get; set; } = 324;
        public int Id { get; set; }

        public HandCard(int cardId)
        {
            InitializeComponent();
            Id = cardId;
            Width = DefaultWidth;
            Height = DefaultHeight;
            MouseEnter += HandCard_MouseEnter;
            MouseLeave += HandCard_MouseLeave;
            RenderTransform = new TranslateTransform(0, 0);
        }

        private void HandCard_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            UpdateLayout();
            Canvas.SetZIndex(this, 10);
            Slide(-ActualHeight * 0.5);
        }

        private void HandCard_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Canvas.SetZIndex(this, 0);
            Slide(0);
        }

        public void RemoveHighlight()
        {
            Overlay.Background = Brushes.Transparent;
            Overlay.Opacity = 0.0;
        }
        public void Highlight()
        {
            Overlay.Background = QuivalColour.HighlightColour;
            Overlay.Opacity = 0.4;
        }

        public void SetPos(Position p)
        {
            Canvas.SetTop(this, p.Top);
            Canvas.SetLeft(this, p.Left);
        }

        public Task Slide(double targetHeight)
        {
            var transform = (TranslateTransform)RenderTransform;

            var animation = new DoubleAnimation
            {
                To = targetHeight,
                Duration = TimeSpan.FromSeconds(0.15),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            TaskCompletionSource tsc = new();
            animation.Completed += (_, _) =>
            {
                Overlay.Opacity = 0.0;
                tsc.SetResult();
            };

            transform.BeginAnimation(TranslateTransform.YProperty, animation);

            return tsc.Task;
        }

        public Task SummonIn(Brush Brush)
        {
            double animationSpeed = 0.4;
            Overlay.Background = Brush;
            Overlay.Opacity = 1.0;

            DoubleAnimation anim = new()
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(animationSpeed),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
                FillBehavior = FillBehavior.Stop
            };

            TaskCompletionSource tsc = new();
            anim.Completed += (_, _) =>
            {
                Overlay.Opacity = 0.0;
                tsc.SetResult();
            };

            Overlay.BeginAnimation(OpacityProperty, anim);

            return tsc.Task;
        }
    }
}
