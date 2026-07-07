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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using QuivalLogicEngine.Cards;

namespace QuivalCardDesigner.Controls
{
    public partial class AbilityControl : UserControl
    {
        Ability CurrentAbility { get; set; } = new();

        public AbilityControl(Effect effect)
        {
            InitializeComponent();
            CurrentAbility.Effect = effect;

            EffectLabel.Content = effect;
            TargetTypeComboBox.ItemsSource = Enum.GetValues<TargetType>();
            SideComboBox.ItemsSource = Enum.GetValues<Side>();
            ValueFromComboBox.ItemsSource = Enum.GetValues<ValueFrom>();
            ConditionalsComboBox.ItemsSource = Enum.GetValues<Conditional>();
        }
    }
}
