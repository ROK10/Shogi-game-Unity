using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int row;
    private int col;
    private Piece piece;

    private GameObject highlight;

    void Start()
    {
        highlight = transform.GetChild(1).gameObject;
        highlight.SetActive(false);
    }

    public int getRow()
    {
        return row;
    }

    public int getCol()
    {
        return col;
    }

    public void setPosition(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public void raised()
    {
        if (this.piece)
            this.piece.raised();
    }

    public void deselected()
    {
        if (this.piece)
            this.piece.deselected();
    }

    public void highlightEnable()
    {
        highlight.SetActive(true);
    }

    public void highlightDisable()
    {
        highlight.SetActive(false);
    }

    public bool isHighlighted()
    {
        return highlight.activeSelf;
    }

    public bool isEnemy()
    {
        return (
            this.piece != null ?
            this.piece.isEnemy() :
            false
        );
    }
    public bool isPlayer ()
    {
        return (
            this.piece != null ?
            this.piece.isPlayer() :
            false
        );
    }

    public PieceType getState()
    {
        return (
            this.piece != null ?
            this.piece.getPiece() :
            PieceType.None
        );
    }

    public List<int[]> getMoves(Tile[,] board)
    {
        return piece.pieceMoves(this.row, this.col, board);
    }

    public void setState(PieceType state, bool enemy, bool player, bool promoted)
    {
        if (state == PieceType.None) removePiece();
        else
        {
            if (this.piece == null) addPiece(state, enemy, player, promoted);
            else
            {
                Logger.Log(this.piece.isEnemy(), state, this.piece.getPiece());
                removePiece();
                addPiece(state, enemy, player, promoted);
                StartCoroutine(logWait());
            }
        }
    }

    public IEnumerator moveState(Tile targetTile)
    {
        PieceType temp = this.piece.getPiece();
        bool tempSide = this.piece.isEnemy();
        bool tempSide2 = this.piece.isPlayer();
        bool tempStance = this.piece.isPromoted();
        yield return
            this.piece.StartCoroutine(
                this.piece.moveAnimation(targetTile.transform.position)
            );
        setState(PieceType.None, false, false, false);
        targetTile.setState(temp, tempSide, tempSide2, tempStance);
        targetTile.checkPromotion();
    }

    private void addPiece(PieceType state, bool enemy, bool player ,bool promoted)
    {
        this.piece = Instantiate(
            GameController.piecePrefab,
            transform.position, Quaternion.identity)
            .GetComponent<Piece>();
        if (enemy)
            this.piece.gameObject.transform.GetChild(0)
            .Rotate(0, 180, 0, Space.World);
        this.piece.setPiece(state, enemy, player, promoted);
    }

    private void removePiece()
    {
        if (this.piece)
        {
            Destroy(this.piece.gameObject);
            this.piece = null;
        }
    }

    private void checkPromotion()
    {
        if (this.piece.getPiece() == PieceType.King ||
            this.piece.getPiece() == PieceType.Gold ||
            this.piece.isPromoted())
            return;

        if (this.piece.isEnemy() == false &&
            this.row >= Board.boardSize - 3)
            this.piece.promotion();

        else if (this.piece.isEnemy() == true &&
            this.row < 3)
            this.piece.promotion();
    }

    private IEnumerator logWait()
    {
        yield return new WaitForSeconds(2.0f);
        Logger.Clear();
    }
}
