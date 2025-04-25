namespace Game;
public class GameManager
{
    public List<GamePlayer> Players { get; set; } = new();
    public List<Pot> Pots { get; set; } = new();
    public GameStage Stage { get; set; } = GameStage.Reset;
    public Deck Deck { get; set; } = new();
    public List<Card> CommunityCards = new();
    public int blind = 20;
    public int smallBlindIndex = 1;
    public int bigBlindIndex = 2;

    public void Init()
    {
        Players = new List<GamePlayer>()
        {
            new("Player", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Sam", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Ben", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Tom", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Rob", Deck.NextCard(), Deck.NextCard(), 1000),
        };

        CommunityCards = Deck.NextCards(5);

        Stage = GameStage.PreFlop;

        Players[smallBlindIndex].Bet(blind / 2);
        Players[bigBlindIndex].Bet(blind);
    }

    public GamePlayer Next(PlayerMove move, int value)
    {
        throw new NotImplementedException();
    }

    public void AdvanceStage()
    {
        
    }

    public void AdvanceBlind()
    {
        smallBlindIndex++;
        bigBlindIndex++;
        if (bigBlindIndex >= 5)
        {
            bigBlindIndex = 0;
        }
        if (smallBlindIndex >= 5)
        {
            smallBlindIndex = 0;
        }
    }
}