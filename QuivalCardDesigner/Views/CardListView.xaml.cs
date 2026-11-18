using QuivalLogicEngine.Cards;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Controls;

namespace QuivalCardDesigner.Views;

public class CardGrid
{
    public string Name { get; set; }
    public int Attack { get; set; }
    public int Health { get; set; }
}

public partial class CardListView : UserControl
{
    public CardListView(Config config)
    {
        InitializeComponent();

        List<CardGrid> cardz = new List<CardGrid>();
        foreach (var dir in config.CardDirectory.GetDirectories())
        {
            var orderedFiles = dir.GetFiles().OrderBy(f => f.LastWriteTimeUtc).ToList();

            var cardDef = GetCardFromFile(orderedFiles[0].FullName);

            CardGrid cardGrid = new CardGrid()
            {
                Name = cardDef.Name,
                Attack = cardDef.Attack,
                Health = cardDef.Health,
            };

            cardz.Add(cardGrid);
        }

        CardListItems.ItemsSource = cardz;
        CardListItems.MouseDoubleClick += CardListItems_MouseDoubleClick;
    }

    private void CardListItems_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        //TODO: load the card in the design view
        throw new NotImplementedException();
    }

    //TODO: This function should probably live in some core IO module
    public static CardDefinition GetCardFromFile(string path)
    {
        string json = File.ReadAllText(path);
        JsonSerializerOptions options = new() { WriteIndented = true };
        options.Converters.Add(new JsonStringEnumConverter());
        CardDefinition cardDef = JsonSerializer.Deserialize<CardDefinition>(json, options)!;

        return cardDef;
    }
}
