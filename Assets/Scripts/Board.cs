using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static readonly int boardSize = 9;
    public Tile[,] board = new Tile[boardSize, boardSize];
    public Tile selectedTile;

    public AudioSource boardSound;

    void Awake()
    {
        initializeBoard();
        prepareBoard();
    }

    private void initializeBoard()
    {
        int row = 0, col = 0;
        foreach (Tile tile in GetComponentsInChildren<Tile>())
        {
            if (col == boardSize) { row++; col = 0; }
            tile.setPosition(row, col);
            board[row, col] = tile;
            col++;
        }
        boardSound = GetComponent<AudioSource>();
    }

    private void prepareBoard()
    {
        // Prepare the player's side.
        for (int i = 0; i < boardSize; i++)
        {
            board[2, i].setState(PieceType.Pawn, false, true, false);
        }
        board[1, 1].setState(PieceType.Bishop, false, true, false);
        board[1, 7].setState(PieceType.Rook, false, true, false);
        board[0, 0].setState(PieceType.Lance, false, true, false);
        board[0, 8].setState(PieceType.Lance, false, true, false);
        board[0, 1].setState(PieceType.Knight, false, true, false);
        board[0, 7].setState(PieceType.Knight, false, true, false);
        board[0, 2].setState(PieceType.Silver, false, true, false);
        board[0, 6].setState(PieceType.Silver, false, true, false);
        board[0, 3].setState(PieceType.Gold, false, true, false);
        board[0, 5].setState(PieceType.Gold, false, true, false);
        board[0, 4].setState(PieceType.King, false, true, false);

        // Prepare the enemy's side.
        for (int i = 0; i < boardSize; i++)
        {
            board[6, i].setState(PieceType.Pawn, true, false, false);
        }
        board[7, 7].setState(PieceType.Bishop, true, false, false);
        board[7, 1].setState(PieceType.Rook, true, false, false);
        board[8, 8].setState(PieceType.Lance, true, false, false);
        board[8, 0].setState(PieceType.Lance, true, false, false);
        board[8, 7].setState(PieceType.Knight, true, false, false);
        board[8, 1].setState(PieceType.Knight, true, false, false);
        board[8, 6].setState(PieceType.Silver, true, false, false);
        board[8, 2].setState(PieceType.Silver, true, false, false);
        board[8, 5].setState(PieceType.Gold, true, false, false);
        board[8, 3].setState(PieceType.Gold, true, false, false);
        board[8, 4].setState(PieceType.King, true, false, false);
    }
    public int EvaluateBoard()
    {
        int score = 0;

        foreach (Tile tile in board)
        {
            if (tile.getState() != PieceType.None)
            {
                int pieceValue = GetPieceValue(tile.getState());
                score += tile.isEnemy() ? pieceValue : -pieceValue;
            }
        }

        return score;
    }

    public int GetPieceValue(PieceType piece)
    {
        switch (piece)
        {
            case PieceType.Pawn:
                return 1;
            case PieceType.Rook:
                return 5;
            case PieceType.Knight:
                return 3;
            case PieceType.Bishop:
                return 3;
            case PieceType.Lance:
                return 9;
            case PieceType.King:
                return 1000;
            default:
                return 0;
        }
    }
    public List<Tile[]> GetAllPossibleMoves(bool isEnemy)
    {
        List<Tile[]> possibleMoves = new List<Tile[]>();

        foreach (Tile tile in board)
        {
            if (tile.getState() != PieceType.None && tile.isEnemy() == isEnemy)
            {
                foreach (int[] move in tile.getMoves(board))
                {
                    int x = move[0], y = move[1];
                    if (x >= 0 && x < Board.boardSize && y >= 0 && y < Board.boardSize)
                    {
                        Tile targetTile = board[x, y];
                        if (targetTile.getState() == PieceType.None || targetTile.isEnemy() != isEnemy)
                        {
                            possibleMoves.Add(new Tile[] { tile, targetTile });
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }
}