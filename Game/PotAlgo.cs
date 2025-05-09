namespace Game;
public static class PotAlgo
{
    class ChipTracker(GamePlayer owner, int value, bool isFolded)
    {
        public GamePlayer Owner { get; } = owner;
        public int Value { get; set; } = value;
        public bool IsFolded { get; } = isFolded;

        public override string ToString()
        {
            return $"owner: {Owner.Name} | value: {Value} | folded: {IsFolded}";
        }
    }

    public static List<Pot> GetPots(List<GamePlayer> players)
    {
        List<ChipTracker> trackers = [];
        foreach (GamePlayer p in players)
        {
            if (p.TotalBet != 0)
            {
                trackers.Add(new ChipTracker(p, p.TotalBet, p.IsFolded));
            }
        }

        Console.WriteLine("\nTrackers:");
        foreach (ChipTracker t in trackers)
        {
            Console.WriteLine(t);
        }
        Console.WriteLine();

        return SplitPot(trackers);
    }

    private static List<Pot> SplitPot(List<ChipTracker> trackers)
    {
        // end condition
        if (trackers.Count == 0) return [];

        // pot splitting logic
        Pot pot;
        int min = GetMin(trackers);
        int potTotal = 0;
        int foldedTotal = 0;
        List<GamePlayer> potPlayers = [];

        // loop through trackers and decrease value
        foreach (ChipTracker t in trackers)
        {
            if (t.IsFolded)
            {
                if (t.Value <= min)
                {
                    foldedTotal += t.Value;
                    t.Value = 0;
                }
                else
                {
                    foldedTotal += min;
                    t.Value -= min;
                }
            }
            else
            {
                potPlayers.Add(t.Owner);
                potTotal += min;
                t.Value -= min;
            }
        }

        pot = new Pot(potTotal + foldedTotal, potPlayers);

        // prepare trackers for next recursion
        trackers.RemoveAll(t => t.Value == 0);

        // we combine all the pots
        List<Pot> pots = [pot];
        pots.AddRange(SplitPot(trackers));
        return pots;
    }

    private static int GetMin(List<ChipTracker> trackers)
    {
        int min = 0;
        ChipTracker current;
        for (int i = 0; i < trackers.Count; i++)
        {
            current = trackers[i];
            if (i == 0) min = current.Value;
            else
            {
                if (!current.IsFolded && current.Value < min) min = current.Value;
            }
        }

        return min;
    }

}