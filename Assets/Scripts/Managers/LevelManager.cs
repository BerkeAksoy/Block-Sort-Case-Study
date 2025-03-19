using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static LevelManager;
using static Piece;

public class LevelManager : BaseSingleton<LevelManager>
{
    [SerializeField] private TextAsset[] _levelJson;
    private int _currentLevel;
    private int _stageCount;
    private int _currentStageIndex;
    private int _slotPieceCountToComplete;
    private float _levelTime;

    public int SlotPieceCountToComplete { get => _slotPieceCountToComplete; }

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
        TouchManager.Instance.FirstTouch = false;
        _currentStageIndex = 0;
        if (_currentLevel >= _levelJson.Length)
        {
            _currentLevel = 0;
        }
        LoadStageFromJson(_levelJson[_currentLevel].text);
    }

    public void LoadNextStage()
    {
        _currentStageIndex++;
        if (_currentStageIndex == _stageCount)
        {
            _currentLevel++;
            LoadLevel();
        }
        else
        {
            LoadStageFromJson(_levelJson[_currentLevel].text);
        }
    }

    public float GetLevelTime()
    {
        return _levelTime;
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

            if (pieceData.Position == null || pieceData.Position.Length < 2)
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
            Piece piece;

            if (pieceData.IsSlot)
            {
                pieceObj = new GameObject("Slot Piece_" + slotLegoCount);
                piece = pieceObj.AddComponent<SlotPiece>();
                slotLegoCount++;
            }
            else
            {
                pieceObj = new GameObject("Def Piece_" + defLegoCount);
                piece = pieceObj.AddComponent<DefPiece>();
                defLegoCount++;
            }
             
            pieceObj.transform.position = new Vector3(pieceData.Position[0], pieceData.Position[1], pieceData.Position[2]);

            List<List<int>> shapeList = new List<List<int>>();
            ConvertToShapeList(pieceData, ref shapeList);

            // Convert string to enum (ensure enum names match exactly)
            PieceColor pieceColor = (PieceColor)Enum.Parse(typeof(PieceColor), pieceData.Color);

            piece.Initialize(pieceColor, shapeList, pieceData.Screwed, pieceData.IsSlot);
        }
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
