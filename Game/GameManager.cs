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
            new("peach", _deck.NextCard(), _deck.NextCard(), 100),
            new("Pepe", _deck.NextCard(), _deck.NextCard(), 40),
            new("Doge", _deck.NextCard(), _deck.NextCard(), 50),
            new("Top G", _deck.NextCard(), _deck.NextCard(), 20),
            new("Waltah", _deck.NextCard(), _deck.NextCard(), 200),
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
        GameStateDto? temp;

        if (Stage == GameStage.Showdown)
        {
            PayWinners();
            // TODO Check if at least 2 players are not busted.
            temp = CheckGameOver();
            if (temp is not null)
            {
                Console.WriteLine("Game is Over!");
                return temp;
            }
            NextRound();
            Console.WriteLine($"Next Hand - Dealer: {_dealer.Name}\n");
            Console.WriteLine("Pre-Flop:");

            foreach (GamePlayer p in Players)
            {
                if (p.Stack == 0 && p.TotalBet != 0)
                {
                    p.Bet(0);
                }
            }


            // TODO: Check if at least 2 players can bet, otherwise we skip to showdown.
            temp = CheckSkipToShowdown();
            if (temp is not null) return temp;

            return new GameStateDto { Type = StateType.PlayerInput, Player = current, MinBet = _highestBet - current.TotalBet };
        }

        switch (action)
        {
            case InputAction.Ping:
                return new GameStateDto { Type = StateType.PlayerInput, Player = current, MinBet = _highestBet - current.TotalBet };

            case InputAction.Check:
                if (current.TotalBet == _highestBet)
                {
                    current.Check();
                }
                else throw new Exception("Cannot check, logic is wrong.");

                LogPlayerMove(current, action, value);
                break;

            case InputAction.Call:
                    current.Bet(value);

                LogPlayerMove(current, action, value);
                break;

            case InputAction.Raise:
                current.Bet(value);
                _highestBet = current.TotalBet;

                LogPlayerMove(current, action, value);
                break;

            case InputAction.Fold:
                current.Fold();

                LogPlayerMove(current, action, value);
                break;
        }

        // TODO Check default win
        if (CountNotFoldedPlayers() == 1)
        {
            WinByDefault();
            Stage = GameStage.Showdown;
            return new GameStateDto { Type = StateType.RoundEndInfo};
        }

        current = _table.GetNext();

        if (current.HasPlayed && current.TotalBet == _highestBet)
        {
            // TODO: Check if at least 2 players can bet, otherwise we skip to showdown.
            temp = CheckSkipToShowdown();
            if (temp is not null) return temp;

            AdvanceStage();

            if (Stage == GameStage.Showdown)
            {
                return Showdown();
            }

            current = _table.GetNext();
        }
        else if (CountNotFoldedPlayers() == 1 && current.TotalBet == _highestBet)
        {
            temp = CheckSkipToShowdown();
            if (temp is not null) return temp;
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
                    p.Chances = ChanceCalculator.GetWinningChancePreFlopLookUp(p.HoleCards, CountNotFoldedPlayers() - 1, _folderLoader);
                }
                else
                {
                    p.Chances = ChanceCalculator.GetWinningChanceSimParallel(p.HoleCards, CommunityCards, CountNotFoldedPlayers() - 1, 10_000);
                }
            }
        }
    }

    private int CountNotFoldedPlayers()
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

        Console.WriteLine("-- Pots --");
        Pot current;
        for (int i = 0; i < _pots.Count; i++)
        {
            current = _pots[i];
            List<Player> algoPlayers = current.Players.Cast<Player>().ToList();
            if (current.Players.Count > 1) current.Winners = Algo.GetWinners(algoPlayers, CommunityCards).Cast<GamePlayer>().ToList();
            else if (current.Players.Count == 1) current.Winners = [current.Players[0]];
            else throw new Exception();

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
        // TODO: Check if there are at least 2 players that can play at start of round, otherwise game over.

        _deck.ResetDeck();
        foreach (GamePlayer p in Players)
        {
            p.ResetHand();
            if (p.Stack > 0) p.NewHand(_deck.NextCard(), _deck.NextCard());
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

    private GameStateDto? CheckGameOver()
    {
        int count = 0;
        foreach (GamePlayer p in Players)
        {
            if (p.Stack == 0) continue;
            count++;
        }

        if (count == 1) return new GameStateDto { Type = StateType.GameEndinfo, Player = Players.First(p => p.Stack != 0) };
        return null;
    }

    private void WinByDefault()
    {
        GamePlayer winner = Players.First(p => !p.IsFolded);
        int value = 0;
        foreach (GamePlayer p in Players)
        {
            value += p.TotalBet;
        }
        _pots = [new Pot(value, [winner], [winner])];
        Console.WriteLine($"Winner By Default: {winner.Name}");
    }

    private GameStateDto? CheckSkipToShowdown()
    {
        int count = 0;
        foreach (GamePlayer p in Players)
        {
            if (p.IsFolded || p.IsAllIn || (p.Stack == 0 && p.TotalBet == 0)) continue;
            count++;
        }

        if (count < 2)
        {
            while (true)
            {
                AdvanceStage();
                if (Stage == GameStage.Showdown)
                {
                    return Showdown();
                }
            }
        }

        return null;
    }

}