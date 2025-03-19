using System.Collections.Generic;
using UnityEngine;

public class SlotLegoUnit : Unit
{
    [SerializeField] private int _orjLegoValue, _curLegoValue;
    [SerializeField] private List<DefLegoUnit> _heldLegoUnit = new List<DefLegoUnit>();

    public int OrjLegoValue { get => _orjLegoValue; set => _orjLegoValue = value; }
    public int CurLegoValue { get => _curLegoValue; set => _curLegoValue = value; }
    public List<DefLegoUnit> HeldLegoUnit { get => _heldLegoUnit;}
}
