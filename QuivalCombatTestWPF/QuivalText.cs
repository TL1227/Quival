using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuivalCombatTestWPF
{
    public static class QuivalText
    {
        public static void FitFontSize(TextBlock textBlock, double maxFontSize, double minFontSize = 8)
        {
            double width = textBlock.Width > 0 ? textBlock.Width : textBlock.ActualWidth;
            double height = textBlock.Height > 0 ? textBlock.Height : textBlock.ActualHeight;

            if (width <= 0 || height <= 0) return;

            double fontSize = maxFontSize;

            while (fontSize > minFontSize)
            {
                var formatted = new FormattedText(
                    textBlock.Text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                    fontSize,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(textBlock).PixelsPerDip)
                {
                    MaxTextWidth = width,
                    TextAlignment = textBlock.TextAlignment
                };

                if (formatted.Height <= height)
                    break;

                fontSize -= 1;
            }

            textBlock.FontSize = fontSize;
        }
    }
}
