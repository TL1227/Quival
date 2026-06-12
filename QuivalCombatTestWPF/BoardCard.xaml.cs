using QuivalCombatTestWPF.Colours;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace QuivalCombatTestWPF
{
    public partial class BoardCard : UserControl
    {
        public required int Id { get; set ; }

        public required bool HasActed {  get; set ; }

        public static int BlankId = -1;

        public static double DefaultWidth { get; set; } = 160;
        public static double DefaultHeight { get; set; } = 110;

        public BoardCard()
        {
            InitializeComponent();

            Width = DefaultWidth;
            Height = DefaultHeight;
            DebugId.Content = Id;
        }

        public void MarkAsActed(bool mark)
        {
            if (mark)
            {
                Overlay.Background = Brushes.Black;
                Overlay.Opacity = 0.5;
            }
            else
            {
                Overlay.Opacity = 0.0;
            }
        }

        public void MarkRed()
        {
            Overlay.Background = Brushes.Red;
            Overlay.Opacity = 0.6;
        }

        public void MarkSelected(bool mark)
        {
            if (mark)
            {
                SelectedOverlay.Background = Brushes.MediumPurple;
                SelectedOverlay.Opacity = 0.6;
            }
            else
            {
                SelectedOverlay.Opacity = 0.0;
            }
        }

        public void RemoveHighlight()
        {
            Border.Background = Brushes.Transparent;
            ShadowEffect.Opacity = 0.0;
        }

        public void Highlight()
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

        public Position GetPos()
        {
            return new()
            {
                Top = Canvas.GetTop(this),
                Left = Canvas.GetLeft(this)
            };
        }

        public int GetAttackFromLabel()
        {
            return (int)AttackLabel.Content;
        }
        public int GetCurrentHealthFromLabel()
        {
            return (int)HealthLabel.Content;
        }

        public Task FlashUp(Brush Brush)
        {
            double animationSpeed = 0.2;
            Overlay.Background = Brush;

            DoubleAnimation anim = new()
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(animationSpeed),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
                FillBehavior = FillBehavior.Stop
            };

            TaskCompletionSource tsc = new();
            anim.Completed += (_, _) =>
            {
                tsc.SetResult();
            };

            Overlay.BeginAnimation(OpacityProperty, anim);

            return tsc.Task;
        }

        public Task FlashDown(Brush Brush)
        {
            double animationSpeed = 0.2;
            Overlay.Background = Brush;

            DoubleAnimation anim = new()
            {
                To = 0.0,
                From = 1.0,
                Duration = TimeSpan.FromSeconds(animationSpeed),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
                FillBehavior = FillBehavior.Stop
            };

            TaskCompletionSource tsc = new();
            anim.Completed += (_, _) =>
            {
                tsc.SetResult();
            };

            Overlay.BeginAnimation(OpacityProperty, anim);

            return tsc.Task;
        }
    }
}
