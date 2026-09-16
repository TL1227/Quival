using System.Windows;
using System.Windows.Controls;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Cards.Effects;

namespace QuivalCardDesigner.Controls;

public partial class AbilityControl : UserControl
{
    Ability CurrentAbility { get; set; } = new();
    bool BonusEffectVisible { get; set; }

    public AbilityControl(Effect effect)
    {
        InitializeComponent();
        CurrentAbility.Effect = effect;
        EffectLabel.Content = effect.GetType().Name.Replace("Effect", "");

        TargetControl.Initialise(CurrentAbility);
        TargetControl.ChangeToTarget += TargetControlHasChanged;

        BonusEffectButton.Click += BonusEffectButton_Click;
    }

    private void TargetControlHasChanged(object? sender, EventArgs e)
    {
        if (TargetControl.TargetTypeComboBox.SelectedItem is Target target)
        {
            PopulateBonusEffectComboBox(target);
        }
    }

    private void BonusEffectButton_Click(object sender, RoutedEventArgs e)
    {
        if (TargetControl.TargetTypeComboBox.SelectedItem is Target target)
        {
            if (BonusEffectVisible)
            {
                BonusEffectStackPanel.Visibility = Visibility.Collapsed;
                BonusEffectButton.Content = " Add Bonus Effect ";

                CurrentAbility.BonusValue = null;
                CurrentAbility.BonusEffect = null;
                CurrentAbility.BonusConditionals = null;
                CurrentAbility.BonusConditionalType = null;

                BonusEffectVisible = false;
            }
            else
            {
                BonusEffectStackPanel.Visibility = Visibility.Visible;
                PopulateBonusEffectComboBox(target);
                BonusEffectButton.Content = " Remove Bonus Effect ";
                BonusEffectVisible = true;
            }
        }
    }

    private void PopulateBonusEffectComboBox(Target target)
    {
        TargetPool pool = new();

        if (target is SelectionTarget st)
        {
            pool = (TargetPool)TargetControl.TargetPoolComboBox.SelectedItem;
        }
        else
        {
            pool = target.GetTargetPool();
        }

        var baseType = typeof(Effect);
        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => baseType.IsAssignableFrom(type) &&
                type != baseType &&
                !type.IsAbstract)
            .ToList();

        List<Effect> effects = new();
        foreach (var type in types)
        {
            var effect = (Effect)Activator.CreateInstance(type)!;

            switch (effect.ValidTargetPool)
            {
                case TargetPool.Creatures:
                    if (TargetPool.Creatures == pool)
                        effects.Add(effect);
                    break;
                case TargetPool.Controllers:
                    if (TargetPool.Controllers == pool)
                        effects.Add(effect);
                    break;
                case TargetPool.Damagables:
                    if (TargetPool.Controllers == pool || TargetPool.Creatures == pool || TargetPool.Damagables == pool)
                        effects.Add(effect);
                    break;
                default:
                    break;
            }

        }

        BonusEffectComboBox.ItemsSource = effects;
        BonusEffectComboBox.DisplayMemberPath = "EffectString";
        BonusEffectComboBox.SelectedIndex = 0;
    }

    public Ability GetAbility()
    {
        TargetControl.PopulateTarget(CurrentAbility);
        ValueControl.PopulateValue(CurrentAbility);
        ConditionalControl.PopulateConditional(CurrentAbility);

        if (BonusEffectVisible)
        {
            bool bonus = true;
            CurrentAbility.BonusEffect = (Effect)BonusEffectComboBox.SelectedItem;
            BonusValueControl.PopulateValue(CurrentAbility, bonus);
            BonusConditionalControl.PopulateConditional(CurrentAbility, bonus);
        }

        return CurrentAbility;
    }
}
