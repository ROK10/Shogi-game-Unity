## Overview

This is a university graduation project aimed at developing a computer version of the Japanese strategic board game Shogi, implemented using Unity and C#. The game includes artificial intelligence (AI) algorithms to challenge the player in both offensive and defensive strategies.

## Features

- 🎮 Fully functional Shogi game board and rule implementation
- 🧠 AI opponent using Minimax algorithm with Alpha-Beta pruning
- 🤖 Basic and advanced AI levels for different difficulties
- 📈 Performance-tested and optimized AI decision logic
- 💡 Designed for educational and entertainment purposes

## Technologies Used

- **Unity** – Game development engine
- **C#** – Programming language for game logic
- **Minimax with Alpha-Beta Pruning** – Main AI algorithm
- **Custom heuristic evaluation function** – For AI decision making

## Game Architecture

- `Piece.cs`: Logic and data for each Shogi piece
- `Tile.cs`: Logic for each board tile
- `Board.cs`: Manages game state, board structure and possible moves
- `GameController.cs`: Controls game loop, AI turns, and user input

## AI Implementation

Two AI levels are implemented:
1. **Basic AI** – Randomized strategy with simple prioritization
2. **Advanced AI** – Uses Minimax with Alpha-Beta pruning and custom evaluation metrics to determine the best move

## Performance

- Depth 1 (no pruning): ~0.0005s
- Depth 4 with Alpha-Beta pruning: ~5s (average)
- Without pruning, high depths result in exponential computation time

## How to Run

### Option 1: Run the compiled game

1. Download the folder **`Compiled Shogi game`** from the project repository.
2. Open the folder and run the `Shogi.exe` file.

### Option 2: Open in Unity

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/shogi-ai.git
