using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Cards.Effects;
using System.Windows;
using System.Windows.Controls;

namespace QuivalCardDesigner.Controls;

public partial class AbilityControl : UserControl
{
    Ability CurrentAbility { get; set; } = new();
    bool BonusEffectVisible { get; set; }

    public AbilityControl(Ability ability)
    {
        InitializeComponent();

        CurrentAbility = ability;
        EffectLabel.Content = CurrentAbility.Effect.GetType().Name.Replace("Effect", "");

        TargetControl.Bind(CurrentAbility);
        TargetControl.ChangeToTarget += TargetControlHasChanged;

        ValueControl.Bind(CurrentAbility);

        ConditionalControl.Bind(CurrentAbility.Conditionals);
        BonusConditionalControl.Bind(CurrentAbility.BonusConditionals);

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

                BonusEffectVisible = false;
            }
            else
            {
                BonusEffectStackPanel.Visibility = Visibility.Visible;
                BonusEffectButton.Content = " Remove Bonus Effect ";

                PopulateBonusEffectComboBox(target);

                BonusEffectVisible = true;
            }

            CurrentAbility.BonusValue = null;
            CurrentAbility.BonusEffect = null;
            CurrentAbility.BonusConditionals.Clear();
            CurrentAbility.BonusConditionalType = ConditionalType.And;
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
}
