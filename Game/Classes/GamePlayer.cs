namespace Game;
public class GamePlayer : Player
{
    public int Stack { get; private set; }

    public bool HasPlayed { get; private set; } = false;
    public bool IsFolded { get; private set; } = false;
    public bool IsAllIn { get; private set; } = false;

    public int TotalBet { get; private set; } = 0;

    public (double win, double tie) Chances { get; set; } = (0, 0);


    public GamePlayer(string name, Card first, Card second, int stack) : base(name, first, second)
    {
        Stack = stack;
    }


    public void Fold()
    {
        IsFolded = true;
    }

    public void Check()
    {
        HasPlayed = true;
    }
    
    public void Bet(int amount)
    {
        HasPlayed = true;

        if (amount >= Stack)
        {
            TotalBet += Stack;
            Stack = 0;
            IsAllIn = true;
            Console.WriteLine($"{Name} goes all-in");
            return;
        }

        Stack -= amount;
        TotalBet += amount;
    }

    public void ResetBettingRound()
    {
        HasPlayed = false;
    }

    public void AddWinnings(int amount)
    {
        Stack += amount;
    }

    public void ResetHand()
    {
        TotalBet = 0;
        IsFolded = false;
        IsAllIn = false;
        HasPlayed = false;
        Chances = (0, 0);
    }

}