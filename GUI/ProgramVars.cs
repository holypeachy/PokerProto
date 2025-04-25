namespace GUI;
partial class Program
{
    const int ViewX = 1600;
    const int ViewY = 900;

    static readonly Dictionary<CardSuit, Texture2D> suitTextures = new();
    static Texture2D tableImg;
    static Texture2D playerProfile;
    static Texture2D chipsImg;
    static Texture2D cardBack;
    static Font font;
    static readonly int fontSize = 70;

    static readonly Dictionary<int, string> cardPrintLookUp = new()
    {
        {1, "A"}, {2, "2"}, {3, "3"}, {4, "4"}, {5, "5"}, {6, "6"}, {7, "7"}, {8, "8"}, {9, "9"}, {10, "10"},{11, "J"}, {12, "Q"}, {13, "K"}, {14, "A"}
    };

    const int communityPosY = 350;
    const int communityPosX = 550;

    static bool showAllCards = true;
    static readonly GameManager gameManager = new();

    static readonly List<Vector2> opponentsPos = new()
    {
        new Vector2(280, 450),
        new Vector2(450, 50),
        new Vector2(980, 50),
        new Vector2(1150, 450),
    };
    static Vector2 playerCardsPos = new(680, 600);


}