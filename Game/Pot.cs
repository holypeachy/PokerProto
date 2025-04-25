namespace Game;

public class Pot
{
    public int Value { get; set; }
    public List<GamePlayer> Players { get; set; }

    public Pot(int total, List<GamePlayer> players)
    {
        Value = total;
        Players = players;
    }
}