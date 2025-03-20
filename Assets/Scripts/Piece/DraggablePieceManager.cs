using UnityEngine;
using System.Collections.Generic;
using System;

public static class DraggablePieceManager
{
    public static float _snapThreshold = 1.0f;
    private static bool _isDragging = false;
    
    private static Camera _mainCamera;
    private static DefPiece _curDefPiece;
    private static GameObject _projection;
    private static Vector3 _originalPosition;
    private static Vector3 _offset;
    private static Plane _dragPlane;

    public static event Action OnSlotCompleted;
    
    private static void Initialize()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    public static void StartDrag(DefPiece piece, Vector2 pointerPosition)
    {
        Initialize();

        if(piece.Screwed) { return; }
        
        _curDefPiece = piece;
        _isDragging = true;
        _originalPosition = piece.transform.position;

        // Create a drag plane on the XY plane (constant Z). (This works if pieces are arranged on XY with Z constant.)
        _dragPlane = new Plane(Vector3.forward, piece.transform.position);
        
        // Determine the _offset between the piece's position and the pointer's world position.
        Ray ray = _mainCamera.ScreenPointToRay(pointerPosition);

        if (_dragPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            _offset = piece.transform.position - worldPoint;
        }
        
        CreateProjection();
    }
    
    public static void UpdateDrag(Vector2 pointerPosition)
    {
        if (!_isDragging || _curDefPiece == null) { return; }

        Ray ray = _mainCamera.ScreenPointToRay(pointerPosition);
        if (_dragPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            Vector3 newPos = worldPoint + _offset + new Vector3(0, 0, -1);
            _curDefPiece.transform.position = newPos;

            UpdateGhostProjection();
        }
    }
    
    public static void EndDrag(Vector2 pointerPosition)
    {
        if (!_isDragging || _curDefPiece == null)
            return;
        
        Vector3? validPos = null;
        bool refreshed = false;
        List<int> preValues  = new List<int>();

        if (_curDefPiece.OnSlot)
        {
            preValues = RefreshOccupiedSlotUnits();
            _curDefPiece.OnSlot = false;
            refreshed = true;
        }

        if (IsValidPlacement(ref validPos, true))
        {
            _curDefPiece.transform.position = validPos.Value;
            _originalPosition = validPos.Value;

            _curDefPiece.OnSlot = true;
            bool slotCompleted = ((SlotPiece)_curDefPiece.OccupiedSlotUnits[0].PieceParent).IsSlotCompleted(_curDefPiece.PieceClr);
            if (slotCompleted)
            {
                OnSlotCompleted?.Invoke();
            }
        }
        else
        {
            if (refreshed) { ReApplySlotValues(preValues); _curDefPiece.OnSlot = true; }
            _curDefPiece.transform.position = _originalPosition;
        }

        if (_projection != null)
            UnityEngine.Object.Destroy(_projection);
        
        _isDragging = false;
        _curDefPiece = null;
    }

    private static List<int> RefreshOccupiedSlotUnits()
    {
        List<int> preValues = new List<int>();
        List<DefLegoUnit> defLegoUnits = _curDefPiece.GetUnits();

        for (int i = 0; i < _curDefPiece.OccupiedSlotUnits.Count; i++)
        {
            preValues.Add(_curDefPiece.OccupiedSlotUnits[i].CurLegoValue);
            _curDefPiece.OccupiedSlotUnits[i].HeldLegoUnit.Remove(defLegoUnits[i]);

            if (_curDefPiece.GetUnits()[i].LegoValue == 1)
            {
                _curDefPiece.OccupiedSlotUnits[i].CurLegoValue = 0;
            }
            else
            {
                if(_curDefPiece.OccupiedSlotUnits[i].OrjLegoValue == 0)
                {
                    if(_curDefPiece.OccupiedSlotUnits[i].CurLegoValue == 1)
                    {
                        _curDefPiece.OccupiedSlotUnits[i].CurLegoValue = _curDefPiece.GetUnits()[i].LegoValue;
                    }
                    else
                    {
                        _curDefPiece.OccupiedSlotUnits[i].CurLegoValue = 0;
                    }
                }
                else
                {
                    _curDefPiece.OccupiedSlotUnits[i].CurLegoValue = _curDefPiece.OccupiedSlotUnits[i].OrjLegoValue;
                }
            }
        }

        return preValues;
    }

    private static void ReApplySlotValues(List<int> preValues)
    {
        List<DefLegoUnit> defLegoUnits = _curDefPiece.GetUnits();

        for (int i = 0; i < _curDefPiece.OccupiedSlotUnits.Count; i++)
        {
            _curDefPiece.OccupiedSlotUnits[i].CurLegoValue = preValues[i];
            _curDefPiece.OccupiedSlotUnits[i].HeldLegoUnit.Add(defLegoUnits[i]);
        }
    }

