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
        board = new Tile[9, 9];
    }

    void Start()
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
    public Board(Board existingBoard)
    {
        int boardSize = 9;
        board = new Tile[boardSize, boardSize];

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                Tile existingTile = existingBoard.board[row, col];
                board[row, col] = existingTile.CreateTileFromExisting(existingTile);

                PieceType state = existingTile.getState();
                bool enemy = existingTile.isEnemy();
                bool player = existingTile.isPlayer();
                bool promoted = existingTile.isPromoted();

                board[row, col].setState(state, enemy, player, promoted);
            }
        }
    }
    public Board CloneWithMove(Tile startTile, Tile endTile)
    {
        // Create a new GameObject and add the Board component
        GameObject newBoardObject = new GameObject("ClonedBoard");
        Board clonedBoard = newBoardObject.AddComponent<Board>();

        // Initialize the cloned board
        clonedBoard.board = new Tile[boardSize, boardSize];
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                clonedBoard.board[row, col] = Instantiate(board[row, col], newBoardObject.transform);
                clonedBoard.board[row, col].setPosition(row, col);
                MeshRenderer meshRenderer = clonedBoard.board[row, col].GetComponentInChildren<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }
            }
        }

        // Get the start and end positions of the move
        int startRow = startTile.getRow();
        int startCol = startTile.getCol();
        int endRow = endTile.getRow();
        int endCol = endTile.getCol();

        // Apply the move on the cloned board
        PieceType movingPieceType = clonedBoard.board[startRow, startCol].getState();
        bool movingPieceIsEnemy = clonedBoard.board[startRow, startCol].isEnemy();
        bool movingPieceisPlayer = clonedBoard.board[startRow, startCol].isPlayer();
        bool movingPieceisPromoted = clonedBoard.board[startRow, startCol].isPromoted();
        clonedBoard.board[startRow, startCol].setState2(PieceType.None, false, false, false);
        clonedBoard.board[endRow, endCol].setState2(movingPieceType, movingPieceIsEnemy, movingPieceisPlayer, movingPieceisPromoted);

        clonedBoard.initializeBoard();

        return clonedBoard;
    }
}
public class TestBoard
{
    public static readonly int boardSize = 9;
    public TestTile[,] testBoard = new TestTile[boardSize, boardSize];
    public TestTile selectedTile;



    public TestBoard createNewBoard(Board existingBoard)
    {
        TestBoard newTestBoard = new();
        int boardSize = 9;
        newTestBoard.testBoard = new TestTile[boardSize, boardSize];
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                //prepareBoard();
                Tile existingTile = existingBoard.board[row, col];
                newTestBoard.testBoard[row, col] = new TestTile();
                newTestBoard.testBoard[row, col].setState(existingTile.getState(), existingTile.isEnemy(), existingTile.isPlayer(), existingTile.isPromoted());
            }
        }
        return newTestBoard;
    }
    public TestBoard createNewBoard(TestBoard existingBoard)
    {
        TestBoard newTestBoard = new();
        int boardSize = 9;
        newTestBoard.testBoard = new TestTile[boardSize, boardSize];
        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                //prepareBoard();
                TestTile existingTile = new();
                existingTile.setState(existingBoard.testBoard[row, col].getState(), existingBoard.testBoard[row, col].isEnemy(), existingBoard.testBoard[row, col].isPlayer(), existingBoard.testBoard[row, col].isPromoted());
                newTestBoard.testBoard[row, col] = new TestTile();
                newTestBoard.testBoard[row, col].setState(existingTile.getState(), existingTile.isEnemy(), existingTile.isPlayer(), existingTile.isPromoted());
            }
        }
        return newTestBoard;
    }
    private void prepareBoard()
    {
        // Prepare the player's side.
        for (int i = 0; i < boardSize; i++)
        {
            testBoard[2, i].setState(PieceType.Pawn, false, true, false);
        }
        testBoard[1, 1].setState(PieceType.Bishop, false, true, false);
        testBoard[1, 7].setState(PieceType.Rook, false, true, false);
        testBoard[0, 0].setState(PieceType.Lance, false, true, false);
        testBoard[0, 8].setState(PieceType.Lance, false, true, false);
        testBoard[0, 1].setState(PieceType.Knight, false, true, false);
        testBoard[0, 7].setState(PieceType.Knight, false, true, false);
        testBoard[0, 2].setState(PieceType.Silver, false, true, false);
        testBoard[0, 6].setState(PieceType.Silver, false, true, false);
        testBoard[0, 3].setState(PieceType.Gold, false, true, false);
        testBoard[0, 5].setState(PieceType.Gold, false, true, false);
        testBoard[0, 4].setState(PieceType.King, false, true, false);

        // Prepare the enemy's side.
        for (int i = 0; i < boardSize; i++)
        {
            testBoard[6, i].setState(PieceType.Pawn, true, false, false);
        }
        testBoard[7, 7].setState(PieceType.Bishop, true, false, false);
        testBoard[7, 1].setState(PieceType.Rook, true, false, false);
        testBoard[8, 8].setState(PieceType.Lance, true, false, false);
        testBoard[8, 0].setState(PieceType.Lance, true, false, false);
        testBoard[8, 7].setState(PieceType.Knight, true, false, false);
        testBoard[8, 1].setState(PieceType.Knight, true, false, false);
        testBoard[8, 6].setState(PieceType.Silver, true, false, false);
        testBoard[8, 2].setState(PieceType.Silver, true, false, false);
        testBoard[8, 5].setState(PieceType.Gold, true, false, false);
        testBoard[8, 3].setState(PieceType.Gold, true, false, false);
        testBoard[8, 4].setState(PieceType.King, true, false, false);
    }

    public List<TestTile[]> GetAllPossibleMoves(bool isEnemy)
    {
        List<TestTile[]> possibleMoves = new List<TestTile[]>();

        foreach (TestTile tile in testBoard)
        {
            if (tile.getState() != PieceType.None && tile.isEnemy() == isEnemy)
            {
                foreach (int[] move in tile.getMoves(testBoard))
                {
                    int x = move[0], y = move[1];
                    if (x >= 0 && x < TestBoard.boardSize && y >= 0 && y < TestBoard.boardSize)
                    {
                        TestTile targetTile = testBoard[x, y];
                        if (targetTile.getState() == PieceType.None || targetTile.isEnemy() != isEnemy)
                        {
                            possibleMoves.Add(new TestTile[] { tile, targetTile });
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    public void CloneMove(Tile startTile, Tile endTile)
    {
        testBoard[endTile.getRow(), endTile.getCol()].setState(startTile.getState(), startTile.isEnemy(), startTile.isPlayer(), startTile.isPromoted());
        testBoard[startTile.getRow(), startTile.getCol()].setState(PieceType.None, false, false, false);

    }
    public void CloneMove(TestTile startTile, TestTile endTile)
    {
        testBoard[endTile.getRow(), endTile.getCol()].setState(startTile.getState(), startTile.isEnemy(), startTile.isPlayer(), startTile.isPromoted());
        testBoard[startTile.getRow(), startTile.getCol()].setState(PieceType.None, false, false, false);

    }
}