using System.Collections.Generic;
using UnityEngine;

public class SlotPiece : Piece
{
    [SerializeField] private List<SlotLegoUnit> _units;
    public List<SlotLegoUnit> Units { get => _units; set => _units = value; }

    public List<SlotLegoUnit> GetUnits()
    {
        _units = _slotUnits;
        return _units;
    }

    public bool IsSlotCompleted(PieceColor colorToCheck) // When all the LegoUnits are 1 the slot is considered to be completed.
    {
        foreach (var slotUnit in GetUnits())
        {
            if (slotUnit.CurLegoValue != 1)
            {
                return false;
            }

            foreach(DefLegoUnit heldUnit in slotUnit.HeldLegoUnit)
            {
                if(heldUnit.LegoColor != colorToCheck)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
