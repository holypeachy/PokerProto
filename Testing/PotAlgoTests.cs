namespace Testing;

public class PotAlgoTests
{
    [Fact]
    public void SinglePot_All_bets_are_the_same()
    {
        Deck deck = new();

        List<GamePlayer> players = [
            new("1", deck.NextCard(), deck.NextCard(), 1000),
            new("2", deck.NextCard(), deck.NextCard(), 1000),
            new("3", deck.NextCard(), deck.NextCard(), 1000),
            new("4", deck.NextCard(), deck.NextCard(), 1000),
            new("5", deck.NextCard(), deck.NextCard(), 1000),
        ];

        foreach (GamePlayer p in players)
        {
            p.Bet(100);
        }

        List<Pot> pots = PotAlgo.GetPots(players);
        pots.Count.Should().Be(1);
        pots[0].Players.Count.Should().Be(5);
    }

    [Fact]
    public void TwoPots_One_player_is_all_in()
    {
        Deck deck = new();

        List<GamePlayer> players = [
            new("1", deck.NextCard(), deck.NextCard(), 100),
            new("2", deck.NextCard(), deck.NextCard(), 1000),
            new("3", deck.NextCard(), deck.NextCard(), 1000),
            new("4", deck.NextCard(), deck.NextCard(), 1000),
            new("5", deck.NextCard(), deck.NextCard(), 1000),
        ];

        foreach (GamePlayer p in players)
        {
            p.Bet(200);
        }

        List<Pot> pots = PotAlgo.GetPots(players);
        pots.Count.Should().Be(2);
        pots[0].Value.Should().Be(500);
        pots[1].Value.Should().Be(400);
        
        pots[0].Players.Count.Should().Be(5);
        pots[1].Players.Count.Should().Be(4);
    }

    [Fact]
    public void ThreePots_Two_players_are_all_in()
    {
        Deck deck = new();

        List<GamePlayer> players = [
            new("1", deck.NextCard(), deck.NextCard(), 100),
            new("2", deck.NextCard(), deck.NextCard(), 200),
            new("3", deck.NextCard(), deck.NextCard(), 1000),
            new("4", deck.NextCard(), deck.NextCard(), 1000),
            new("5", deck.NextCard(), deck.NextCard(), 1000),
        ];

        foreach (GamePlayer p in players)
        {
            p.Bet(500);
        }

        List<Pot> pots = PotAlgo.GetPots(players);
        pots.Count.Should().Be(3);
        pots[0].Value.Should().Be(500);
        pots[1].Value.Should().Be(400);
        pots[2].Value.Should().Be(900);

        pots[0].Players.Count.Should().Be(5);
        pots[1].Players.Count.Should().Be(4);
        pots[2].Players.Count.Should().Be(3);
    }

    [Fact]
    public void FourPots_Three_players_are_all_in()
    {
        Deck deck = new();

        List<GamePlayer> players = [
            new("1", deck.NextCard(), deck.NextCard(), 100),
            new("2", deck.NextCard(), deck.NextCard(), 200),
            new("3", deck.NextCard(), deck.NextCard(), 300),
            new("4", deck.NextCard(), deck.NextCard(), 1000),
            new("5", deck.NextCard(), deck.NextCard(), 1000),
        ];

        foreach (GamePlayer p in players)
        {
            p.Bet(800);
        }

        List<Pot> pots = PotAlgo.GetPots(players);
        pots.Count.Should().Be(4);
        pots[0].Value.Should().Be(500);
        pots[1].Value.Should().Be(400);
        pots[2].Value.Should().Be(300);
        pots[3].Value.Should().Be(1000);

        pots[0].Players.Count.Should().Be(5);
        pots[1].Players.Count.Should().Be(4);
        pots[2].Players.Count.Should().Be(3);
        pots[3].Players.Count.Should().Be(2);
    }
}
