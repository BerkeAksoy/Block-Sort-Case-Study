using UnityEngine;

public class SlotLegoUnit : Unit
{
    private int _orjLegoValue, _curLegoValue;

    public int OrjLegoValue { get => _orjLegoValue; set => _orjLegoValue = value; }
    public int CurLegoValue { get => _curLegoValue; set => _curLegoValue = value; }
}
