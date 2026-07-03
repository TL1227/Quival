using System.Windows;
using System.Windows.Controls;

namespace QuivalCombatTestWPF
{
    public static class QuivalText
    {
        public static void FitFontSize(TextBlock textBlock, double maxWidth, double maxHeight, double maxFontSize = 20, double minFontSize = 8)
        {
            double fontSize = maxFontSize;

            while (fontSize > minFontSize)
            {
                textBlock.FontSize = fontSize;
                textBlock.Measure(new Size(maxWidth, double.PositiveInfinity));

                if (textBlock.DesiredSize.Height <= maxHeight)
                    break;

                fontSize -= 0.5;
            }

            textBlock.FontSize = fontSize;
        }
    }
}
