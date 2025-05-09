using System.Diagnostics;

namespace Game;
public class Pot(int value, List<GamePlayer> players)
{
    public List<GamePlayer> Players { get; private set; } = players;
    public int Value { get; private set; } = value;
    public List<GamePlayer>? Winners { get; set; } = null;
    public int Split { get; private set; }

    public void PayWinners()
    {
        Debug.Assert(Winners is not null, "Winners should never be null when paying winners. This means we never determined the winners of this pot.");

            Split = Value / Winners.Count;
            foreach (var w in Winners)
            {
                w.AddWinnings(Split);
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