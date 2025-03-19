using System.Collections.Generic;
using UnityEngine;

public class DefPiece : Piece
{
    [SerializeField] private List<DefLegoUnit> _units;
    [SerializeField] private List<SlotLegoUnit> _occupiedSlotUnits = new List<SlotLegoUnit>();
    private bool _screwed, _onSlot;

    public List<SlotLegoUnit> OccupiedSlotUnits { get => _occupiedSlotUnits; set => _occupiedSlotUnits = value; }
    public bool OnSlot { get => _onSlot; set => _onSlot = value; }

    public List<DefLegoUnit> GetUnits()
    {
        _units = _DefUnits;
        return _units;
    }

}
