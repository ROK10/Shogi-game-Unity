﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    None, Pawn, Bishop, Rook, Lance, Knight, Silver, Gold, King
};

public class Piece : MonoBehaviour
{
    private static readonly float hoverHeight = 0.3f;
    private static readonly float movementSpeed = 10.0f;
    private static readonly float turnSpeed = 15.0f;

    private TextMesh characterFace;

    private PieceType piece = PieceType.None;
    private bool promoted;
    private bool enemy;
    private bool player;

    void Awake()
    {
        characterFace = Instantiate(
            GameController.facePrefab, this.transform)
            .GetComponent<TextMesh>();
    }

    public void raised()
    {
        transform.Translate(0, hoverHeight, 0);
    }

    public void deselected()
    {
        transform.Translate(0, -hoverHeight, 0);
    }

    public PieceType getPiece()
    {
        return piece;
    }

    public bool isEnemy()
    {
        return enemy;
    }

    public bool isPlayer()
    {
        return player;
    }

    public bool isPromoted()
    {
        return promoted;
    }

    public void setPiece(PieceType piece, bool enemy, bool player, bool promoted)
    {
        this.piece = piece;
        this.enemy = enemy;
        this.player = player;
        if (enemy)
            characterFace.transform.Rotate(0, 180, 0, Space.World);
        this.promoted = promoted;
        renderFace(piece);
    }

    public void promotion()
    {
        this.promoted = true;
        characterFace.text = "";
        StartCoroutine(flipAnimation());
        characterFace.color = Color.red;
    }

    public IEnumerator moveAnimation(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        for (float i = 0.0f, c = 0.0f; i <= 1.0f;
            c += movementSpeed * Time.deltaTime,
            i = c / distance)
        {
            transform.position = Vector3.Lerp(
                startPosition, targetPosition, i
            );
            yield return null;
        }
    }

    private IEnumerator flipAnimation()
    {
        for (int i = 0; i < (180 / turnSpeed); i++)
        {
            transform.GetChild(0).Rotate(Vector3.forward, turnSpeed);
            yield return null;
        }
        renderFace(piece);
    }

    private void renderFace(PieceType piece)
    {
        if (promoted == false)
        {
            switch (piece)
            {
                case PieceType.Pawn:
                    characterFace.text = "P";
                    //characterFace.text = "歩";
                    break;

                case PieceType.Lance:
                    characterFace.text = "L";
                    //characterFace.text = "香";
                    break;

                case PieceType.Knight:
                    characterFace.text = "Kn";
                    //characterFace.text = "桂";
                    break;

                case PieceType.Bishop:
                    characterFace.text = "B";
                    //characterFace.text = "角";
                    break;

                case PieceType.Rook:
                    characterFace.text = "R";
                    //characterFace.text = "飛";
                    break;

                case PieceType.Silver:
                    characterFace.text = "S";
                    //characterFace.text = "銀";
                    break;

                case PieceType.Gold:
                    characterFace.text = "G";
                    //characterFace.text = "金";
                    break;

                case PieceType.King:
                    characterFace.text = "Ki";
                    //characterFace.text = (enemy ? "王" : "玉");
                    break;

                default:
                    break;
            }
            characterFace.color = Color.black;
        }
        else
        {
            switch (piece)
            {
                case PieceType.Pawn:
                    characterFace.text = "と";
                    break;

                case PieceType.Lance:
                    characterFace.text = "杏";
                    break;

                case PieceType.Knight:
                    characterFace.text = "圭";
                    break;

                case PieceType.Bishop:
                    characterFace.text = "馬";
                    break;

                case PieceType.Rook:
                    characterFace.text = "龍";
                    break;

                case PieceType.Silver:
                    characterFace.text = "全";
                    break;

                default:
                    break;
            }
            characterFace.color = Color.red;
        }
    }

