using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public enum PieceColor
    {
        None,
        Blue,
        Green,
        Orange,
        Red,
        Yellow
    }

    private BoardManager _boardManager;

    private List<List<int>> _shapeArray = new List<List<int>>();
    protected List<SlotLegoUnit> _slotUnits = new List<SlotLegoUnit>();
    protected List<DefLegoUnit> _DefUnits = new List<DefLegoUnit>();

    private PieceColor _pieceColor;
    private Material _pieceMat;
    protected Vector3 _originalPosition;
    private const float _unitSize = 1.0f; // One cubic Lego unit is 1 unit in Unity scale

    public List<List<int>> ShapeArray { get =>  _shapeArray; }
    public PieceColor PieceClr { get => _pieceColor; }

    public virtual void Initialize(PieceColor color, List<List<int>> shapeArray, bool screwed = false)
    {
        _pieceColor = color;
        _shapeArray = shapeArray;

        _boardManager = BoardManager.Instance;
    }

    protected void CreateLegoUnits(bool isSlot, bool isScrewed = false)
    {
        for (int row = 0; row < _shapeArray.Count; row++)
        {
            for (int col = 0; col < _shapeArray[row].Count; col++)
            {
                int cellValue = _shapeArray[row][col];
                if (cellValue == 0) continue; // Skip empty cells

                Vector3 position = new Vector3(col * _unitSize, -row * _unitSize, 0);
                GameObject legoGO;

                if (cellValue == 1)
                {
                    legoGO = DefineCornersForSquareLego(row, col);
                    if (legoGO == null) { Debug.Log($"legoGo is null row {row} col {col}"); }
                    legoGO.transform.localPosition = position;

                    if (isSlot)
                    {
                        Transform unit = legoGO.transform.GetChild(0);
                        SlotLegoUnit newSlotLegoUnit = legoGO.transform.GetChild(0).AddComponent<SlotLegoUnit>();
                        
                        unit.gameObject.layer = 3;
                        newSlotLegoUnit.OrjLegoValue = 0;
                        newSlotLegoUnit.CurLegoValue = 0;

                        _slotUnits.Add(newSlotLegoUnit);
                    }
                    else
                    {
                        DefLegoUnit newDefLegoUnit = legoGO.transform.GetChild(0).AddComponent<DefLegoUnit>();
                        if (isScrewed) { Instantiate(_boardManager.FullScrew, legoGO.transform); }

                        newDefLegoUnit.LegoValue = 1;
                        newDefLegoUnit.LegoColor = _pieceColor;

                        _DefUnits.Add(newDefLegoUnit);
                    }
                }
                else if (cellValue >= 2 && cellValue <= 5)
                {
                    legoGO = DefineCornersForTriangleLego(row, col, cellValue);
                    legoGO.transform.localPosition = position;

                    if (isSlot)
                    {
                        Transform unit = legoGO.transform.GetChild(0);
                        SlotLegoUnit newSlotLegoUnit = legoGO.transform.GetChild(0).AddComponent<SlotLegoUnit>();

                        unit.gameObject.layer = 3;
                        newSlotLegoUnit.OrjLegoValue = cellValue;
                        newSlotLegoUnit.CurLegoValue = cellValue;


                        _slotUnits.Add(newSlotLegoUnit);
                    }
                    else
                    {
                        DefLegoUnit newDefLegoUnit = legoGO.transform.GetChild(0).AddComponent<DefLegoUnit>();
                        if (isScrewed) { Instantiate(_boardManager.HalfScrew, legoGO.transform); }

                        newDefLegoUnit.LegoValue = cellValue;
                        newDefLegoUnit.LegoColor = _pieceColor;

                        _DefUnits.Add(newDefLegoUnit);
                    }

                    ApplyHalfUnitRotation(legoGO, cellValue);
                }
                else
                {
                    Debug.LogWarning("Inappropriate cell value detected");
                    continue;
                }

                legoGO.GetComponentInChildren<MeshRenderer>().material = _pieceMat;
            }
        }
    }

    private GameObject DefineCornersForSquareLego(int row, int col)
    {
        GameObject legoUnit = null;
        bool up = false, down = false, right = false, left = false;
        int closedSideCount = 0;

        for (int i = 0; i < Helpers.NeighborDirections.Length; i++)
        {
            int rowToCheck = row - Helpers.NeighborDirections[i].y; // Minus row because I am constructing the piece from top to bottom.
            int colToCheck = col + Helpers.NeighborDirections[i].x;

            if (colToCheck < 0 || rowToCheck < 0 || colToCheck >= _shapeArray[row].Count || rowToCheck >= _shapeArray.Count || _shapeArray[rowToCheck][colToCheck] == 0) continue;

            switch(i)
            {
                case 0: up = true; closedSideCount++; break;
                case 1: down = true; closedSideCount++; break;
                case 2: right = true; closedSideCount++; break;
                case 3: left = true; closedSideCount++; break;
            }
        }

        switch (closedSideCount)
        {
            case 0: legoUnit = Instantiate(_boardManager.FullAllRoundedLego, transform); break;
            case 1:
                legoUnit = Instantiate(_boardManager.FullThreeRoundedLego, transform);

                if (up) { legoUnit.transform.Rotate(0, 0, 90); }
                else if (down) { legoUnit.transform.Rotate(0, 0, 270); }
                else if (right) { } // By default the hard corner faces right
                else { legoUnit.transform.Rotate(0, 0, 180); }
                break;
            case 2:
                if (up && down)
                {
                    legoUnit = Instantiate(_boardManager.FullTwoParallelRoundedLego, transform);
                    legoUnit.transform.Rotate(0, 0, 90);
                }
                else if (left && right)
                {
                    legoUnit = Instantiate(_boardManager.FullTwoParallelRoundedLego, transform);
                    // By default the hard corners face left and right
                }
                else
                {
                    legoUnit = Instantiate(_boardManager.FullTwoRoundedLego, transform);

                    if (up && left) { legoUnit.transform.Rotate(0, 0, 180); }
                    else if (up && right) { legoUnit.transform.Rotate(0, 0, 90); }
                    else if (down && left) { legoUnit.transform.Rotate(0, 0, 270); }
                    else { } // By default the hard corners face right and down
                }
                break;
            case 3:
                legoUnit = Instantiate(_boardManager.FullOneRoundedLego, transform);

                if (!up) { } // By default the soft corner faces up
                else if (!down) { legoUnit.transform.Rotate(0, 0, 180); }
                else if (!right) { legoUnit.transform.Rotate(0, 0, 270); }
                else { legoUnit.transform.Rotate(0, 0, 90); }
                break;
            case 4: legoUnit = Instantiate(_boardManager.FullAllHardLego, transform); break;
        }

        return legoUnit;
    }

    private GameObject DefineCornersForTriangleLego(int row, int col, int rotType)
    {
        GameObject legoUnit = null;
        bool up = false, down = false, right = false, left = false;

        for (int i = 0; i < Helpers.NeighborDirections.Length; i++)
        {
            int rowToCheck = row - Helpers.NeighborDirections[i].y; // Minus row because I am constructing the piece from top to bottom.
            int colToCheck = col + Helpers.NeighborDirections[i].x;

            if (colToCheck < 0 || rowToCheck < 0 || colToCheck >= _shapeArray[row].Count || rowToCheck >= _shapeArray.Count || _shapeArray[rowToCheck][colToCheck] == 0) continue;

            switch (i)
            {
                case 0: up = true; break;
                case 1: down = true; break;
                case 2: right = true; break;
                case 3: left = true; break;
            }
        }

        if (!(up || down || right || left)) { legoUnit = Instantiate(_boardManager.HalfAllRoundedLego, transform); return legoUnit; }

        switch (rotType)
        {
            case 2:
                if (up)
                {
                    if (right) { legoUnit = Instantiate(_boardManager.HalfAllHardLego, transform); return legoUnit; }
                    legoUnit = Instantiate(_boardManager.HalfRightRoundedLego, transform);
                }
                else if (right) { legoUnit = Instantiate(_boardManager.HalfUpRoundedLego, transform); }
                break;
            case 3:
                if (up)
                {
                    if (left) { legoUnit = Instantiate(_boardManager.HalfAllHardLego, transform); return legoUnit; }
                    legoUnit = Instantiate(_boardManager.HalfUpRoundedLego, transform);
                }
                else if (left) { legoUnit = Instantiate(_boardManager.HalfRightRoundedLego, transform); }
                break;
            case 4:
                if (down)
                {
                    if (left) { legoUnit = Instantiate(_boardManager.HalfAllHardLego, transform); return legoUnit; }
                    legoUnit = Instantiate(_boardManager.HalfRightRoundedLego, transform);
                }
                else if (left)
                {
                    legoUnit = Instantiate(_boardManager.HalfUpRoundedLego, transform);
                }
                break;
            case 5:
                if(down)
                {
                    if(right) { legoUnit = Instantiate(_boardManager.HalfAllHardLego, transform); return legoUnit; }
                    legoUnit = Instantiate(_boardManager.HalfUpRoundedLego, transform);
                }
                if (right) { legoUnit = Instantiate(_boardManager.HalfRightRoundedLego, transform); }
                break;
        }

        return legoUnit;
    }

    private void ApplyHalfUnitRotation(GameObject halfUnit, int rotType)
    {
        // Hypotenus defines the facing direction
        // rotType: 2 = 0°, 3 = 90°, 4 = 180°, 5 = 270°

        float rotationAngle = 0f;

        switch (rotType)
        {
            case 2: rotationAngle = 0f; break; // Facing down left
            case 3: rotationAngle = 90f; break; // Facing down right
            case 4: rotationAngle = 180f; break; // Facing up right
            case 5: rotationAngle = 270f; break; // Facing up left
        }

        halfUnit.transform.localEulerAngles = new Vector3(0, 0, rotationAngle);
    }

    protected void SetPieceMaterial()
    {
        switch (_pieceColor)
        {
            case PieceColor.None:
                _pieceMat = _boardManager.Materials[0];
                break;
            case PieceColor.Blue:
                _pieceMat = _boardManager.Materials[1];
                break;
            case PieceColor.Green:
                _pieceMat = _boardManager.Materials[2];
                break;
            case PieceColor.Orange:
                _pieceMat = _boardManager.Materials[3];
                break;
            case PieceColor.Red:
                _pieceMat = _boardManager.Materials[4];
                break;
            case PieceColor.Yellow:
                _pieceMat = _boardManager.Materials[5];
                break;
        }
    }

}
