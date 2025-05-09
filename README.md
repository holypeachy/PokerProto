# 🍑 PokerProto / PokerAI - Texas Hold 'em game prototype.
#### A basic Texas Hold 'em game that implements my [PokerAlgo](https://github.com/holypeachy/PokerAlgo) for determining winners and calculating winning chances. The project is meant to help me figure out the game logic as well as build a framework I can use to create an AI.
- The game logic for the poker game I am working on.
- Will implement a Poker AI using a genetic algorithm.
- Will also implement a logging system to track games played (Players, cards, moves, bets, outcomes).
## Features
- Implements my [PokerAlgo](https://github.com/holypeachy/PokerAlgo).
- GUI of my own design using [raylib-cs](https://github.com/raylib-cs/raylib-cs).
- PotAlgo: A recursive algorithm I came up with for pot splitting.
## Quick Log and Ideas
- 5/9/2025: I restructured GameManager, it should be more decoupled now. The new GameStateDto is tiny but provides all the information needed by GUI or any outside code to either provide input or be notified of the round or game ending. I started to fix the main edge cases but smaller cases arose and the logic is too complex for me to keep track of. I'll write tests for the GameManager and I'll use a TDD approach from here.
- 5/5/2025: I Implemented the GUI and the basic game betting logic, they have also been wired. It's not super polished yet (no showdown screen on the GUI), I'll do that after the game logic is fully finished. I would like to add better debug logging and I need to write unit tests for PotAlgo and GameManager.

## Showcase:
### Initial game screen.
I have a flag to show all opponent cards. For now, the GUI takes input for all the players since there is no AI yet.  
The win and tie chances for each player are calculated asynchronously.

![image](https://github.com/user-attachments/assets/b459dc7c-6b41-4944-a754-b6a476af79e8)

![image](https://github.com/user-attachments/assets/100d1563-c7ec-4924-8f39-cc4e14894023)

### Showdown.
There is only one pot in this example, but the output will reflect if there are multiple pots.

![image](https://github.com/user-attachments/assets/4bd229d7-8df8-4fca-905e-8e47930fcdfc)

![image](https://github.com/user-attachments/assets/720e8830-c89e-46b9-a4d0-b3710026ec4d)

