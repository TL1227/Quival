using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace QuivalCombatTestWPF;

//TODO: This whole class's job is just to calculate the position of the cards that are being summoned for animation purposes.
//Once it's been calculated once it will probably remain the same.
//Maybe we should look at precalculating these things.
internal class CardLayoutManager
{
    public List<Point[]> CombatZones { get; set; } = new();
    public List<Point> BlockZones { get; set; } = new();

    public CardLayoutManager()
    {
        CombatZones.Add(new Point[5]); //player
        CombatZones.Add(new Point[5]); //opponent

        BlockZones.Add(new Point()); //player
        BlockZones.Add(new Point()); //opponent
    }

    public void FillCombatZonePoints(CombatZone[] combatZones, Grid battleField)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int y = 0; y < 5; y++)
            {
                var card = BoardCard.GetBlankCard();
                combatZones[i].SummonSlots[y].ClearCard();

                combatZones[i].SummonSlots[y].SetCard(card);
                combatZones[i].UpdateLayout();

                try
                {
                    Point point = card.TransformToVisual(battleField).Transform(new Point(0, 0));
                    CombatZones[i][y] = point;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                combatZones[i].SummonSlots[y].ClearCard();
            }
        }
    }

    public void FillBlockZonePoints(BlockZone[] blockZones, Grid battleField)
    {
        for (int i = 0; i < 2; i++)
        {
            var card = BoardCard.GetBlankCard();
            blockZones[i].BlockArea.Children.Clear();

            blockZones[i].BlockArea.Children.Add(card);
            blockZones[i].BlockArea.UpdateLayout();
            Point point = card.TransformToVisual(Window.GetWindow(card)).Transform(new Point(0,0));
            BlockZones[i] = point;

            blockZones[i].BlockArea.Children.Clear();
        }
    }
}
