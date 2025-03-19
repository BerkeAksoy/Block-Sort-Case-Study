using System.Collections.Generic;
using UnityEngine;

public class SlotPiece : Piece
{
    private List<SlotLegoUnit> _units;
    public List<SlotLegoUnit> Units { get => _units; set => _units = value; }

    public List<SlotLegoUnit> GetUnits()
    {
        _units = _slotUnits;
        return _units;
    }

    /*
    public bool IsSlotCompleted(Piece slotToCheck) // When all the LegoUnits are 1 the slot is considered to be completed.
    {
        foreach (var unit in slotToCheck.LegoUnits)
        {
            if (unit.CurLegoValue != 1)
            {
                return false;
            }
        }

        return true;
    }
    */
}
