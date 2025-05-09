namespace Game;
public record GameStateDto
{
    required public StateType Type;
    public GamePlayer? Player;
    public int? MinBet;
    public List<Pot>? Pots;
}