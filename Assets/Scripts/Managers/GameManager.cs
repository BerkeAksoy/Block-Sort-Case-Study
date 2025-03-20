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
        _timerUI = GameObject.FindGameObjectWithTag("UI").GetComponent<PuzzleTimer>();
        LevelManager.Instance.LoadLevel();
        _timerUI.InitTimer();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            LevelManager.Instance.LoadNextStage();
        }
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


}
