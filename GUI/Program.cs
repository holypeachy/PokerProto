namespace GUI;
partial class Program
{
    public static void Main()
    {
        InitWindow(ViewX, ViewY, "PokerProto");
        SetTargetFPS(144);

        LoadAssets();

        gameManager.Init();

        while (!WindowShouldClose())
        {
            BeginDrawing();

            ClearBackground(Color.Pink);

            DrawText("Hello, world!", 12, 12, 20, Color.Black);

            RenderBackground();

            RenderBoard();

            RenderUI();

            EndDrawing();
        }

        CloseWindow();
    }

    static void LoadAssets()
    {
        tableImg = LoadTexture("Assets/table.png");

        cardBack = LoadTexture("Assets/card_back.png");
        suitTextures[CardSuit.Spades] = LoadTexture("Assets/spades.png");
        suitTextures[CardSuit.Clubs] = LoadTexture("Assets/clubs.png");
        suitTextures[CardSuit.Diamonds] = LoadTexture("Assets/diamonds.png");
        suitTextures[CardSuit.Hearts] = LoadTexture("Assets/hearts.png");

        font = LoadFontEx("Assets/CalSans-Regular.ttf", fontSize, null, 0);

        playerProfile = LoadTexture("Assets/player_card.png");
        chipsImg = LoadTexture("Assets/chips.png");
    }

    static void RenderBackground()
    {
        DrawTextureEx(tableImg, new Vector2(0, 0), 0, (float)ViewY / tableImg.Height, Color.White);
    }

    static void RenderBoard()
    {
        // community
        int x = communityPosX;
        foreach (Card c in gameManager.CommunityCards)
        {
            RenderCard(c, new Vector2(x, communityPosY));
            x += 105;
        }

        // player
        GamePlayer player = gameManager.Players[0];
        RenderCard(player.HoleCards.First, playerCardsPos);
        RenderCard(player.HoleCards.Second, playerCardsPos + new Vector2(70, 0));

        // player bet
        if (player.TotalBet != 0)
        {
            DrawTextureEx(chipsImg, playerCardsPos + new Vector2(50, -60), 0, 0.5f, Color.White);
            DrawTextEx(font, player.TotalBet.ToString(), playerCardsPos + new Vector2(110, -60), fontSize - 30, 1, Color.Orange);
        }

        // opponents
        for (int i = 1; i < 5; i++)
        {
            RenderOpponent(gameManager.Players[i], opponentsPos[i - 1]);
        }
    }

    static void RenderUI()
    {
        // TODO
    }

    static void RenderCard(Card card, Vector2 pos)
    {
        DrawTextureEx(suitTextures[card.Suit], pos, 0, 0.20f, Color.White);
        DrawTextEx(font, cardPrintLookUp[card.Rank], new Vector2(pos.X + 15, pos.Y - 5), fontSize, 0, Color.Black);
    }

    static void RenderOpponent(GamePlayer player, Vector2 pos)
    {
        // cards
        if (showAllCards)
        {
            RenderCard(player.HoleCards.First, pos + new Vector2(-120, 80));
            RenderCard(player.HoleCards.Second, pos + new Vector2(-70, 80));
        }
        else
        {
            DrawTextureEx(cardBack, pos + new Vector2(-120, 80), 0, 0.20f, Color.White);
            DrawTextureEx(cardBack, pos + new Vector2(-70, 80), 0, 0.20f, Color.White);
        }

        // profile
        DrawTextureEx(playerProfile, pos, 0, 0.45f, Color.White);
        DrawTextEx(font, player.Name, pos + new Vector2(10, 8), fontSize - 40, 1, Color.White);
        DrawTextEx(font, "$ " + player.Stack.ToString(), pos + new Vector2(10, 170), fontSize - 30, 1, Color.White);

        // bets
        if (player.TotalBet != 0)
        {
            DrawTextureEx(chipsImg, pos + new Vector2(10, 220), 0, 0.5f, Color.White);
            DrawTextEx(font, player.TotalBet.ToString(), pos + new Vector2(70, 230), fontSize - 30, 1, Color.Orange);
        }
    }
}
/*
TODO:
TODO: Finish GUI.
TODO: Game logic.

* Changes:
* feat: start implementing gui.
*/