namespace GUI;
partial class Program
{
    const int ViewX = 1600;
    const int ViewY = 900;

    static readonly Dictionary<CardSuit, Texture2D> suitTextures = new();
    static Texture2D tableBg;
    static Texture2D playerProfile;
    static Texture2D chips;
    static Texture2D cardBack;
    static Texture2D bottomUI;
    static Texture2D buttonTexture;
    static Font font;
    static readonly int fontSize = 70;

    static Vector2 playerCardsPos = new(680, 600);

    static readonly List<Vector2> opponentsPos = new()
    {
        new Vector2(280, 450),
        new Vector2(450, 50),
        new Vector2(980, 50),
        new Vector2(1200, 450),
    };

    const int communityPosY = 350;
    const int communityPosX = 550;

    static readonly Dictionary<int, string> cardPrintLookUp = new()
    {
        {1, "A"}, {2, "2"}, {3, "3"}, {4, "4"}, {5, "5"}, {6, "6"}, {7, "7"}, {8, "8"}, {9, "9"}, {10, "10"},{11, "J"}, {12, "Q"}, {13, "K"}, {14, "A"}
    };

    static List<Button> buttons = new()
    {
        new Button("Chek", 450, 820, 150, 80, ButtonAction.Check),
        new Button("Call", 610, 820, 150, 80, ButtonAction.Bet),
        new Button("Fold", 770, 820, 150, 80, ButtonAction.Fold),
        new Button("  -", 1100, 820, 80, 80, ButtonAction.DecreaseBet),
        new Button(" +", 1400, 820, 80, 80, ButtonAction.IncreaseBet),
    };

    static int clickCount = 1;
    static string statusBuffer = "game status";
    

    static bool showAllCards = true;
    static readonly GameManager gameManager = new();
    static GamePlayer? currentPlayer = null;
    static int minBet = 10;
    static int inputBet = 10;
}