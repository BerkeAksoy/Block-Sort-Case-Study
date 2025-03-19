using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    private Piece _pieceParent;
    protected Piece PieceParent { get => _pieceParent; }

    private void Start()
    {
        _pieceParent = GetComponentInParent<Piece>();
    }
}
