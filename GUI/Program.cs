namespace GUI;
partial class Program
{
    public static void Main()
    {
        InitWindow(ViewX, ViewY, "PokerProto");
        SetTargetFPS(144);

        LoadAssets();

        GameStateDto dto = gameManager.Next(InputAction.Ping, 0);
        currentPlayer = dto.Player;
        minBet = dto.MinBet ?? throw new Exception("Initial dto.MinBet is null.");
        inputBet = minBet;

        Console.WriteLine();
        Console.WriteLine("Game Start");
        Console.WriteLine();
        Console.WriteLine("Pre-Flop:");

        while (!WindowShouldClose())
        {
            BeginDrawing();

            ClearBackground(Color.Pink);

            RenderBackground();

            RenderBoard();

            RenderUI();

            EndDrawing();

            HandleInput();
        }

        CloseWindow();
    }

    static void LoadAssets()
    {
        tableBg = LoadTexture("Assets/table.png");

        cardBack = LoadTexture("Assets/card_back.png");
        suitTextures[CardSuit.Spades] = LoadTexture("Assets/spades.png");
        suitTextures[CardSuit.Clubs] = LoadTexture("Assets/clubs.png");
        suitTextures[CardSuit.Diamonds] = LoadTexture("Assets/diamonds.png");
        suitTextures[CardSuit.Hearts] = LoadTexture("Assets/hearts.png");

        font = LoadFontEx("Assets/CalSans-Regular.ttf", fontSize, null, 0);

        playerProfile = LoadTexture("Assets/player_card.png");
        chips = LoadTexture("Assets/chips.png");
        bottomUI = LoadTexture("Assets/bottom.png");
        buttonTexture = LoadTexture("Assets/button_texture.png");
    }

    static void RenderBackground()
    {
        DrawTextureEx(tableBg, new Vector2(0, 0), 0, (float)ViewY / tableBg.Height, Color.White);
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
        RenderPlayer(gameManager.Players[0]);

        // opponents
        for (int i = 1; i < 5; i++)
        {
            RenderOpponent(gameManager.Players[i], opponentsPos[i - 1]);
        }
    }

    static void RenderUI()
    {
        // status buffer
        DrawTextEx(font, statusBuffer, new Vector2(20, 12), 30, 1, Color.White);

        // bottom ui base
        DrawTextureEx(bottomUI, new Vector2(0, 0), 0, (float)ViewY / bottomUI.Height, Color.White);

        // Current Player
        DrawTextEx(font, currentPlayer is null ? "Turn: None" : $"Turn: {currentPlayer.Name}", new Vector2(50, 825), 70, 1f, Color.White);
        DrawTextEx(font, currentPlayer is null ? "$ None" : $"$ {currentPlayer.Stack}", new Vector2(1350, 12), 70, 1f, Color.White);

        // call or raise
        if (minBet == inputBet)
        {
            buttons[1].Label = "Call";
        }
        else
        {
            buttons[1].Label = "Raise";
        }

        // buttons
        foreach (Button btn in buttons)
        {
            if (btn.Action == ButtonAction.Bet && inputBet == 0) continue;
            if (btn.Action == ButtonAction.Check && minBet != 0) continue;
            DrawTextureRec(buttonTexture, btn.Rectangle, btn.Position, Color.White);
            DrawTextEx(font, btn.Label, btn.Position + new Vector2(20, 10), 60, 1f, Color.White);
        }

        // Raise amount
        DrawTextEx(font, "$ " + inputBet, new Vector2(1200, 830), fontSize, 1, Color.White);
    }

    static void RenderCard(Card card, Vector2 pos)
    {
        DrawTextureEx(suitTextures[card.Suit], pos, 0, 0.20f, Color.White);
        DrawTextEx(font, cardPrintLookUp[card.Rank], new Vector2(pos.X + 15, pos.Y - 5), fontSize, 0, Color.Black);
    }

    static void RenderPlayer(GamePlayer player)
    {
        if (!player.IsAllIn && player.Stack == 0) return;
        // cards
        if (!player.IsFolded)
        {
            RenderCard(player.HoleCards.First, playerCardsPos);
            RenderCard(player.HoleCards.Second, playerCardsPos + new Vector2(70, 0));
        }
        DrawTextEx(font, $"$ {player.Stack}", playerCardsPos + new Vector2(0, 150), 40, 1, Color.White);

        // bet
        if (player.TotalBet != 0)
        {
            DrawTextureEx(chips, playerCardsPos + new Vector2(50, -60), 0, 0.5f, Color.White);
            DrawTextEx(font, player.TotalBet.ToString(), playerCardsPos + new Vector2(110, -60), fontSize - 30, 1, Color.Orange);
        }

        // chances
        if (!player.IsFolded)
        {
            DrawTextEx(font, string.Format("win: {0:0.00}%\n", player.Chances.win * 100) + string.Format("tie: {0:0.00}%", player.Chances.tie * 100), playerCardsPos + new Vector2(-150, 120), fontSize / 2.5f, 1, Color.White);
        }
    }

