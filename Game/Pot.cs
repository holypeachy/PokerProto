namespace Game;

public class Pot(int value, List<GamePlayer> players)
{
    public List<GamePlayer> Players { get; set; } = players;
    public int Value { get; set; } = value;
    public List<GamePlayer>? Winners { get; set; } = null;
}