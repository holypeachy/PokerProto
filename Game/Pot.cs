namespace Game;

public class Pot
{
    public int Total { get; set; }
    public List<Player> Players { get; set; }

    public Pot(int total, List<Player> players)
    {
        Total = total;
        Players = players;
    }
}