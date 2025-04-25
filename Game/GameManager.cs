namespace Game;
public class GameManager
{
    public List<GamePlayer> Players { get; set; } = new();
    public List<Pot> Pots { get; set; } = new();
    public GameStage Stage { get; set; } = GameStage.Reset;
    public Deck GameDeck { get; set; } = new();

    public void Init(List<GamePlayer> players)
    {
        Players = players;
    }

    public void NextStage()
    {

    }

    public void RoundBets()
    {

    }
}