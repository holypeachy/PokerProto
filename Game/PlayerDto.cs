namespace Game;
public record PlayerDto
{
    public required GamePlayer Player;
    public required List<PlayerAction> PossibleActions;
    public required int MinBet;
}