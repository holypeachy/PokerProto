namespace Game;

public class Pot(int total, List<GamePlayer> players)
{
    public int Value { get; set; } = total;
    public List<GamePlayer> Players { get; set; } = players;
}