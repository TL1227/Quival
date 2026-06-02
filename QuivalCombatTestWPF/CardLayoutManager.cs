using System.Windows;
using System.Windows.Controls;

namespace QuivalCombatTestWPF;

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

    public void FillCombatZonePoints(CombatZone combatZone, Grid battleField)
    {
        Grid[] zones = [combatZone.OpponentCombatZone, combatZone.PlayerCombatZone];

        for (int i = 0; i < 2; i++)
        {
            zones[i].Children.Clear();

            for (int y = 0; y < 5; y++)
            {
                var card = new BoardCard() { CardId = -1, HasActed = true, Side = Side.Player };
                Grid.SetRow(card, i);
                Grid.SetColumn(card, y);
                zones[i].Children.Add(card);

                zones[i].UpdateLayout();

                Point point = card.TransformToVisual(battleField).Transform(new Point(0,0));
                CombatZones[i][y] = point;
            }

            zones[i].Children.Clear();
        }
    }

    public void FillBlockZonePoints(BlockZone playerBlockZone, BlockZone opponentBlockZone)
    {
        for (int i = 0; i < 2; i++)
        {
            BlockZone bz = i == 0 ? playerBlockZone : opponentBlockZone;
            var card = new BoardCard() { CardId = -1, HasActed = true, Side = Side.Player };
            bz.BlockArea.Children.Clear();
            bz.BlockArea.Children.Add(card);
            bz.BlockArea.UpdateLayout();
            Point point = card.TransformToVisual(Window.GetWindow(card)).Transform(new Point(0,0));
            BlockZones[i] = point;
            bz.BlockArea.Children.Clear();
        }
    }
}