    static void RenderOpponent(GamePlayer opponent, Vector2 pos)
    {
        if (!opponent.IsAllIn && opponent.Stack == 0) return;
        // cards
        if (opponent.IsFolded) { }
        else if (debugShowOpponentInfo)
        {
            RenderCard(opponent.HoleCards.First, pos + new Vector2(-120, 80));
            RenderCard(opponent.HoleCards.Second, pos + new Vector2(-70, 80));
        }
        else
        {
            DrawTextureEx(cardBack, pos + new Vector2(-120, 80), 0, 0.20f, Color.White);
            DrawTextureEx(cardBack, pos + new Vector2(-70, 80), 0, 0.20f, Color.White);
        }

        // profile
        DrawTextureEx(playerProfile, pos, 0, 0.45f, Color.White);
        DrawTextEx(font, opponent.Name, pos + new Vector2(10, 8), fontSize - 40, 1, Color.White);
        DrawTextEx(font, "$ " + opponent.Stack.ToString(), pos + new Vector2(10, 170), fontSize - 30, 1, Color.White);

        // bets
        if (opponent.TotalBet != 0)
        {
            DrawTextureEx(chips, pos + new Vector2(10, 220), 0, 0.5f, Color.White);
            DrawTextEx(font, opponent.TotalBet.ToString(), pos + new Vector2(70, 230), fontSize - 30, 1, Color.Orange);
        }

        // chances
        if (!opponent.IsFolded && debugShowOpponentInfo)
        {
            DrawTextEx(font, string.Format("win: {0:0.00}%\n", opponent.Chances.win * 100) + string.Format("tie: {0:0.00}%", opponent.Chances.tie * 100), pos + new Vector2(-120, 10), fontSize / 2.5f, 1, Color.White);
        }
    }

    static void HandleInput()
    {
        GameStateDto? dto = null;

        foreach (Button btn in buttons)
        {
            if (btn.IsClickedOn())
            {
                // Console.WriteLine($"{clickCount}-Button Clicked: " + btn.Action);
                // statusBuffer = $"{clickCount++}-Button Clicked: " + btn.Action;

                switch (btn.Action)
                {
                    case ButtonAction.IncreaseBet:
                        if(currentPlayer.Stack >= inputBet + 10)
                            inputBet += 10;
                        break;
                        
                    case ButtonAction.DecreaseBet:
                        if (inputBet > minBet) inputBet -= 10;
                        break;

                    case ButtonAction.Check:
                        if (minBet == 0)
                        {
                            dto = gameManager.Next(InputAction.Check, 0);
                        }
                        break;

                    case ButtonAction.Bet:
                        if (minBet == 0 && inputBet == 0) return;
                        if (inputBet == minBet)
                            dto = gameManager.Next(InputAction.Call, inputBet);
                        else
                            dto = gameManager.Next(InputAction.Raise, inputBet);
                        break;

                    case ButtonAction.Fold:
                        dto = gameManager.Next(InputAction.Fold, 0);
                        break;
                }
            }
        }

        if (dto is null) return;

        switch (dto.Type)
        {
            case StateType.PlayerInput:
                currentPlayer = dto.Player;
                minBet = dto.MinBet ?? throw new Exception();
                inputBet = minBet;
                break;

            case StateType.RoundEndInfo:
                // TODO: Trigger Round End
                break;
            
            case StateType.GameEndinfo:
                // TODO: Trigger Game End
                break;
            default:
                break;
        }
    }

}
/*
TODO:
TODO: Code review.
TODO: Check if at least 2 players can bet, otherwise we skip to showdown.
TODO: Check if there are at least 2 players that can play at start of round, otherwise game over.
TODO: Unit test PotAlgo.
TODO: Improve debug logging.
TODO: Add round logging.
TODO: 

? Future Ideas:
? Instead of creating newTrackers:
    trackers.RemoveAll(t => t.Value == 0);
    return SplitPot(trackers);


* Notes:
* 

* Changes:
* feat: GameManager restructure.
* details: Renamed showAllCards flag to debugShowOpponentInfo; opponent chances of winning are only displayed if this flag is on. GameStateDto is now used to communicate to the GUI the state of the game. StateType indicates what the GameManager needs. PlayerAction is now InputAction. Added Ping action to request the current state of the game (not fully working).
*/