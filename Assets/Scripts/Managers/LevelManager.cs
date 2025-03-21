using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Piece;

public class LevelManager : BaseSingleton<LevelManager>
{
    [SerializeField] private TextAsset[] _levelJson;
    private int _currentLevel;
    private int _stageCount;
    private int _currentStageIndex;
    private int _slotPieceCountToComplete;
    private float _levelTime;
    private List<GameObject> _piecesOnStage = new List<GameObject>();

    public int SlotPieceCountToComplete { get => _slotPieceCountToComplete; }

    public event Action OnStageLoad;

    [System.Serializable]
    public class LevelData
    {
        public int LevelTime;
        public int StageCount;
        public int[] SlotPieceCountToComplete;
        public PieceDataArrayWrapper[] Pieces;
    }

    [System.Serializable]
    public class PieceDataArrayWrapper
    {
        public PieceData[] StagePieces;
    }

    [System.Serializable]
    public class PieceData
    {
        public string Color;
        public IntArrayWrapper[] ShapeArray;
        public bool Screwed;
        public bool IsSlot;
        public int[] Position;
    }

    [System.Serializable]
    public class IntArrayWrapper
    {
        public int[] Row;
    }

    public void LoadLevel()
    {
        WipeStagePieces();

        TouchManager.Instance.FirstTouch = false;
        _currentStageIndex = 0;

        if (_currentLevel >= _levelJson.Length)
        {
            _currentLevel = 0;
        }

        LoadStageFromJson(_levelJson[_currentLevel].text);
        PuzzleTimer.Instance.InitTimer();
    }

    public void LoadNextStage()
    {
        _currentStageIndex++;

        if (_currentStageIndex == _stageCount)
        {
            _currentLevel++;
            WipeStagePieces();
            LoadLevel();
        }
        else
        {
            StartCoroutine(SlideStagePieces());
        }
    }

    private void WipeStagePieces()
    {
        foreach(GameObject p in _piecesOnStage)
        {
            Destroy(p.gameObject);
        }

        _piecesOnStage.Clear();
    }

    public float GetLevelTime()
    {
        return _levelTime;
    }

    public int GetCurrentLevel()
    {
        return _currentLevel + 1;
    }

    public int GetCurrentStage()
    {
        return _currentStageIndex + 1;
    }

    public int GetStageCount()
    {
        return _stageCount;
    }

    private void LoadStageFromJson(string json)
    {
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        if (levelData == null)
        {
            Debug.LogError("Failed to parse LevelData. Check JSON formatting.");
            return;
        }

        if (levelData.Pieces == null || _currentStageIndex >= levelData.Pieces.Length)
        {
            Debug.LogError("No stage data available for the current stage index. Ensure your JSON file contains the correct structure for Pieces.");
            return;
        }

        PieceData[] currentStagePieces = levelData.Pieces[_currentStageIndex].StagePieces;
        _slotPieceCountToComplete = levelData.SlotPieceCountToComplete[_currentStageIndex];
        int slotLegoCount = 0, defLegoCount = 0;

        if (_currentStageIndex == 0)
        {
            _stageCount = levelData.StageCount;
            _levelTime = levelData.LevelTime;
        }

        for (int i = 0; i < currentStagePieces.Length; i++)
        {
            PieceData pieceData = currentStagePieces[i];

            if (pieceData.Position == null || pieceData.Position.Length < 3)
            {
                Debug.LogError("Invalid or missing Position data for a piece at index: " + i);
                continue;
            }

            if (string.IsNullOrEmpty(pieceData.Color))
            {
                Debug.LogError("PieceData.Color is null or empty for Piece" + i);
                continue;
            }

            GameObject pieceObj;
            List<List<int>> shapeList = new List<List<int>>();
            ConvertToShapeList(pieceData, ref shapeList);

            // Convert string to enum (ensure enum names match exactly)
            PieceColor pieceColor = (PieceColor)Enum.Parse(typeof(PieceColor), pieceData.Color);

            if (pieceData.IsSlot)
            {
                pieceObj = new GameObject("Slot Piece_" + slotLegoCount);
                SlotPiece newSlotPiece = pieceObj.AddComponent<SlotPiece>();
                newSlotPiece.Initialize(pieceColor, shapeList, pieceData.Screwed);
                slotLegoCount++;
            }
            else
            {
                pieceObj = new GameObject("Def Piece_" + defLegoCount);
                DefPiece newDefPiece = pieceObj.AddComponent<DefPiece>();
                newDefPiece.Initialize(pieceColor, shapeList, pieceData.Screwed);
                defLegoCount++;
            }
            
            _piecesOnStage.Add(pieceObj);
            pieceObj.transform.position = new Vector3(pieceData.Position[0], pieceData.Position[1], pieceData.Position[2]);
        }

        OnStageLoad?.Invoke();

        BoardManager.Instance.LockTouchable();
        StartCoroutine(ScaleAnim());
    }

    private IEnumerator CheckPiecesInitPos()
    {
        yield return null;

        for(int i = 0; i < _piecesOnStage.Count; i++)
        {
            DefPiece defPiece = _piecesOnStage[i].GetComponent<DefPiece>();
            if (defPiece != null)
            {
                defPiece.CheckIfBornOnSlot();
            }
        }
        BoardManager.Instance.UnlockTouchable();
    }

    private IEnumerator ScaleAnim(float animLength = 0.8f)
    {
        for (int i = 0; i < _piecesOnStage.Count; i++)
        {
            _piecesOnStage[i].transform.localScale = new Vector3(0, 0, 0);
            _piecesOnStage[i].transform.DOScale(new Vector3(1, 1, 1), animLength).SetEase(Ease.OutCirc);
        }

        yield return new WaitForSeconds(animLength);

        StartCoroutine(CheckPiecesInitPos());
    }

    private IEnumerator SlideStagePieces(float animLength = 1f)
    {
        float moveRightTime = 0.4f;
        for (int i = 0; i < _piecesOnStage.Count; i++)
        {
            Transform pieceT = _piecesOnStage[i].transform;
            pieceT.DOMove(new Vector3(pieceT.position.x + 4f, pieceT.position.y, pieceT.position.z), moveRightTime).SetEase(Ease.InQuad);
            pieceT.DOMove(new Vector3(pieceT.position.x - 20f, pieceT.position.y, pieceT.position.z), moveRightTime).SetEase(Ease.InCubic).SetDelay(moveRightTime);
        }

        yield return new WaitForSeconds(animLength);

        WipeStagePieces();
        LoadStageFromJson(_levelJson[_currentLevel].text);
    }

    // Construct the ShapeList from IntArrayWrapper[] to List<List<int>>
    private void ConvertToShapeList(PieceData pieceData, ref List<List<int>> shapeList)
    {
        if (pieceData.ShapeArray != null)
        {
            foreach (var wrapper in pieceData.ShapeArray)
            {
                if (wrapper == null || wrapper.Row == null)
                {
                    Debug.LogError("Invalid shape row in piece data for Piece");
                    continue;
                }
                List<int> row = new List<int>(wrapper.Row);
                shapeList.Add(row);
            }
        }
        else
        {
            Debug.LogWarning("ShapeArray is null for piece data at Piece");
        }
    }

}
