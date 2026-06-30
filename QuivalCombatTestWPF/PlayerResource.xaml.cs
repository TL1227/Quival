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
    public partial class PlayerResource : UserControl
    {
        public PlayerResource()
        {
            InitializeComponent();
        }

        public void TakeDamage(int dmg)
        {
            Flash(Brushes.Red, 0.2);
            int currentHealth = (int)HealthPoints.Content;
            HealthPoints.Content = currentHealth -= dmg;
        }
        public void HealDamage(int dmg)
        {
            Flash(Brushes.MediumAquamarine, 0.2);
            int currentHealth = (int)HealthPoints.Content;
            HealthPoints.Content = currentHealth += dmg;
        }

        public Task Flash(Brush Brush, double flashSpeed)
        {
            FlashOverlay.Background = Brush;

            DoubleAnimation anim = new()
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(flashSpeed),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
                AutoReverse = true,
                FillBehavior = FillBehavior.Stop
            };

            TaskCompletionSource tsc = new();
            anim.Completed += (_, _) =>
            {
                tsc.SetResult();
            };

            FlashOverlay.BeginAnimation(OpacityProperty, anim);

            return tsc.Task;
        }
    }
}
