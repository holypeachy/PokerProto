namespace Testing;

public class PotAlgoTests
{
    [Fact]
    public void GetPots()
    {
        Deck deck = new();

        List<GamePlayer> players = [
            new("peach", deck.NextCard(), deck.NextCard(), 1000),
            new("Pepe", deck.NextCard(), deck.NextCard(), 1000),
            new("Doge", deck.NextCard(), deck.NextCard(), 1000),
            new("Top G", deck.NextCard(), deck.NextCard(), 1000),
            new("Waltah", deck.NextCard(), deck.NextCard(), 1000),
        ];

        foreach (GamePlayer p in players)
        {
            p.Bet(100);
        }

        PotAlgo.GetPots(players);
    }
}
