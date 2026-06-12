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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuivalCombatTestWPF
{
    public partial class FullCard : UserControl
    {
        public FullCard()
        {
            InitializeComponent();
        }

        public Position GetPos()
        {
            return new()
            {
                Top = Canvas.GetTop(this),
                Left = Canvas.GetLeft(this)
            };
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
