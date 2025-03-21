using System.Collections;
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
        _timerUI = PuzzleTimer.Instance;
        LevelManager.Instance.LoadLevel();
        _timerUI.InitTimer();
    }

    private void CheckWinCondition()
    {
        completedSlotCount++;

        if (LevelManager.Instance.SlotPieceCountToComplete == completedSlotCount)
        {
            _timerUI.DisplayWinText();
            StartCoroutine(LoadNext());
        }
    }

    private IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(1f);
        _timerUI.ContinueTimer();
        completedSlotCount = 0;
        LevelManager.Instance.LoadNextStage();
    }

    public void ReloadLevel()
    {
        LevelManager.Instance.LoadLevel();
        _timerUI.InitTimer();
    }
}
