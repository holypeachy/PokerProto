namespace Game;

public class GamePlayer : Player
{
    public Guid Id { get; }
    public int Stack { get; set; }
    public bool IsFolded { get; set; } = false;
    public int TotalBet { get; set; } = 0;

    public GamePlayer(Guid id, string name, Card first, Card second, int stack) : base(name, first, second)
    {
        Id = id;
        Stack = stack;
    }

    public void Bet(int amount)
    {
        if (amount > Stack)
        {
            throw new Exception("amount > Stack");
        }

        Stack -= amount;
        TotalBet += amount;
    }
}
