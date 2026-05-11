using QuivalLogicEngine.Cards;

namespace QuivalCombatTestWPF
{
    internal static class Mapper
    {
        public static List<HandCard> MapToHandCards(List<Card> cards)
        {
            List<HandCard> result = new();

            foreach (var card in cards)
            {
                HandCard handcard = new(card.Id);
                handcard.CardNameLabel.Content = card.Name;
                handcard.CardDescriptionLabel.Text = card.Description;
                handcard.CostContent.Content = card.Cost;

                if (card is CreatureCard cc)
                {
                    handcard.AttackLabel.Content = cc.Attack;
                    handcard.HealthLabel.Content = cc.Health;
                }

                result.Add(handcard);
            }

            return result;
        }
    }
}
