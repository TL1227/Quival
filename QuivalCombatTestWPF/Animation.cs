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
    internal static class Animation
    {
        public static Task MoveToPointAndBack(BoardCard boardCard, Position start, Position end)
        {
            double animationSpeed = 0.4;

            DoubleAnimation yAnim = new()
            {
                From = start.Top,
                To = end.Top,
                Duration = TimeSpan.FromSeconds(animationSpeed),
                AutoReverse = true,
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
            };

            DoubleAnimation xAnim = new()
            {
                From = start.Left,
                To = end.Left,
                Duration = TimeSpan.FromSeconds(animationSpeed),
                AutoReverse = true,
                EasingFunction = new BackEase() { Amplitude = 0.05, EasingMode = EasingMode.EaseIn },
            };

            TaskCompletionSource tsc = new();
            xAnim.Completed += (_, _) =>
            {
                tsc.SetResult();
            };

            boardCard.BeginAnimation(Canvas.LeftProperty, xAnim);
            boardCard.BeginAnimation(Canvas.TopProperty, yAnim);

            return tsc.Task;
        }

        public static Task MoveToPoint(BoardCard boardCard, Position start, Position end, EasingMode easingMode = EasingMode.EaseIn)
        {
            double animationSpeed = 0.6;

            DoubleAnimation yAnim = new()
            {
                From = start.Top,
                To = end.Top,
                Duration = TimeSpan.FromSeconds(animationSpeed),
                EasingFunction = new CubicEase() { EasingMode = easingMode }
            };

            DoubleAnimation xAnim = new()
            {
                From = start.Left,
                To = end.Left,
                Duration = TimeSpan.FromSeconds(animationSpeed),
                EasingFunction = new BackEase() { Amplitude = 0.05, EasingMode = easingMode }
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

        public static Task AnimateDeath(BoardCard card)
        {
            card.RemoveHighlight();
            card.MarkRed();

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
                card.MarkAsInvisible();
                tcs.SetResult();
            };

            card.BeginAnimation(UIElement.OpacityProperty, animation);

            return tcs.Task;
        }

        public static Task DirectEffect(double xLocation, double yLocation, Canvas canvas, Brush effectColor, EasingMode easingMode = EasingMode.EaseIn)
        {
            Label label = new();
            label.Height = 50;
            label.Width = 50;
            label.Background = effectColor;
            Canvas.SetTop(label, 540);
            Canvas.SetLeft(label, 810);
            canvas.Children.Add(label);
            canvas.UpdateLayout();

            double animationSpeed = 0.5;

            DoubleAnimation yAnim = new()
            {
                From = Canvas.GetTop(label),
                To = yLocation,
                Duration = TimeSpan.FromSeconds(animationSpeed),
                EasingFunction = new CubicEase() { EasingMode = easingMode }
            };

            DoubleAnimation xAnim = new()
            {
                From = Canvas.GetLeft(label),
                To = xLocation,
                Duration = TimeSpan.FromSeconds(animationSpeed),
                EasingFunction = new BackEase() { Amplitude = 0.05, EasingMode = easingMode }
            };

            TaskCompletionSource tsc = new();
            xAnim.Completed += (_, _) =>
            {
                canvas.Children.Remove(label);
                tsc.SetResult();
            };

            label.BeginAnimation(Canvas.LeftProperty, xAnim);
            label.BeginAnimation(Canvas.TopProperty, yAnim);

            return tsc.Task;
        }

        public static void DisplayMessage(string message, Canvas canvas)
        {
            //TODO: This position should probably be passed into the function
            Label messageLabel = new();
            messageLabel.Foreground = Brushes.Red;
            messageLabel.Content = message;
            messageLabel.FontSize = 30;
            canvas.Children.Add(messageLabel);
            Canvas.SetTop(messageLabel, 40);
            Canvas.SetLeft(messageLabel, 800);
            canvas.UpdateLayout();

            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(2000),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
            };

            animation.Completed += (_, _) =>
            {
                canvas.Children.Remove(messageLabel);
            };

            messageLabel.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        public static void DisplayRound(int roundNumber, Canvas canvas)
        {
            //TODO: This position should probably be passed into the function
            Label messageLabel = new();
            messageLabel.Foreground = Brushes.White;
            messageLabel.Background = Brushes.Black;
            messageLabel.Content = $"ROUND {roundNumber}";
            messageLabel.FontSize = 50;
            canvas.Children.Add(messageLabel);
            Canvas.SetTop(messageLabel, 10);
            Canvas.SetLeft(messageLabel, 800);

            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(2000),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
            };

            animation.Completed += (_, _) =>
            {
                canvas.Children.Remove(messageLabel);
            };

            messageLabel.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        public static Task DisplayTurn(Canvas canvas)
        {
            //TODO: This position should probably be passed into the function
            Label messageLabel = new();
            messageLabel.Foreground = Brushes.Black;
            messageLabel.Background = Brushes.CadetBlue;
            messageLabel.Content = $"NEW TURN";
            messageLabel.FontSize = 50;
            canvas.Children.Add(messageLabel);
            Canvas.SetTop(messageLabel, 500);
            Canvas.SetLeft(messageLabel, 800);

            var animation = new DoubleAnimation
            {
                To = 0.0,
                From = 1.0,
                Duration = TimeSpan.FromMilliseconds(2000),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn },
            };

            TaskCompletionSource tsc = new();
            animation.Completed += (_, _) =>
            {
                canvas.Children.Remove(messageLabel);
                tsc.SetResult();
            };

            messageLabel.BeginAnimation(UIElement.OpacityProperty, animation);

            return tsc.Task;
        }
    }
}
