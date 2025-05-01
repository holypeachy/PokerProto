namespace Game;
public class GameManager
{
    private readonly Deck Deck;
    public List<GamePlayer> Players { get; private set; }
    private PlayerTable _table;
    public List<Card> CommunityCards { get; private set; } = [];

    public GameStage Stage { get; private set; }
    private readonly int blind = 40;
    private int highestBet;

    public GamePlayer dealer;

    public GameManager()
    {
        Deck = new();

        Players = [
            new("peach", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Pepe", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Doge", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Top G", Deck.NextCard(), Deck.NextCard(), 1000),
            new("Waltah", Deck.NextCard(), Deck.NextCard(), 1000),
        ];

        _table = new PlayerTable(Players);

        Stage = GameStage.PreFlop;

        highestBet = blind;

        dealer = Players[0];

        _table.SetCurrent(dealer);
        _table.GetNext().Bet(blind / 2);
        _table.GetNext().Bet(blind);
    }

    public PlayerDto Init()
    {
        PlayerDto dto = new PlayerDto { Player = _table.GetNext(), MinBet = blind };
        return dto;
    }

    public object Next(PlayerAction action, int Value)
    {
        GamePlayer current = _table.Current.Value;

        switch (action)
        {
            case PlayerAction.Check:
                if (current.TotalBet == highestBet)
                {
                    current.Check();
                }
                else
                {
                    throw new Exception("cannot check");
                }
                break;
            case PlayerAction.Call:
                if (Value == highestBet - current.TotalBet)
                {
                    current.Bet(Value);
                }
                else
                {
                    throw new Exception("passed value does not equal call bet");
                }
                break;
            case PlayerAction.Raise:
                if (Value > highestBet - current.TotalBet)
                {
                    current.Bet(Value);
                    highestBet = current.TotalBet;
                }
                else
                {
                    throw new Exception("passed value is less than call bet");
                }
                break;
            case PlayerAction.Fold:
                current.Fold();
                break;
        }

        current = _table.GetNext();
        PlayerDto dto;

        if (current.HasPlayed)
        {
            if (current.TotalBet == highestBet)
            {
                AdvanceStage();
                
                if (Stage == GameStage.Showdown) return Showdown();

                current = _table.GetNext();
            }
        }

        dto = new PlayerDto { Player = current, MinBet = highestBet - current.TotalBet };
        return dto;
    }

    private void AdvanceStage()
    {
        switch (Stage)
        {
            case GameStage.PreFlop:
                Stage = GameStage.Flop;
                CommunityCards = Deck.NextCards(3);
                foreach (GamePlayer p in Players)
                {
                    p.ResetBettingRound();
                }
                _table.SetCurrent(dealer);
                break;

            case GameStage.Flop:
                Stage = GameStage.Turn;
                CommunityCards.Add(Deck.NextCard());
                foreach (GamePlayer p in Players)
                {
                    p.ResetBettingRound();
                }
                _table.SetCurrent(dealer);
                break;

            case GameStage.Turn:
                Stage = GameStage.River;
                CommunityCards.Add(Deck.NextCard());
                foreach (GamePlayer p in Players)
                {
                    p.ResetBettingRound();
                }
                _table.SetCurrent(dealer);
                break;

            case GameStage.River:
                Stage = GameStage.Showdown;
                break;

            case GameStage.Showdown:
                break;
        }
    }

    private object Showdown()
    {
        throw new NotImplementedException();
    }
}