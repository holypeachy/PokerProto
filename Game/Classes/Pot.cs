namespace Game;

public class Pot(int value, List<GamePlayer> players)
{
    public List<GamePlayer> Players { get; set; } = players;
    public int Value { get; set; } = value;
    public List<GamePlayer>? Winners { get; set; } = null;

    public void PayWinners()
    {
        if (Winners is null) throw new Exception("You fucked up bitch");

        if (Winners.Count == 1)
        {
            Console.WriteLine($"Winner Of Pot ({Value}): {Winners[0].Name} | {Winners[0].WinningHand}");
            Console.WriteLine(Winners[0].Name + $"\nbefore: {Winners[0].Stack}");
            Winners[0].AddWinnings(Value);
            Console.WriteLine("after: " + Winners[0].Stack);
        }
        else
        {
            int split = Value / Winners.Count;
            Console.WriteLine($"Winners of Pot ({Value}): ");
            foreach (var w in Winners)
            {
                Console.WriteLine(w.Name + $": {split} | {w.WinningHand}");
                Console.WriteLine(w.Name + $"\nbefore: {Winners[0].Stack} ");
                w.AddWinnings(split);
                Console.WriteLine("after: " + w.Stack);
            }
        }
    }

    public override string ToString()
    {
        string players = "";
        foreach (var item in Players)
        {
            players += item.Name + "|";
        }
        return $"players: {players}\nvalue: {Value}";
    }
}