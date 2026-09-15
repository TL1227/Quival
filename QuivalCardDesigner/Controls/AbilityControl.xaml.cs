using System.Windows;
using System.Windows.Controls;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Cards.Effects;

namespace QuivalCardDesigner.Controls;

public partial class AbilityControl : UserControl
{
    Ability CurrentAbility { get; set; } = new();

    public AbilityControl(Effect effect)
    {
        InitializeComponent();
        CurrentAbility.Effect = effect;
        EffectLabel.Content = effect.GetType().Name.Replace("Effect", "");

        TargetControl.Initialise(CurrentAbility);
    }

    public Ability GetAbility()
    {
        TargetControl.PopulateTarget(CurrentAbility);
        ValueControl.PopulateValue(CurrentAbility);
        ConditionalControl.PopulateConditional(CurrentAbility); 

        return CurrentAbility;
    }

}