    public List<int[]> pieceMoves(int row, int col, Tile[,] board)
    {
        List<int[]> possibleMoves = new List<int[]>();
        if (enemy == true)
        {
            if (promoted == false)
                switch (this.piece)
                {
                    case PieceType.Pawn:
                        possibleMoves.Add(new int[2] { row - 1, col });
                        break;

                    case PieceType.Knight:
                        possibleMoves.Add(new int[2] { row - 2, col - 1 });
                        possibleMoves.Add(new int[2] { row - 2, col + 1 });
                        break;

                    case PieceType.Silver:
                        possibleMoves.Add(new int[2] { row - 1, col });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        break;

                    case PieceType.Gold:
                        possibleMoves.Add(new int[2] { row - 1, col });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row + 1, col });
                        break;

                    case PieceType.King:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        break;

                    case PieceType.Lance:
                        lanceMoves(possibleMoves, row, col, board);
                        break;

                    case PieceType.Bishop:
                        bishopMoves(possibleMoves, row, col, board);
                        break;

                    case PieceType.Rook:
                        rookMoves(possibleMoves, row, col, board);
                        break;

                    default:
                        break;
                }
            else
                switch (this.piece)
                {
                    case PieceType.Bishop:
                        bishopMoves(possibleMoves, row, col, board);
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        break;

                    case PieceType.Rook:
                        rookMoves(possibleMoves, row, col, board);
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        break;

                    default:
                        possibleMoves.Add(new int[2] { row - 1, col });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row + 1, col });
                        break;
                }
            return
                clearSpecificCollision(possibleMoves, board);
        }
        else
        {
            if (promoted == false)
                switch (this.piece)
                {
                    case PieceType.Pawn:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        break;

                    case PieceType.Knight:
                        possibleMoves.Add(new int[2] { row + 2, col - 1 });
                        possibleMoves.Add(new int[2] { row + 2, col + 1 });
                        break;

                    case PieceType.Silver:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        break;

                    case PieceType.Gold:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        break;

                    case PieceType.King:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        break;

                    case PieceType.Lance:
                        lanceMoves(possibleMoves, row, col, board);
                        break;

                    case PieceType.Bishop:
                        bishopMoves(possibleMoves, row, col, board);
                        break;

                    case PieceType.Rook:
                        rookMoves(possibleMoves, row, col, board);
                        break;

                    default:
                        break;
                }
            else
                switch (this.piece)
                {
                    case PieceType.Bishop:
                        bishopMoves(possibleMoves, row, col, board);
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        break;

                    case PieceType.Rook:
                        rookMoves(possibleMoves, row, col, board);
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        break;

                    default:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        break;
                }
            return
                clearSpecificCollision(possibleMoves, board);
        }
    }
    private List<int[]> clearSpecificCollision(List<int[]> possibleMoves, Tile[,] board)
    {
        if (enemy == false)
        {
            for (int i = possibleMoves.Count - 1; i >= 0; i--)
            {
                int[] possibleMove = possibleMoves[i];
                int x = possibleMove[0], y = possibleMove[1];
                if (x >= 0 && x < Board.boardSize && y >= 0 && y < Board.boardSize)
                {
                    Tile checkedTile = board[x, y];
                    if (checkedTile.getState() != PieceType.None &&
                        checkedTile.isEnemy() == false)
                        possibleMoves.Remove(possibleMove);
                }
            }
        }
        else
        {
            for (int i = possibleMoves.Count - 1; i >= 0; i--)
            {
                int[] possibleMove = possibleMoves[i];
                int x = possibleMove[0], y = possibleMove[1];
                if (x >= 0 && x < Board.boardSize && y >= 0 && y < Board.boardSize)
                {
                    Tile checkedTile = board[x, y];
                    if (checkedTile.getState() != PieceType.None &&
                        checkedTile.isPlayer() == false)
                        possibleMoves.Remove(possibleMove);
                }
            }
        }
        return possibleMoves;
    }

    private void lanceMoves(List<int[]> possibleMoves, int row, int col, Tile[,] board)
    {
        if (enemy == true)
        {
            for (int i = row - 1; i >= 0; i--)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isPlayer())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isPlayer()) break;
                }
                else break;
            }
        }
        else
        {
            for (int i = row + 1; i < Board.boardSize; i++)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isEnemy())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isEnemy()) break;
                }
                else break;
            }
        }
    }

    private void bishopMoves(List<int[]> possibleMoves, int row, int col, Tile[,] board)
    {
        if (enemy == true)
        {
            for (int i = 1; i < Board.boardSize; i++)
            {
                if (row + i > 8 || col + i > 8) break;
                if (board[row + i, col + i].getState() == PieceType.None ||
                    board[row + i, col + i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row + i, col + i });
                    if (board[row + i, col + i].isEnemy()) break;
                }
                else break;
            }
            for (int i = 1; i < Board.boardSize; i++)
            {
                if (row + i > 8 || col - i < 0) break;
                if (board[row + i, col - i].getState() == PieceType.None ||
                    board[row + i, col - i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row + i, col - i });
                    if (board[row + i, col - i].isPlayer()) break;
                }
                else break;
            }
            for (int i = 1; i < Board.boardSize; i++)
            {
                if (row - i < 0 || col + i > 8) break;
                if (board[row - i, col + i].getState() == PieceType.None ||
                    board[row - i, col + i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row - i, col + i });
                    if (board[row - i, col + i].isPlayer()) break;
                }
                else break;
            }
            for (int i = 1; i < Board.boardSize; i++)
            {
                if (row - i < 0 || col - i < 0) break;
                if (board[row - i, col - i].getState() == PieceType.None ||
                    board[row - i, col - i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row - i, col - i });
                    if (board[row - i, col - i].isPlayer()) break;
                }
                else break;
            }
        }
        else
        {
            for (int i = 1; i < Board.boardSize; i++)
            {
                if (row + i > 8 || col + i > 8) break;
                if (board[row + i, col + i].getState() == PieceType.None ||
                    board[row + i, col + i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row + i, col + i });
                    if (board[row + i, col + i].isEnemy()) break;
                }
                else break;
            }
            for (int i = 1; i < Board.boardSize; i++)
            {
                if (row + i > 8 || col - i < 0) break;
                if (board[row + i, col - i].getState() == PieceType.None ||
                    board[row + i, col - i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row + i, col - i });
                    if (board[row + i, col - i].isEnemy()) break;
                }
                else break;
            }
            for (int i = 1; i < Board.boardSize; i++)
            {
                if (row - i < 0 || col + i > 8) break;
                if (board[row - i, col + i].getState() == PieceType.None ||
                    board[row - i, col + i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row - i, col + i });
                    if (board[row - i, col + i].isEnemy()) break;
                }
                else break;
            }
            for (int i = 1; i < Board.boardSize; i++)
            {
                if (row - i < 0 || col - i < 0) break;
                if (board[row - i, col - i].getState() == PieceType.None ||
                    board[row - i, col - i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row - i, col - i });
                    if (board[row - i, col - i].isEnemy()) break;
                }
                else break;
            }
        }

    }

    private void rookMoves(List<int[]> possibleMoves, int row, int col, Tile[,] board)
    {
        if (enemy == true)
        {
            for (int i = row + 1; i < Board.boardSize; i++)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isPlayer())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isPlayer()) break;
                }
                else break;
            }
            for (int i = row - 1; i >= 0; i--)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isPlayer())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isPlayer()) break;
                }
                else break;
            }
            for (int i = col + 1; i < Board.boardSize; i++)
            {
                if (board[row, i].getState() == PieceType.None ||
                    board[row, i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row, i });
                    if (board[row, i].isPlayer()) break;
                }
                else break;
            }
            for (int i = col - 1; i >= 0; i--)
            {
                if (board[row, i].getState() == PieceType.None ||
                    board[row, i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row, i });
                    if (board[row, i].isPlayer()) break;
                }
                else break;
            }
        }
        else
        {
            for (int i = row + 1; i < Board.boardSize; i++)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isEnemy())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isEnemy()) break;
                }
                else break;
            }
            for (int i = row - 1; i >= 0; i--)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isEnemy())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isEnemy()) break;
                }
                else break;
            }
            for (int i = col + 1; i < Board.boardSize; i++)
            {
                if (board[row, i].getState() == PieceType.None ||
                    board[row, i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row, i });
                    if (board[row, i].isEnemy()) break;
                }
                else break;
            }
            for (int i = col - 1; i >= 0; i--)
            {
                if (board[row, i].getState() == PieceType.None ||
                    board[row, i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row, i });
                    if (board[row, i].isEnemy()) break;
                }
                else break;
            }
        }
    }
}
public class TestPiece
{
    
