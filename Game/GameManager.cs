namespace Game;
public class GameManager
{
    private readonly Deck _deck;
    private readonly string _preflopDataPath = @"C:\Users\Frank\Code\PokerProto\PokerAlgo";
    private readonly FolderLoader _folderLoader;

    public List<GamePlayer> Players { get; private set; }
    private readonly PlayerTable _table;
    public List<Card> CommunityCards { get; private set; }

    public GameStage Stage { get; private set; }
    private int _blind;
    private int _highestBet;

    private GamePlayer _dealer;

    List<Pot> _pots = new();

    public string StatusBuffer = "";


    public GameManager()
    {
        _deck = new();
        _folderLoader = new(_preflopDataPath);
        _blind = 40;

        Players = [
            new GamePlayer("peach", _deck.NextCard(), _deck.NextCard(), 1000),
            new GamePlayer("1", _deck.NextCard(), _deck.NextCard(), 1000),
            new GamePlayer("2", _deck.NextCard(), _deck.NextCard(), 1000),
            new GamePlayer("3", _deck.NextCard(), _deck.NextCard(), 1000),
            new GamePlayer("4", _deck.NextCard(), _deck.NextCard(), 1000),
        ];
        CommunityCards = [];

        _table = new PlayerTable(Players);

        Stage = GameStage.PreFlop;

        _highestBet = _blind;

        _dealer = Players[0];

        _table.SetCurrent(_dealer);
        _table.GetNext().Bet(_blind / 2);
        _table.GetNext().Bet(_blind);
        _table.GetNext();
    }

    public GameStateDto Next(InputAction action, int value)
    {
        return new GameStateDto{ Player = _table.Current.Value, MinBet = 40, Type = StateType.PlayerInput};
    }

}