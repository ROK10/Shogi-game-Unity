﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{

    //Assets and part of the scripts are taken from the project https://github.com/NicholasSebastian/Shogi-Game.
    //Most of the code has been rewritten and completed
    //Artificial intelligence has been implemented using the minimax algorithm

    public static bool game = true;
    public static bool multiplayer = false;
    public static bool playersTurn = true;

    public static bool playersWin = false;
    public static string endComment;

    public static float enemyOffensiveLevel = 0.8f;
    public static float EnemyMinThinkTime = 2.0f;

    public static int searchDepth = 1;

    private int turnCounter;
    List<int[]> validMoves = new();
    List<int[]> validKingMoves = new();
    List<int[]> validEnemyMoves = new();
    List<int[]> validEnemyKingMoves = new();

    GameMode gameMode;

    private static GameObject boardPrefab;
    public static GameObject facePrefab;
    public static GameObject piecePrefab;
    private Board board;
    public enum GameMode
    {
        Mode1,
        Mode2,
        Mode3,
    }
    void Awake()
    {
        boardPrefab = (GameObject)Resources.Load("Board");
        facePrefab = (GameObject)Resources.Load("Face");
        piecePrefab = (GameObject)Resources.Load("Piece");
    }

    void Start()
    {
        MainMenu mainMenu = FindObjectOfType<MainMenu>();
        gameMode = MainMenu.selectedGameMode;
        board = Instantiate(boardPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0))
                .GetComponent<Board>();
        turnCounter = 0;

        if (multiplayer == false)
            StartCoroutine(GameLoop());
        else if (multiplayer)
            Debug.Log("Two player local game not yet implemented.");
    }

    private IEnumerator GameLoop()
    {
        while (game)
        {
            if (playersTurn)
                yield return
                StartCoroutine(PlayerControls());
            else
                yield return
                StartCoroutine(EnemyControls());
            turnCounter++;
        }
    }

    private void endGame(DeathType deathType, bool playerWins)
    {
        game = false;
        playersWin = playerWins;
        switch (deathType)
        {
            case DeathType.Checkmate:
                endComment = "Checkmate!";
                break;

            case DeathType.KingKilled:
                endComment = "King has been killed!";
                break;

            case DeathType.NoMoves:
                endComment = "No moves left!";
                break;

            case DeathType.NoPieces:
                endComment = "No pieces have been killed!";
                break;

            default:
                break;
        }
        StartCoroutine(waitForGameEnd());
    }

    private IEnumerator waitForGameEnd()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(0);
    }

    private IEnumerator PlayerControls()
    {
        if (gameMode == GameMode.Mode3)
        {
            float startTime = Time.time;
            //playerStatus();
            yield return new WaitForSeconds(0.2f);

            while (playersTurn == true)
            {
                yield return new WaitForSeconds(
                    Time.time - startTime < EnemyMinThinkTime ?
                    0.2f : 0.0f
                );
                // Initialize lists for the different targets.
                List<Tile[]> playerTargets = new();
                List<Tile[]> spaceTargets = new();
                List<Tile[]> pawnSpaceTargets = new();

                int numberOfPieces = 0;
                bool kingInDanger = false;
                Tile kingPiece = null;
                // For all the possible targets of all enemy pieces present:
                foreach (Tile currentTile in board.board)
                {
                    if (currentTile.getState() != PieceType.None && currentTile.isPlayer())
                    {
                        numberOfPieces++;
                        foreach (int[] possibleMove in currentTile.getMoves(board.board))
                        {
                            // Skip invalid moves / moves that lead to another enemy piece.
                            int x = possibleMove[0], y = possibleMove[1];
                            if (x < 0 || x >= Board.boardSize || y < 0 || y >= Board.boardSize
                                || board.board[x, y].isPlayer()) continue;

                            // Add these moves to a list for checkmate checking later.
                            this.validEnemyMoves.Add(possibleMove);
                            if (currentTile.getState() == PieceType.King)
                                this.validEnemyKingMoves.Add(possibleMove);

                            Tile targetTile = board.board[x, y];
                            // If the target is the player's piece:
                            if (targetTile.getState() != PieceType.None &&
                                targetTile.isPlayer() == false)
                            {
                                // If the target is a king, then attack and stop checking.
                                if (targetTile.getState() == PieceType.King)
                                {
                                    yield return StartCoroutine(pieceMovement(currentTile, targetTile));
                                    endGame(DeathType.KingKilled, false);
                                    break;
                                }
                                // add to a list of playerTargets.
                                else playerTargets.Add(new Tile[2] { currentTile, targetTile });
                            }
                            // If the target is empty, add to a list of spaceTargets.
                            else if (targetTile.getState() == PieceType.None)
                            {
                                spaceTargets.Add(new Tile[2] { currentTile, targetTile });
                                if (currentTile.getState() == PieceType.Pawn)
                                    pawnSpaceTargets.Add(new Tile[2] { currentTile, targetTile });
                            }
                        }

                        // If the piece is a king, and it is in range of enemy's attack, then move.
                        if (currentTile.getState() == PieceType.King)
                        {
                            kingPiece = currentTile;
                            foreach (int[] possiblePlayerMove in this.validMoves)
                            {
                                if (currentTile.getRow() == possiblePlayerMove[0] &&
                                    currentTile.getCol() == possiblePlayerMove[1])
                                    kingInDanger = true;
                            }
                        }
                    }
                    if (game == false) break;
                }
                // If it turns out the enemy has no pieces, declare the loss.
                if (numberOfPieces == 0) endGame(DeathType.NoPieces, true);

                Tile[] moveTarget = new Tile[2];
                // If the king is within range of the player's attack, move the king.
                if (kingInDanger)
                    moveTarget = (
                        playerTargets.Count > 0 ?
                        playerTargets[
                            playerTargets.FindIndex(
                            x => x[0].getState() == PieceType.King
                        )] :
                        spaceTargets[
                            spaceTargets.FindIndex(
                            x => x[0].getState() == PieceType.King
                        ) + 1]
                    );

                // If there is/are enemy piece(s) in range of attack, high chance to attack:
                else if (playerTargets.Count > 0 && Random.value < enemyOffensiveLevel)
                    moveTarget = playerTargets[Random.Range(0, playerTargets.Count - 1)];

                // If there is/are empty space(s) in range of movement, move:
                else if (spaceTargets.Count > 0)
                    moveTarget = (
                        turnCounter < enemyOffensiveLevel * 20 &&
                        Random.value < enemyOffensiveLevel ?
                        pawnSpaceTargets[Random.Range(0, pawnSpaceTargets.Count - 1)] :
                        spaceTargets[Random.Range(0, spaceTargets.Count - 1)]
                    );

                // Else declare that there are no available moves and lose.
                else endGame(DeathType.NoMoves, true);

                // Check if the enemy has been checkmated.
                checkmateStatus(kingPiece, this.validEnemyKingMoves, this.validMoves, true);

                // Finally execute the enemy's move.
                Tile enemySelectedTile = moveTarget[0];
                Tile enemyTargetedTile = moveTarget[1];
                yield return StartCoroutine(pieceMovement(enemySelectedTile, enemyTargetedTile));
            }
        }
        else 
        {
            playerStatus();
            while (playersTurn)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        Transform clicked = hit.transform;
                        Tile clickedTile = clicked.GetComponent<Tile>();
                        if (clicked.CompareTag("Tile"))
                            if (clickedTile.getState() != PieceType.None)
                                if (clickedTile.isEnemy() == false)
                                {
                                    if (board.selectedTile)
                                        deselectPiece();
                                    board.selectedTile = clickedTile;
                                    selectPiece();
                                }
                                else movePiece(clickedTile);
                            else movePiece(clickedTile);
                        else deselectPiece();
                    }
                    else deselectPiece();
                }
                yield return null;
            }
        }
    }

    int turn = 0;
    TimeSpan totalTime = TimeSpan.Zero;

    public IEnumerator EnemyControls()
    {
        float startTime = Time.time;
        //playerStatus();
        yield return new WaitForSeconds(0.0f);

        while (!playersTurn)
        {
            yield return new WaitForSeconds(
            Time.time - startTime < EnemyMinThinkTime ?
            0.0f : 0.0f
        );

            List<Tile[]> allPossibleEnemyMoves = board.GetAllPossibleMoves(true);
            if (allPossibleEnemyMoves.Count == 0)
            {
                endGame(DeathType.NoMoves, true);
                yield break;
            }
            Stopwatch stopwatch = new Stopwatch();


            Tile[] bestMove = null;
            int maxDepth;
            int bestMoveValue = int.MinValue;
            if (gameMode == GameMode.Mode2)
            {
                maxDepth = 3;
            }
            else
            {
                maxDepth = 2;
            }
            //Debug.Log($"Start enemy turn time " + System.DateTime.Now.ToLongTimeString() + " " + turn);
            //for (int currentDepth = 1; currentDepth <= maxDepth; currentDepth++)
            //{
            stopwatch.Start();
            foreach (Tile[] move in allPossibleEnemyMoves)
                {

                    TestBoard newBoard = new();
                    TestBoard clonedBoard = newBoard.createNewBoard(board);

                //Debug.Log($"Lance ? 2222: + " + clonedBoard.testBoard[0, 0].getState());
                //Debug.Log($"Depth" + (maxDepth+1));
                clonedBoard.CloneMove(move[0], move[1]);
                    //int boardValue = Minimax(clonedBoard, currentDepth, false, int.MinValue, int.MaxValue);
                    int boardValue = Minimax(clonedBoard, maxDepth, false, int.MinValue, int.MaxValue);
                    if (boardValue >= bestMoveValue)
                    {
                        bestMoveValue = boardValue;
                        bestMove = move;
                    }
                    //Debug.Log($"Maximizing player, boardValue: {boardValue}, bestValue: {bestMoveValue}");
                }
            //}

            Tile enemySelectedTile = bestMove[0];
            Tile enemyTargetedTile = bestMove[1];

            stopwatch.Stop();

            TimeSpan moveTime = stopwatch.Elapsed;

            totalTime += moveTime;
            turn++;
            //Debug.Log($"Time spent on move: {moveTime.TotalSeconds} s");

            stopwatch.Reset();

            TimeSpan averageTime = TimeSpan.FromSeconds((double)totalTime.TotalSeconds / turn);

            // Output the average time to the debug log
            //Debug.Log($"Average time spent on moves: {averageTime.TotalSeconds} s");

            if (enemyTargetedTile.getState() == PieceType.King)
            {
                yield return StartCoroutine(pieceMovement(enemySelectedTile, enemyTargetedTile));
                endGame(DeathType.KingKilled, false);
                break;
            }

            yield return StartCoroutine(pieceMovement(enemySelectedTile, enemyTargetedTile));
        }
    }

    public int Minimax(TestBoard board, int depth, bool isMaximizingPlayer, int alpha, int beta)
    {

        if (depth == 0)
        {
            return EvaluateBoard(board, isMaximizingPlayer);
        }

        int bestValue;

        if (isMaximizingPlayer)
        {
            bestValue = int.MinValue;
            List<TestTile[]> possibleMoves = board.GetAllPossibleMoves(true);
            foreach (TestTile[] move in possibleMoves)
            {
                TestBoard newBoard = new();
                TestBoard clonedBoard = newBoard.createNewBoard(board);
                clonedBoard.CloneMove(move[0], move[1]);
                int boardValue = Minimax(clonedBoard, depth - 1, !isMaximizingPlayer, alpha, beta);
                bestValue = Math.Max(bestValue, boardValue);
                //Debug.Log($"Maximizing player, boardValue: {boardValue}, bestValue: {bestValue}");

                alpha = Math.Max(alpha, bestValue);

                if (beta <= alpha)
                {
                    break; // Бета відсікання
                }
            }
        }
        else
        {
            bestValue = int.MaxValue;
            List<TestTile[]> possibleMoves = board.GetAllPossibleMoves(false);
            foreach (TestTile[] move in possibleMoves)
            {

                TestBoard newBoard = new();
                TestBoard clonedBoard = newBoard.createNewBoard(board);
                clonedBoard.CloneMove(move[0], move[1]);
                int boardValue = Minimax(clonedBoard, depth - 1, !isMaximizingPlayer, alpha, beta);
                bestValue = Math.Min(bestValue, boardValue);
                //Debug.Log($"Minimizing player, boardValue: {boardValue}, bestValue: {bestValue}");

                beta = Math.Min(beta, bestValue);

                if (beta <= alpha)
                {
                    break; // Альфа відсікання
                }
            }
        }

        return bestValue;
    }
    public int EvaluateBoard(TestBoard board, bool isMaximizingPlayer)
    {
        int score = 0;

        // Iterate through all tiles on the board
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                TestTile tile = board.testBoard[row, col];
                PieceType pieceType = tile.getState();
                //Debug.Log(tile.getState());
                //PieceType pieceType = PieceType.Knight;
                int pieceValue = 0;

                // Assign a value to each piece type
                switch (pieceType)
                {
                    case PieceType.Pawn:
                        pieceValue = 10;
                        break;
                    case PieceType.Knight:
                        pieceValue = 60;
                        break;
                    case PieceType.Bishop:
                        pieceValue = 130;
                        break;
                    case PieceType.Rook:
                        pieceValue = 150;
                        break;
                    case PieceType.Lance:
                        pieceValue = 50;
                        break;
                    case PieceType.Silver:
                        pieceValue = 80;
                        break;
                    case PieceType.Gold:
                        pieceValue = 90;
                        break;
                    case PieceType.King:
                        pieceValue = 900;
                        break;
                    case PieceType.None:
                        pieceValue = 0;
                        break;
                    default:
                        pieceValue = 0;
                        break;
                }

                // Add or subtract the value from the total score depending on the piece's color
                if (pieceType != PieceType.None)
                {
                    int positionalScore = positionalScores[pieceType][row, col];
                    //Debug.Log((tile.isEnemy() == isMaximizingPlayer) ? -pieceValue - positionalScore : pieceValue + positionalScore);
                    score += (!tile.isEnemy()) ? -pieceValue - positionalScore : pieceValue + positionalScore;
                    //score += (tile.isEnemy()) ? pieceValue  : -pieceValue;
                }
            }
        }

        //Debug.Log($"Returning board score: {score} (isMaximizingPlayer: {isMaximizingPlayer})");
        return score;
    }

    private static readonly Dictionary<PieceType, int[,]> positionalScores = new Dictionary<PieceType, int[,]>
    {
        { PieceType.Pawn, new int[,] {
            { 0,  0,  0,  0,  0,  0,  0,  0,  0 },
            { 5,  5,  5,  5,  5,  5,  5,  5,  5 },
            { 1,  1,  2,  3,  3,  2,  1,  1,  1 },
            { 0,  0,  0,  0,  0,  0,  0,  0,  0 },
            { 0,  0,  0,  0,  0,  0,  0,  0,  0 },
            { 0,  0,  0,  0,  0,  0,  0,  0,  0 },
            { 1,  1,  2,  3,  3,  2,  1,  1,  1 },
            { 5,  5,  5,  5,  5,  5,  5,  5,  5 },
            { 0,  0,  0,  0,  0,  0,  0,  0,  0 }
        } },
        {
        PieceType.King,
        new int[,]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        } },
        { 
        PieceType.Rook, new int[,] {
        { 2,  3,  2,  2,  2,  2,  2,  3,  2 },
        { 3,  4,  3,  3,  3,  3,  3,  4,  3 },
        { 2,  3,  2,  2,  2,  2,  2,  3,  2 },
        { 2,  3,  2,  2,  2,  2,  2,  3,  2 },
        { 2,  3,  2,  2,  2,  2,  2,  3,  2 },
        { 2,  3,  2,  2,  2,  2,  2,  3,  2 },
        { 2,  3,  2,  2,  2,  2,  2,  3,  2 },
        { 3,  4,  3,  3,  3,  3,  3,  4,  3 },
        { 2,  3,  2,  2,  2,  2,  2,  3,  2 }
    } },
        {
        PieceType.Gold, new int[,] {
        { 1,  1,  1,  1,  1,  1,  1,  1,  1 },
        { 1,  2,  2,  2,  2,  2,  2,  2,  1 },
        { 1,  2,  3,  3,  3,  3,  3,  2,  1 },
        { 1,  2,  3,  4,  4,  4,  3,  2,  1 },
        { 1,  2,  3,  4,  5,  4,  3,  2,  1 },
        { 1,  2,  3,  4,  4,  4,  3,  2,  1 },
        { 1,  2,  3,  3,  3,  3,  3,  2,  1 },
        { 1,  2,  2,  2,  2,  2,  2,  2,  1 },
        { 1,  1,  1,  1,  1,  1,  1,  1,  1 }
        }
    },
{
        PieceType.Silver,
        new int[,]
        {
            { 1,  1,  1,  1,  1,  1,  1,  1,  1 },
            { 1,  2,  2,  2,  2,  2,  2,  2,  1 },
            { 1,  2,  3,  3,  3,  3,  3,  2,  1 },
            { 1,  2,  3,  4,  4,  4,  3,  2,  1 },
            { 1,  2,  3,  4,  5,  4,  3,  2,  1 },
            { 1,  2,  3,  4,  4,  4,  3,  2,  1 },
            { 1,  2,  3,  3,  3,  3,  3,  2,  1 },
            { 1,  2,  2,  2,  2,  2,  2,  2,  1 },
            { 1,  1,  1,  1,  1,  1,  1,  1,  1 }
        }
    },
{
        PieceType.Knight,
        new int[,]
        {
            { 4,  4,  4,  4,  4,  4,  4,  4,  4 },
            { 4,  6,  6,  6,  6,  6,  6,  6,  4 },
            { 4,  6,  8,  8,  8,  8,  8,  6,  4 },
            { 4,  6,  8, 10, 10, 10,  8,  6,  4 },
            { 4,  6,  8, 10, 12, 10,  8,  6,  4 },
            { 4,  6,  8, 10, 10, 10,  8,  6,  4 },
            { 4,  6,  8,  8,  8,  8,  8,  6,  4 },
            { 4,  6,  6,  6,  6,  6,  6,  6,  4 },
            { 4,  4,  4,  4,  4,  4,  4,  4,  4 }
        }
    },
{
        PieceType.Bishop,
        new int[,]
        {
            { 2,  2,  2,  2,  2,  2,  2,  2,  2 },
            { 2,  3,  3,  3,  3,  3,  3,  3,  2 },
            { 2,  3,  4,  4,  4,  4,  4,  3,  2 },
            { 2,  3,  4,  5,  5,  5,  4,  3,  2 },
            { 2,  3,  4,  5,  6,  5,  4,  3,  2 },
            { 2,  3,  4,  5,  5,  5,  4,  3,  2 },
            { 2,  3,  4,  4,  4,  4,  4,  3,  2 },
            { 2,  3,  3,  3,  3,  3,  3,  3,  2 },
            { 2,  2,  2,  2,  2,  2,  2,  2,  2 }
        }
    },
{
        PieceType.Lance,
        new int[,]
        {
            { 3,  3,  3,  3,  3,  3,  3,  3,  3 },
            { 3,  4,  4,  4,  4,  4,  4,  4,  3 },
            { 3,  4,  5,  5,  5,  5,  5,  4,  3 },
            { 3,  4,  5,  6,  6,  6,  5,  4,  3 },
            { 3,  4,  5,  6,  7,  6,  5,  4,  3 },
            { 3,  4,  5,  6,  6,  6,  5,  4,  3 },
            { 3,  4,  5,  5,  5,  5,  5,  4,  3 },
            { 3,  4,  4,  4,  4,  4,  4,  4,  3 },
            { 3,  3,  3,  3,  3,  3,  3,  3,  3 }
        }
    },
    };
    

    public void selectPiece()
    {
        board.selectedTile.raised();
        foreach (int[] possibleMove in board.selectedTile.getMoves(board.board))
        {
            int x = possibleMove[0], y = possibleMove[1];
            if (x >= 0 && x < Board.boardSize && y >= 0 && y < Board.boardSize)
                board.board[x, y].highlightEnable();
        }
    }

    public void deselectPiece()
    {
        if (board.selectedTile)
        {
            board.selectedTile.deselected();
            board.selectedTile = null;
            foreach (Tile tile in board.board) tile.highlightDisable();
        }
    }

    public void movePiece(Tile targetTile)
    {
        if (targetTile.isHighlighted())
        {
            if (board.selectedTile)
            {
                StartCoroutine(pieceMovement(board.selectedTile, targetTile));
                if (targetTile.getState() == PieceType.King)
                    endGame(DeathType.KingKilled, true);
            }
        }
        else deselectPiece();
    }

    private IEnumerator pieceMovement(Tile currentTile, Tile targetTile)
    {
        yield return
            currentTile.StartCoroutine(
                currentTile.moveState(targetTile)
            );
        board.boardSound.PlayOneShot(board.boardSound.clip);
        if (playersTurn) deselectPiece();
        playersTurn = !playersTurn;
    }

    public void playerStatus()
    {
        if (board == null)
        {
            Debug.LogError("Board object is not initialized.");
            return;
        }
        // For all the possible targets of all player pieces present:
        int numberOfPieces = 0;
        Tile currentKing = null;
        validMoves.Clear();
        validKingMoves.Clear();
        foreach (Tile tile in board.board)
        {
            if (tile.getState() != PieceType.None && tile.isEnemy() == false)
            {
                numberOfPieces++;
                foreach (int[] possibleMove in tile.getMoves(board.board))
                {
                    // Skip invalid moves / moves that lead to another fellow piece.
                    int x = possibleMove[0], y = possibleMove[1];
                    if (x < 0 || x >= Board.boardSize ||
                        y < 0 || y >= Board.boardSize || (
                        board.board[x, y].getState() != PieceType.None &&
                        board.board[x, y].isEnemy() == false
                        ))
                        continue;
                    // Record all of the player's valid moves into the list.
                    this.validMoves.Add(possibleMove);
                    if (tile.getState() == PieceType.King)
                    {
                        this.validKingMoves.Add(possibleMove);
                        currentKing = tile;
                    }
                }
            }
        }
        // Check for death conditions.
        if (numberOfPieces == 0) endGame(DeathType.NoPieces, false);
        if (validMoves.Count == 0) endGame(DeathType.NoMoves, false);
        checkmateStatus(currentKing, this.validKingMoves, this.validEnemyMoves, false);


    }

    private void checkmateStatus(Tile currentKing, List<int[]> currentKingMoves, List<int[]> allEnemyMoves, bool enemyTurn)
    {
        int overlapCount = 0;
        bool kingChecked = false;
        foreach (int[] currentKingMove in currentKingMoves)
        {
            foreach (int[] allEnemyMove in allEnemyMoves)
            {
                if (currentKing.getRow() == allEnemyMove[0] &&
                    currentKing.getCol() == allEnemyMove[1] &&
                    kingChecked == false)
                {
                    overlapCount++;
                    kingChecked = true;
                    break;
                }

                if (currentKingMove[0] == allEnemyMove[0] &&
                    currentKingMove[1] == allEnemyMove[1])
                {
                    overlapCount++;
                    break;
                }
            }
        }
        if (overlapCount == currentKingMoves.Count + 1) endGame(DeathType.Checkmate, enemyTurn);
    }

    private enum DeathType
    {
        Checkmate, KingKilled, NoMoves, NoPieces
    };
}
