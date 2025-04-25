namespace Game;

public class GamePlayer : Player
{
    public int Stack { get; set; }
    public bool IsFolded { get; set; } = false;
    public int TotalBet { get; set; } = 0;

    public GamePlayer(string name, Card first, Card second, int stack) : base(name, first, second)
    {
        Stack = stack;
    }

    public void Bet(int amount)
    {
        if (amount > Stack)
        {
            TotalBet += Stack;
            Stack = 0;
            return;
        }

        Stack -= amount;
        TotalBet += amount;
    }

    public void Fold()
    {
        IsFolded = true;
    }

    public void Reset()
    {
        TotalBet = 0;
        IsFolded = false;
    }
}
