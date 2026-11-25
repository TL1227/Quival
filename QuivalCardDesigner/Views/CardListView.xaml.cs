using QuivalLogicEngine.Cards;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Controls;

namespace QuivalCardDesigner.Views;

public class CardGrid
{
    public required string Name { get; set; }
    public int Attack { get; set; }
    public int Health { get; set; }
    public string Description { get; set; }
    public required CardDefinition CardDefinition { get; set; }
}

public partial class CardListView : UserControl
{
    private MainWindow MainWindow {  get; set; }

    public CardListView(MainWindow mainwindow)
    {
        InitializeComponent();
        MainWindow = mainwindow;

        List<CardGrid> cardz = new List<CardGrid>();
        foreach (var dir in Config.CardDirectory.GetDirectories())
        {
            var orderedFiles = dir.GetFiles().OrderBy(f => f.LastWriteTimeUtc).ToList();

            var cardDef = GetCardFromFile(orderedFiles[0].FullName);

            CardGrid cardGrid = new CardGrid()
            {
                Name = cardDef.Name,
                Attack = cardDef.Attack,
                Health = cardDef.Health,
                Description = cardDef.Description,
                CardDefinition = cardDef
            };

            cardz.Add(cardGrid);
        }

        CardListItems.ItemsSource = cardz;
        CardListItems.MouseDoubleClick += CardListItems_MouseDoubleClick;
        CardListItems.UpdateLayout();

        Loaded += CardListView_Loaded;
    }

    private void CardListView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        //This hides the last property on the CardGrid object
        CardListItems.Columns[^1].MaxWidth = 0;
    }

    private void CardListItems_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (CardListItems.SelectedItem != null && CardListItems.SelectedItem is CardGrid g)
        {
            MainWindow.OpenExistingCardView(g.CardDefinition);
        }
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
