using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using QuivalCombatTestWPF.Colours;

namespace QuivalCombatTestWPF
{

    public partial class HandCard : UserControl
    {
        public static double DefaultWidth { get; set; } = 170;
        public static double DefaultHeight { get; set; } = 220;
        public int Id { get; set; }

        public HandCard(int cardId)
        {
            InitializeComponent();
            Id = cardId;
            Width = DefaultWidth;
            Height = DefaultHeight;
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
