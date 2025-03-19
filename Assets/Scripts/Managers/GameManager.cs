using UnityEngine;

public class GameManager : BaseSingleton<GameManager>
{
    PuzzleTimer _timerUI;

    private void Start()
    {
        _timerUI = GameObject.FindGameObjectWithTag("UI").GetComponent<PuzzleTimer>();
        LevelManager.Instance.LoadLevel();
        _timerUI.InitTimer();
    }


}
