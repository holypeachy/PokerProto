namespace Game;
public class PlayerTable
{
    private readonly LinkedList<GamePlayer> _circle;
    public LinkedListNode<GamePlayer> Current { get; private set; }

    public PlayerTable(List<GamePlayer> players)
    {
        _circle = new LinkedList<GamePlayer>(players);
        Current = _circle.First ?? throw new Exception();
    }

    public GamePlayer GetNext()
    {
        do
        {
            Current = Current.Next ?? _circle.First ?? throw new Exception();
        }
        while (Current.Value.IsFolded || Current.Value.IsAllIn || (!Current.Value.IsAllIn && Current.Value.Stack == 0));

        return Current.Value;
    }

    public void SetCurrent(GamePlayer player)
    {
        LinkedListNode<GamePlayer>? toFind = _circle.Find(player) ?? throw new Exception("player not found in linked list");
        Current = toFind;
    }
}
