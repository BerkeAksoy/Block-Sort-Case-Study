using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : BaseSingleton<TouchManager>
{
    private BoardManager _boardManager;
    private Camera _mainCamera;
    private DefPiece _touchedPiece;
    private bool isDragging = false, _firstTouch;

    public bool FirstTouch { get => _firstTouch; set => _firstTouch = value; }

    public event Action OnFirstTouch;

    private void Start()
    {
        _boardManager = BoardManager.Instance;
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (_boardManager.IsTouchable())
        {
            // For mobile input.
            if (Touchscreen.current != null)
            {
                if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                {
                    Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
                    TryCallPieceDrag(touchPos);
                }
                else if (Touchscreen.current.primaryTouch.press.isPressed && isDragging)
                {
                    Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
                    DraggablePieceManager.UpdateDrag(touchPos);
                }
                else if (Touchscreen.current.primaryTouch.press.wasReleasedThisFrame && isDragging)
                {
                    Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
                    DraggablePieceManager.EndDrag(touchPos);
                    _touchedPiece = null;
                    isDragging = false;
                }
            }
            // For desktop input.
            else if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    TryCallPieceDrag(mousePos);
                }
                else if (Mouse.current.leftButton.isPressed && isDragging)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    DraggablePieceManager.UpdateDrag(mousePos);
                }
                else if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    DraggablePieceManager.EndDrag(mousePos);
                    isDragging = false;
                }
            }
        }
    }

    private void TryCallPieceDrag(Vector2 touchPos)
    {
        GameObject touched = DetectTouch(touchPos);
        if (touched != null)
        {
            _touchedPiece = touched.GetComponentInParent<DefPiece>();

            if (_touchedPiece != null)
            {
                if (!_firstTouch)
                {
                    _firstTouch = true;
                    OnFirstTouch?.Invoke();
                }

                DraggablePieceManager.StartDrag(_touchedPiece, touchPos);
            }
            isDragging = true;
        }
    }

    private GameObject DetectTouch(Vector2 screenPosition)
    {
        Ray ray = _mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform.gameObject;
        }
        return null;
    }
}
