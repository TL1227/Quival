using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace QuivalCombatTestWPF.Colours
{
    static class QuivalColour
    {
        public static SolidColorBrush HighlightColour { get; set; } = new SolidColorBrush(Colors.MediumAquamarine);

        public static void ChangetoPurpleHighlights()
        {
            HighlightColour.Color = Colors.MediumPurple;
        }

        public static void ChangetoBlueHighlights()
        {
            HighlightColour.Color = Colors.MediumAquamarine;
        }
    }
}
