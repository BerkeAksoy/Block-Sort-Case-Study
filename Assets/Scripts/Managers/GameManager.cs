using UnityEngine;

public class GameManager : BaseSingleton<GameManager>
{
    PuzzleTimer _timerUI;
    private int completedSlotCount = 0;

    private void OnEnable()
    {
        DraggablePieceManager.OnSlotCompleted += CheckWinCondition;
    }

    private void OnDisable()
    {
        DraggablePieceManager.OnSlotCompleted -= CheckWinCondition;
    }

    private void Start()
    {
        _timerUI = GameObject.FindGameObjectWithTag("UI").GetComponent<PuzzleTimer>();
        LevelManager.Instance.LoadLevel();
        _timerUI.InitTimer();
    }

    private void CheckWinCondition()
    {
        completedSlotCount++;
        if (LevelManager.Instance.SlotPieceCountToComplete == completedSlotCount)
        {
            _timerUI.DisplayWinText();
        }
    }


}
