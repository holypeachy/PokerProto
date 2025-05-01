namespace Game;

public class GamePlayer : Player
{
    public int Stack { get; private set; }
    public bool HasFolded { get; private set; } = false;
    public bool IsAllIn { get; private set; } = false;
    public int TotalBet { get; private set; } = 0;
    public bool HasPlayed { get; set; } = false;

    public GamePlayer(string name, Card first, Card second, int stack) : base(name, first, second)
    {
        Stack = stack;
    }

    public void Bet(int amount)
    {
        HasPlayed = true;

        if (amount > Stack)
        {
            TotalBet += Stack;
            Stack = 0;
            IsAllIn = true;
            return;
        }

        Stack -= amount;
        TotalBet += amount;
    }

    public void Fold()
    {
        HasFolded = true;
    }

    public void Check()
    {
        HasPlayed = true;
    }

    public void ResetBettingRound()
    {
        HasPlayed = false;
    }

    public void ResetHand()
    {
        TotalBet = 0;
        HasFolded = false;
        IsAllIn = false;
        HasPlayed = false;
    }
}
