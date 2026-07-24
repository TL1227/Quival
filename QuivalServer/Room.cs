using QuivalLogicEngine;
using QuivalLogicEngine.Cards;
using QuivalLogicEngine.Messages;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuivalServer;

internal class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public Match Match { get; set; } = new();
    public PlayerClient[] Players { get; set; } = new PlayerClient[2];

    public bool BothPlayersSet()
    {
        return Players[0] != null && Players[1] != null;
    }

    public bool AddPlayer(PlayerClient player, List<Card> deck)
    {
        if (Players[0] == null)
        {
            player.Id = 0;
            Players[0] = player;
            AddToMatch(player, deck);

            return true;
        }
        else if (Players[1] == null)
        {
            player.Id = 1;
            Players[1] = player;
            AddToMatch(player, deck);
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task TryStartMatch()
    {
        if (BothPlayersSet())
        {
            foreach (var player in Players)
            {
                GameStateUpdate update = new()
                {
                    GameState = Match.GetGameState(player.Id)
                };

                //player.Writer.WriteLineAsync(gs);
                await player.SendMessageAsync(update);
            }
        }
    }

    private void AddToMatch(PlayerClient playerClient, List<Card> deck)
    {
        Match.SetPlayer(playerClient.Id, deck);
        Console.WriteLine($"Added Player {playerClient.Id} to {Name} [id: {Id}]");
    }

    public async Task HandleMessage(Message message, PlayerClient player)
    {
        switch (message)
        {
            case SubmitTurn submitTurn:
                {
                    Console.WriteLine($"Player {player.Id} submitting {submitTurn.Turn.TurnType}");

                    if (Match.PlayerHasSetTurn(player.Id))
                    {
                        Console.WriteLine($"Player {player.Id} already has turn set");
                        return;
                    }

                    Match.SubmitTurn(player.Id, submitTurn.Turn);

                    var selectionTargets = Match.GetSelectionsIfPlayerNeedsThem(player.Id);
                    if (selectionTargets != null)
                    {
                        MakeSelections ms = new() { TargetSelections = selectionTargets };

                        string? gs = JsonSerializer.Serialize(ms, ms.GetType());
                        //player.Writer.WriteLineAsync(gs);
                        await player.SendMessageAsync(ms);
                    }
                    else
                    {
                        if (Match.BothPlayersHaveSubmittedTurns() && Match.BothPlayersHaveSubmittedTargets())
                        {
                            Console.WriteLine($"Processing Both Players' Turns");
                            await ProcessCards();
                        }
                    }
                }
                break;
            case MakeSelections selections:
                {
                    Match.SubmitTargetSelection(player.Id, selections.TargetSelections);

                    if (Match.BothPlayersHaveSubmittedTurns() && Match.BothPlayersHaveSubmittedTargets())
                    {
                        Console.WriteLine($"Both players have submitted turns");
                        await ProcessCards();
                    }
                }
                break;
            case StartMatchRequest request:
                {
                    await TryStartMatch();
                }
                break;
            default:
                Console.WriteLine($"Unknown message from player {player.Id}");
                break;
        }
    }

    public async Task ProcessCards()
    {
        Match.ProcessCards();

        foreach (var player in Players)
        {
            GameStateUpdate update = new()
            {
                GameState = Match.GetGameState(player.Id)
            };

            //string? gs = JsonSerializer.Serialize(update, update.GetType());
            //await player.Writer.WriteLineAsync(gs);
            await player.SendMessageAsync(update);
        }
    }
}
