using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected Piece _pieceParent;
    public Piece PieceParent { get => _pieceParent; }

    private void Start()
    {
        _pieceParent = GetComponentInParent<Piece>();
    }
}