    private PieceType piece = PieceType.None;
    private bool promoted;
    private bool enemy;
    private bool player;

    public PieceType getPiece()
    {
        return piece;
    }

    public bool isEnemy()
    {
        return enemy;
    }

    public bool isPlayer()
    {
        return player;
    }

    public bool isPromoted()
    {
        return promoted;
    }

    public void setPiece(PieceType piece, bool enemy, bool player, bool promoted)
    {
        this.piece = piece;
        this.enemy = enemy;
        this.player = player;
        this.promoted = promoted;
    }

    public void promotion()
    {
        this.promoted = true;
    }


    public List<int[]> pieceMoves(int row, int col, TestTile[,] board)
    {
        List<int[]> possibleMoves = new List<int[]>();
        if (enemy == true)
        {
            if (promoted == false)
                switch (this.piece)
                {
                    case PieceType.Pawn:
                        possibleMoves.Add(new int[2] { row - 1, col });
                        break;

                    case PieceType.Knight:
                        possibleMoves.Add(new int[2] { row - 2, col - 1 });
                        possibleMoves.Add(new int[2] { row - 2, col + 1 });
                        break;

                    case PieceType.Silver:
                        possibleMoves.Add(new int[2] { row - 1, col });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        break;

                    case PieceType.Gold:
                        possibleMoves.Add(new int[2] { row - 1, col });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row + 1, col });
                        break;

                    case PieceType.King:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        break;

                    case PieceType.Lance:
                        lanceMoves(possibleMoves, row, col, board);
                        break;

                    case PieceType.Bishop:
                        bishopMoves(possibleMoves, row, col, board);
                        break;

                    case PieceType.Rook:
                        rookMoves(possibleMoves, row, col, board);
                        break;

                    default:
                        break;
                }
            else
                switch (this.piece)
                {
                    case PieceType.Bishop:
                        bishopMoves(possibleMoves, row, col, board);
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        break;

                    case PieceType.Rook:
                        rookMoves(possibleMoves, row, col, board);
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        break;

                    default:
                        possibleMoves.Add(new int[2] { row - 1, col });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row + 1, col });
                        break;
                }
            return
                clearSpecificCollision(possibleMoves,board);
        }
        else
        {
            if (promoted == false)
                switch (this.piece)
                {
                    case PieceType.Pawn:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        break;

                    case PieceType.Knight:
                        possibleMoves.Add(new int[2] { row + 2, col - 1 });
                        possibleMoves.Add(new int[2] { row + 2, col + 1 });
                        break;

                    case PieceType.Silver:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        break;

                    case PieceType.Gold:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        break;

                    case PieceType.King:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        break;

                    case PieceType.Lance:
                        lanceMoves(possibleMoves, row, col, board);
                        break;

                    case PieceType.Bishop:
                        bishopMoves(possibleMoves, row, col, board);
                        break;

                    case PieceType.Rook:
                        rookMoves(possibleMoves, row, col, board);
                        break;

                    default:
                        break;
                }
            else
                switch (this.piece)
                {
                    case PieceType.Bishop:
                        bishopMoves(possibleMoves, row, col, board);
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        break;

                    case PieceType.Rook:
                        rookMoves(possibleMoves, row, col, board);
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col - 1 });
                        possibleMoves.Add(new int[2] { row - 1, col + 1 });
                        break;

                    default:
                        possibleMoves.Add(new int[2] { row + 1, col });
                        possibleMoves.Add(new int[2] { row + 1, col - 1 });
                        possibleMoves.Add(new int[2] { row + 1, col + 1 });
                        possibleMoves.Add(new int[2] { row, col - 1 });
                        possibleMoves.Add(new int[2] { row, col + 1 });
                        possibleMoves.Add(new int[2] { row - 1, col });
                        break;
                }
            return
                clearSpecificCollision(possibleMoves, board);
        }
    }
    private List<int[]> clearSpecificCollision(List<int[]> possibleMoves, TestTile[,] board)
    {
        if (enemy == false)
        {
            for (int i = possibleMoves.Count - 1; i >= 0; i--)
            {
                int[] possibleMove = possibleMoves[i];
                int x = possibleMove[0], y = possibleMove[1];
                if (x >= 0 && x < TestBoard.boardSize && y >= 0 && y < TestBoard.boardSize)
                {
                    TestTile checkedTile = board[x, y];
                    if (checkedTile.getState() != PieceType.None &&
                        checkedTile.isEnemy() == false)
                        possibleMoves.Remove(possibleMove);
                }
            }
        }
        else
        {
            for (int i = possibleMoves.Count - 1; i >= 0; i--)
            {
                int[] possibleMove = possibleMoves[i];
                int x = possibleMove[0], y = possibleMove[1];
                if (x >= 0 && x < TestBoard.boardSize && y >= 0 && y < TestBoard.boardSize)
                {
                    TestTile checkedTile = board[x, y];
                    if (checkedTile.getState() != PieceType.None &&
                        checkedTile.isPlayer() == false)
                        possibleMoves.Remove(possibleMove);
                }
            }
        }
        return possibleMoves;
    }


    private void lanceMoves(List<int[]> possibleMoves, int row, int col, TestTile[,] board)
    {
        if (enemy == true)
        {
            for (int i = row - 1; i >= 0; i--)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isPlayer())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isPlayer()) break;
                }
                else break;
            }
        }
        else
        {
            for (int i = row + 1; i < TestBoard.boardSize; i++)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isEnemy())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isEnemy()) break;
                }
                else break;
            }
        }
    }

    private void bishopMoves(List<int[]> possibleMoves, int row, int col, TestTile[,] board)
    {
        if (enemy == true)
        {
            for (int i = 1; i < TestBoard.boardSize; i++)
            {
                if (row + i > 8 || col + i > 8) break;
                if (board[row + i, col + i].getState() == PieceType.None ||
                    board[row + i, col + i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row + i, col + i });
                    if (board[row + i, col + i].isEnemy()) break;
                }
                else break;
            }
            for (int i = 1; i < TestBoard.boardSize; i++)
            {
                if (row + i > 8 || col - i < 0) break;
                if (board[row + i, col - i].getState() == PieceType.None ||
                    board[row + i, col - i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row + i, col - i });
                    if (board[row + i, col - i].isPlayer()) break;
                }
                else break;
            }
            for (int i = 1; i < TestBoard.boardSize; i++)
            {
                if (row - i < 0 || col + i > 8) break;
                if (board[row - i, col + i].getState() == PieceType.None ||
                    board[row - i, col + i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row - i, col + i });
                    if (board[row - i, col + i].isPlayer()) break;
                }
                else break;
            }
            for (int i = 1; i < TestBoard.boardSize; i++)
            {
                if (row - i < 0 || col - i < 0) break;
                if (board[row - i, col - i].getState() == PieceType.None ||
                    board[row - i, col - i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row - i, col - i });
                    if (board[row - i, col - i].isPlayer()) break;
                }
                else break;
            }
        }
        else
        {
            for (int i = 1; i < TestBoard.boardSize; i++)
            {
                if (row + i > 8 || col + i > 8) break;
                if (board[row + i, col + i].getState() == PieceType.None ||
                    board[row + i, col + i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row + i, col + i });
                    if (board[row + i, col + i].isEnemy()) break;
                }
                else break;
            }
            for (int i = 1; i < TestBoard.boardSize; i++)
            {
                if (row + i > 8 || col - i < 0) break;
                if (board[row + i, col - i].getState() == PieceType.None ||
                    board[row + i, col - i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row + i, col - i });
                    if (board[row + i, col - i].isEnemy()) break;
                }
                else break;
            }
            for (int i = 1; i < TestBoard.boardSize; i++)
            {
                if (row - i < 0 || col + i > 8) break;
                if (board[row - i, col + i].getState() == PieceType.None ||
                    board[row - i, col + i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row - i, col + i });
                    if (board[row - i, col + i].isEnemy()) break;
                }
                else break;
            }
            for (int i = 1; i < TestBoard.boardSize; i++)
            {
                if (row - i < 0 || col - i < 0) break;
                if (board[row - i, col - i].getState() == PieceType.None ||
                    board[row - i, col - i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row - i, col - i });
                    if (board[row - i, col - i].isEnemy()) break;
                }
                else break;
            }
        }
        
    }

    private void rookMoves(List<int[]> possibleMoves, int row, int col, TestTile[,] board)
    {
        if (enemy == true)
        {
            for (int i = row + 1; i < TestBoard.boardSize; i++)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isPlayer())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isPlayer()) break;
                }
                else break;
            }
            for (int i = row - 1; i >= 0; i--)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isPlayer())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isPlayer()) break;
                }
                else break;
            }
            for (int i = col + 1; i < TestBoard.boardSize; i++)
            {
                if (board[row, i].getState() == PieceType.None ||
                    board[row, i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row, i });
                    if (board[row, i].isPlayer()) break;
                }
                else break;
            }
            for (int i = col - 1; i >= 0; i--)
            {
                if (board[row, i].getState() == PieceType.None ||
                    board[row, i].isPlayer())
                {
                    possibleMoves.Add(new int[2] { row, i });
                    if (board[row, i].isPlayer()) break;
                }
                else break;
            }
        }
        else
        {
            for (int i = row + 1; i < TestBoard.boardSize; i++)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isEnemy())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isEnemy()) break;
                }
                else break;
            }
            for (int i = row - 1; i >= 0; i--)
            {
                if (board[i, col].getState() == PieceType.None ||
                    board[i, col].isEnemy())
                {
                    possibleMoves.Add(new int[2] { i, col });
                    if (board[i, col].isEnemy()) break;
                }
                else break;
            }
            for (int i = col + 1; i < TestBoard.boardSize; i++)
            {
                if (board[row, i].getState() == PieceType.None ||
                    board[row, i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row, i });
                    if (board[row, i].isEnemy()) break;
                }
                else break;
            }
            for (int i = col - 1; i >= 0; i--)
            {
                if (board[row, i].getState() == PieceType.None ||
                    board[row, i].isEnemy())
                {
                    possibleMoves.Add(new int[2] { row, i });
                    if (board[row, i].isEnemy()) break;
                }
                else break;
            }
        }
    }
}