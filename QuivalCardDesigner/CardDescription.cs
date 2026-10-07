using QuivalLogicEngine.Cards;
using System.Text;

using Key = QuivalLogicEngine.CardDescription.CardDescriptionKeys;

namespace QuivalCardDesigner;

public class CardDescription
{
    public static string Ability(Ability ability, Trigger trigger)
    {
        StringBuilder sb = new();

        string conditionalText = ability.GetConditionalText();

        string effectText = ability.Effect.GetEffectCardDescription(ability.Target, trigger);

        effectText = effectText.Replace(Key.EffectValue, ability.Value.Amount.ToString());

        if (ability.Value is Count count)
        {
            if (effectText.Contains(Key.CountValue))
            {
                effectText = effectText.Replace(Key.CountValue, count.GetCountText());
            }
            else
            {
                effectText += ' ' + count.GetCountText();
            }
        }
        else
        {
            if (effectText.Contains(Key.CountValue))
            {
                effectText = effectText.Replace(Key.CountValue, "");
            }
        }

        if (ability.Value.Amount > 1)
            effectText = effectText.Replace("(", "").Replace(")", "");
        else
            effectText = effectText.Replace("(s)", "");

        effectText = effectText.Replace("  ", " ");

        if (conditionalText == "")
            sb.Append($"{effectText}");
        else
            sb.Append($"{conditionalText} {effectText}");

        return sb.ToString();
    }
}
