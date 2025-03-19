using UnityEngine;

public class DefLegoUnit : Unit
{
    [SerializeField] private int _legoValue = 0;
    private Piece.PieceColor _legoColor = Piece.PieceColor.None;
    public int LegoValue { get => _legoValue; set => _legoValue = value; }
    public Piece.PieceColor LegoColor { get => _legoColor; set => _legoColor = value; }
}
