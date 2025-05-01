namespace Game;
public class GameManager
{
    private readonly Deck _deck;
    private string _preflopDataPath = @"C:\Users\Frank\Code\PokerProto\PokerAlgo";
    private readonly FolderLoader _folderLoader;

    public List<GamePlayer> Players { get; private set; }
    private readonly PlayerTable _table;
    public List<Card> CommunityCards { get; private set; } = [];

    public GameStage Stage { get; private set; }
    private readonly int _blind = 40;
    private int _highestBet;

    private GamePlayer _dealer;


    public GameManager()
    {
        _deck = new();
        _folderLoader = new(_preflopDataPath);

        Players = [
            new("peach", _deck.NextCard(), _deck.NextCard(), 1000),
            new("Pepe", _deck.NextCard(), _deck.NextCard(), 1000),
            new("Doge", _deck.NextCard(), _deck.NextCard(), 1000),
            new("Top G", _deck.NextCard(), _deck.NextCard(), 1000),
            new("Waltah", _deck.NextCard(), _deck.NextCard(), 1000),
        ];

        _table = new PlayerTable(Players);

        Stage = GameStage.PreFlop;

        _highestBet = _blind;

        _dealer = Players[0];

        _table.SetCurrent(_dealer);
        _table.GetNext().Bet(_blind / 2);
        _table.GetNext().Bet(_blind);

        Task.Run(CalculatePlayerChances);
    }

    public PlayerDto Init()
    {
        // preflop init for GUI
        return new PlayerDto { Player = _table.GetNext(), MinBet = _blind };
    }

    public object Next(PlayerAction action, int value)
    {
        GamePlayer current = _table.Current.Value;

        switch (action)
        {
            case PlayerAction.Check:
                if (current.TotalBet == _highestBet)
                {
                    current.Check();
                }
                else throw new Exception("cannot check");
                LogPlayerMove(current, action, value);
                break;

            case PlayerAction.Call:
                if (value == _highestBet - current.TotalBet)
                {
                    current.Bet(value);
                }
                else throw new Exception("passed value does not equal call bet");
                LogPlayerMove(current, action, value);
                break;

            case PlayerAction.Raise:
                if (value > _highestBet - current.TotalBet)
                {
                    current.Bet(value);
                    _highestBet = current.TotalBet;
                }
                else throw new Exception("passed value is less than call bet");
                LogPlayerMove(current, action, value);
                break;

            case PlayerAction.Fold:
                current.Fold();
                LogPlayerMove(current, action, value);
                break;
        }

        current = _table.GetNext();
        PlayerDto dto;

        if (current.HasPlayed)
        {
            if (current.TotalBet == _highestBet)
            {
                AdvanceStage();

                if (Stage == GameStage.Showdown) return Showdown();

                current = _table.GetNext();
            }
        }

        dto = new PlayerDto { Player = current, MinBet = _highestBet - current.TotalBet };
        return dto;
    }

    private void AdvanceStage()
    {
        switch (Stage)
        {
            case GameStage.PreFlop:
                Stage = GameStage.Flop;

                foreach (GamePlayer p in Players)
                {
                    p.ResetBettingRound();
                }

                CommunityCards = _deck.NextCards(3);
                Task.Run(CalculatePlayerChances);

                _table.SetCurrent(_dealer);
                Console.WriteLine("\nThe Flop: " + string.Join(' ', CommunityCards));
                break;

            case GameStage.Flop:
                Stage = GameStage.Turn;

                foreach (GamePlayer p in Players)
                {
                    p.ResetBettingRound();
                }

                CommunityCards.Add(_deck.NextCard());
                Task.Run(CalculatePlayerChances);

                _table.SetCurrent(_dealer);
                Console.WriteLine("\nThe Turn: " + CommunityCards[3]);
                break;

            case GameStage.Turn:
                Stage = GameStage.River;

                foreach (GamePlayer p in Players)
                {
                    p.ResetBettingRound();
                }

                CommunityCards.Add(_deck.NextCard());
                Task.Run(CalculatePlayerChances);

                _table.SetCurrent(_dealer);
                Console.WriteLine("\nThe River: " + CommunityCards[4]);
                break;

            case GameStage.River:
                Stage = GameStage.Showdown;
                break;

            case GameStage.Showdown:
                break;
        }
    }

    private void CalculatePlayerChances()
    {
        foreach (GamePlayer p in Players)
        {
            if (!p.HasFolded)
            {
                if (Stage == GameStage.PreFlop)
                {
                    p.Chances = ChanceCalculator.GetWinningChancePreFlopLookUp(p.HoleCards, CountPlayersLeft() - 1, _folderLoader);
                }
                else
                {
                    p.Chances = ChanceCalculator.GetWinningChanceSimParallel(p.HoleCards, CommunityCards, CountPlayersLeft() - 1, 10_000);
                }
            }
        }
    }

    private int CountPlayersLeft()
    {
        int count = 0;
        foreach (GamePlayer p in Players)
        {
            if (p.HasFolded) continue;
            count++;
        }
        return count;
    }

    private object Showdown()
    {
        List<Player> players = new();
        foreach (var item in Players)
        {
            if (!item.HasFolded)
            {
                players.Add(item);
            }
        }
        List<GamePlayer> winners = Algo.GetWinners(players, CommunityCards).OfType<GamePlayer>().ToList();
        Console.WriteLine("\nWinners:");
        foreach (GamePlayer p in winners)
        {
            Console.WriteLine(p);
            Console.WriteLine(p.WinningHand);
        }
        return new object();
    }

    private void LogPlayerMove(GamePlayer player, PlayerAction action, int value)
    {
        Console.WriteLine($"{player.Name} => {action}" + (action == PlayerAction.Call || action == PlayerAction.Raise ? $" {value}" : ""));
    }
}