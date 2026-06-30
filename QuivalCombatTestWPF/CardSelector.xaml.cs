using QuivalLogicEngine.Cards;
using System.Windows.Controls;

namespace QuivalCombatTestWPF
{
    public partial class CardSelector : UserControl
    {
        private int index = 0;
        List<TargetSelection> TargetSelections;

        public CardSelector(List<TargetSelection> targetSelections)
        {
            InitializeComponent(); 
            TargetSelections = targetSelections;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            CardSelectorLabel.Content = 
                $"Select {TargetSelections[index].NumberToPick - TargetSelections[index].SelectedTargets.Count} targets to {GetSelectMessageFromEffect(TargetSelections[index].Effect)}";
        }

        private string GetSelectMessageFromEffect(Effect effect)
        {
            switch (effect)
            {
                case QuivalLogicEngine.Cards.Effect.AttackBuffRound:
                    return "buff the attack of";
                case QuivalLogicEngine.Cards.Effect.DirectDamage:
                    return "damage";
                case QuivalLogicEngine.Cards.Effect.Heal:
                    return "heal";

                case QuivalLogicEngine.Cards.Effect.AttackUpToken:
                case QuivalLogicEngine.Cards.Effect.DamageAbsorbToken:
                case QuivalLogicEngine.Cards.Effect.DrawCard:
                case QuivalLogicEngine.Cards.Effect.RestoreAction:
                case QuivalLogicEngine.Cards.Effect.None:
                default:
                    return "";
            }
        }

        public bool CardIsValidTarget(int cardId)
        {
            return TargetSelections[index].TargetsToPickFrom.Contains(cardId);
        }

        public List<TargetSelection>? SelectCard(int cardId)
        {
            TargetSelections[index].SelectedTargets.Add(cardId);

            if (TargetSelections[index].SelectedTargets.Count >= TargetSelections[index].NumberToPick)
            {
                if (++index >= TargetSelections.Count)
                    return TargetSelections;
            }

            UpdateLabel();

            return null;
        }
    }
}
