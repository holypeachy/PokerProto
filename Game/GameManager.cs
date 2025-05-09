using System.Diagnostics;

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

    List<Pot> _pots = new();

    public string StatusBuffer = "";


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
        _table.GetNext();

        Task.Run(CalculatePlayerChances);
    }

    public GameStateDto Next(InputAction action, int value)
    {
        GamePlayer current = _table.Current.Value;

        if (Stage == GameStage.Showdown)
        {
            PayWinners();
            NextRound();
            Console.WriteLine($"Next Hand - Dealer: {_dealer.Name}\n");
            Console.WriteLine("Pre-Flop:");
            return new GameStateDto { Type = StateType.PlayerInput, Player = current, MinBet = _highestBet - current.TotalBet};
        }

        switch (action)
        {
            case InputAction.Ping:
                return new GameStateDto { Type = StateType.PlayerInput, Player = current, MinBet = _highestBet - current.TotalBet};

            case InputAction.Check:
                if (current.TotalBet == _highestBet)
                {
                    current.Check();
                }
                else throw new Exception("Cannot check, logic is wrong.");

                LogPlayerMove(current, action, value);
                break;

            case InputAction.Call:
                if (value == _highestBet - current.TotalBet)
                {
                    current.Bet(value);
                }
                else throw new Exception("Passed value does not equal call bet, logic is wrong.");

                LogPlayerMove(current, action, value);
                break;

            case InputAction.Raise:
                if (value > _highestBet - current.TotalBet)
                {
                    current.Bet(value);
                    _highestBet = current.TotalBet;
                }
                else throw new Exception("Passed value is less than call bet, logic is wrong.");

                LogPlayerMove(current, action, value);
                break;

            case InputAction.Fold:
                current.Fold();

                LogPlayerMove(current, action, value);
                break;
        }

        current = _table.GetNext();

        if (current.HasPlayed)
        {
            if (current.TotalBet == _highestBet)
            {
                AdvanceStage();

                if (Stage == GameStage.Showdown)
                {
                    return Showdown();
                }

                current = _table.GetNext();
            }
        }
        return new GameStateDto { Type = StateType.PlayerInput, Player = current, MinBet = _highestBet - current.TotalBet };
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
                throw new Exception("AdvanceStage was called on Stage = Showdown. Logic is wrong.");
        }
    }

    private void CalculatePlayerChances()
    {
        foreach (GamePlayer p in Players)
        {
            if (!p.IsFolded)
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
            if (p.IsFolded) continue;
            count++;
        }
        return count;
    }

    private GameStateDto Showdown()
    {
        StatusBuffer = "Check terminal for showdown information";
        _pots = PotAlgo.GetPots(Players);

        Pot current;
        for (int i = 0; i < _pots.Count; i++)
        {
            current = _pots[i];
            List<Player> algoPlayers = current.Players.Cast<Player>().ToList();
            current.Winners = Algo.GetWinners(algoPlayers, CommunityCards).Cast<GamePlayer>().ToList();

            Console.WriteLine("Pot:");
            Console.WriteLine(current);
        }

        return new GameStateDto() { Type = StateType.RoundEndInfo, Pots = _pots };
    }

    private void PayWinners()
    {
        Pot current;
        for (int i = 0; i < _pots.Count; i++)
        {
            current = _pots[i];
            current.PayWinners();
        }
    }

    private void LogPlayerMove(GamePlayer player, InputAction action, int value)
    {
        string s = $"{player.Name} => {action} {value}";
        Console.WriteLine(s);
        StatusBuffer = s;
    }

    private void NextRound()
    {
        _deck.ResetDeck();
        foreach (GamePlayer p in Players)
        {
            p.ResetHand();
            if(p.Stack > 0) p.NewHand(_deck.NextCard(), _deck.NextCard());
        }

        CommunityCards = [];

        Stage = GameStage.PreFlop;

        _highestBet = _blind;

        _table.SetCurrent(_dealer);
        _dealer = _table.GetNext();

        _table.GetNext().Bet(_blind / 2);
        _table.GetNext().Bet(_blind);
        _table.GetNext();

        Task.Run(CalculatePlayerChances);
    }

}