    private static void UpdateGhostProjection()
    {
        Vector3? validPos = null;

        if (IsValidPlacement(ref validPos))
        {
            if (!_projection.activeInHierarchy) { _projection.SetActive(true); }
            
            _projection.transform.position = validPos.Value;
        }
        else
        {
            if (_projection.activeInHierarchy)
                _projection.SetActive(false);
        }
    }

    private static bool IsValidPlacement(ref Vector3? validPos, bool placePiece = false)
    {
        List<DefLegoUnit> defLegoUnits = _curDefPiece.GetUnits();
        List<SlotLegoUnit> occupiedSlotUnits = new List<SlotLegoUnit>();

        Transform validHit0Transform = null;

        if (defLegoUnits == null || defLegoUnits.Count == 0) { return false; }

        for (int i = 0; i < defLegoUnits.Count; i++) // For each legoUnit of the current draggable piece: Cast a ray from the legoUnit's position in the positive Z direction.
        {
            Vector3 origin = defLegoUnits[i].transform.position;
            Ray ray = new Ray(origin, Vector3.forward);
            RaycastHit hit;
            LayerMask mask = 3; // Slot layer
            
            if (!Physics.Raycast(ray, out hit, 10f, ~mask))
            {
                //Debug.Log($"I am {legoUnits[i].transform.parent.parent.name}'s legoUnit {i} and I hit nothing");
                return false;
            }
            else
            {
                //Debug.Log($"Congrats!, I am {legoUnits[i].transform.parent.parent.name}'s legoUnit {i} and I hit {hit.transform.name} layer mask of hit is {hit.transform.gameObject.layer}");
                if(i == 0) { validHit0Transform = hit.transform; }
            }

            SlotLegoUnit slotUnit = hit.collider.GetComponent<SlotLegoUnit>();
            if (slotUnit == null) { return false; }

            occupiedSlotUnits.Add(slotUnit);

            if (!IsCompatible(defLegoUnits[i].LegoValue, slotUnit.CurLegoValue)) { return false; }
        }

        Vector3 SlotPiecePos = validHit0Transform.parent.parent.position;
        Vector3 SlotUnitGO = validHit0Transform.parent.localPosition;
        validPos = SlotPiecePos + SlotUnitGO - defLegoUnits[0].transform.parent.localPosition;
        validPos = new Vector3(validPos.Value.x, validPos.Value.y, 0);

        if (placePiece)
        {
            for (int i = 0; i < defLegoUnits.Count; i++)
            {
                occupiedSlotUnits[i].HeldLegoUnit.Add(defLegoUnits[i]);

                if (defLegoUnits[i].LegoValue == 1)
                {
                    occupiedSlotUnits[i].CurLegoValue = 1;
                }
                else
                {
                    if (occupiedSlotUnits[i].CurLegoValue == 0)
                    {
                        occupiedSlotUnits[i].CurLegoValue = Helpers.GetComplementary(defLegoUnits[i].LegoValue);
                    }
                    else if(occupiedSlotUnits[i].CurLegoValue == defLegoUnits[i].LegoValue)
                    {
                        occupiedSlotUnits[i].CurLegoValue = 1;
                    }
                }
            }

            _curDefPiece.OccupiedSlotUnits = occupiedSlotUnits;
        }

        return true; // All legoUnits passed the compatibility test.
    }

    private static bool IsCompatible(int draggableValue, int slotValue)
    {
        if (draggableValue == 1)
            return slotValue == 0;
        else if (draggableValue == 2)
            return slotValue == 0 || slotValue == 2;
        else if (draggableValue == 3)
            return slotValue == 0 || slotValue == 3;
        else if (draggableValue == 4)
            return slotValue == 0 || slotValue == 4;
        else if (draggableValue == 5)
            return slotValue == 0 || slotValue == 5;

        return false; // If an unexpected value is encountered, consider it incompatible.
    }

    // Creates a ghost _projection copy of the current piece for visual feedback.
    private static void CreateProjection()
    {
        if (_projection != null)
            UnityEngine.Object.Destroy(_projection);

        Vector3 pos = _curDefPiece.transform.position + new Vector3 (0, 0, 1);
        _projection = UnityEngine.Object.Instantiate(_curDefPiece.gameObject, pos, _curDefPiece.transform.rotation);
        
        // Disable collider on the _projection.
        Collider col = _projection.GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
        
        // Set all MeshRenderers on the _projection to use the _projection material.
        MeshRenderer[] renderers = _projection.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            r.material = BoardManager.Instance.ProjectionMaterial;
        }
    }
}
