using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace QuivalCombatTestWPF
{
    internal class Animator
    {
        public Canvas AnimationLayer { get; set; }

        public Animator(Canvas animationLayer)
        {
            AnimationLayer = animationLayer;
        }

        public Task AnimateReturnFromBlockZone(BoardCard card, Control newCard, Grid blockArea)
        {
            Point startScreen = card.PointToScreen(new Point(0, 0));
            Point endScreen = newCard.PointToScreen(new Point(0, 0));

            // Convert screen positions into AnimationLayer's coordinate space
            Point start = AnimationLayer.PointFromScreen(startScreen);
            Point end = AnimationLayer.PointFromScreen(endScreen);

            blockArea.Children.Remove(card);
            AnimationLayer.Children.Add(card);
            AnimationLayer.UpdateLayout();

            Canvas.SetLeft(card, start.X);
            Canvas.SetTop(card, start.Y);

            CubicEase ease = new CubicEase() { EasingMode = EasingMode.EaseIn };
            TimeSpan time = TimeSpan.FromSeconds(0.4);

            DoubleAnimation xAnim = new(start.X, end.X, time) { EasingFunction = ease };
            DoubleAnimation yAnim = new(start.Y, end.Y + 10, time) { EasingFunction = ease };

            TaskCompletionSource tsc = new();
            yAnim.Completed += (_, _) =>
            {
                AnimationLayer.Children.Remove(card);
                tsc.SetResult();
            };

            card.BeginAnimation(Canvas.LeftProperty, xAnim);
            card.BeginAnimation(Canvas.TopProperty, yAnim);
            return tsc.Task;
        }
    }
}
