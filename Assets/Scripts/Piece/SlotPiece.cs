using System.Collections.Generic;
using UnityEngine;

public class SlotPiece : Piece
{
    [SerializeField] private List<SlotLegoUnit> _units;
    public List<SlotLegoUnit> Units { get => _units; set => _units = value; }

    public override void Initialize(PieceColor color, List<List<int>> shapeArray, bool screwed)
    {
        base.Initialize(color, shapeArray);

        gameObject.tag = "Slot";
        gameObject.layer = 3;

        SetPieceMaterial();
        CreateLegoUnits(true);
    }

    public List<SlotLegoUnit> GetUnits()
    {
        _units = _slotUnits;
        return _units;
    }

    public bool IsSlotCompleted(PieceColor colorToCheck) // When all the LegoUnits are 1 the slot is considered to be completed.
    {
        HashSet<DefPiece> defPiecesOnSlot = new HashSet<DefPiece>();

        foreach (var slotUnit in GetUnits())
        {
            if (slotUnit.CurLegoValue != 1)
            {
                return false;
            }

            foreach(DefLegoUnit heldUnit in slotUnit.HeldLegoUnit)
            {
                defPiecesOnSlot.Add((DefPiece)heldUnit.PieceParent);

                if (heldUnit.LegoColor != colorToCheck)
                {
                    return false;
                }
            }
        }

        foreach (DefPiece defPiece in defPiecesOnSlot)
        {
            defPiece.IsLocked = true;
            AudioManager.Instance.PlaySlotComplete();
        }

        return true;
    }
}
