namespace Game;
public class GameManager
{
    public List<GamePlayer> Players { get; private set; } = [];
    private readonly Deck Deck = new();
    public List<Card> CommunityCards { get; private set; } = [];

    public GameStage Stage { get; private set; } = GameStage.PreFlop;


    public void Init()
    {
        Players = [
            new("peach", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Giga Chad", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Shrigma Male", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Top G", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Jeff", Deck.NextCard(), Deck.NextCard(), 1000),
        ];
    }
    
}