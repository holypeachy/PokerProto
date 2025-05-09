using System.Diagnostics;

namespace Game;
public class Pot(int value, List<GamePlayer> players, List<GamePlayer>? winners = null)
{
    public List<GamePlayer> Players { get; private set; } = players;
    public int Value { get; private set; } = value;
    public List<GamePlayer>? Winners { get; set; } = winners;

    public void PayWinners()
    {
        Debug.Assert(Winners is not null, "Winners should never be null when paying winners. This means we never determined the winners of this pot.");

            int split = Value / Winners.Count;
            foreach (var w in Winners)
            {
                w.AddWinnings(split);
            }
        }

    public override string ToString()
    {
        string players = "| ";
        foreach (var item in Players)
        {
            players += item.Name + " | ";
        }

        string wString = string.Empty;
        if (Winners is not null)
        {
            foreach (GamePlayer w in Winners)
            {
                wString += $"\t{w.Name} ({Value / Winners.Count()}) | {w.Stack} => {w.Stack + Value / Winners.Count()} | {w.WinningHand}\n";
            }
        }

        return $"players ({Players.Count}): {players}\nvalue: {Value}\nwinner(s):\n{wString}";
    }
}