using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _levelText;
    [SerializeField] private TMPro.TextMeshProUGUI _stageText;
    [SerializeField] private Button _replayButton;

    private LevelManager _levelManager;

    private void OnEnable()
    {
        LevelManager.Instance.OnStageLoad += UpdateInfo;
        PuzzleTimer.Instance.OnTimerEnd += OpenReplayButton;
    }

    private void OnDisable()
    {
        _levelManager.OnStageLoad -= UpdateInfo;
        PuzzleTimer.Instance.OnTimerEnd -= OpenReplayButton;
    }

    private void Start()
    {
        _levelManager = LevelManager.Instance;
    }

    private void UpdateInfo()
    {
        _levelText.text = "Level " + _levelManager.GetCurrentLevel();
        _stageText.text = "Stage " + _levelManager.GetCurrentStage() + "/" + _levelManager.GetStageCount();
    }

    private void OpenReplayButton()
    {
        _replayButton.gameObject.SetActive(true);
    }

    public void CallReloadLevel()
    {
        GameManager.Instance.ReloadLevel();
        _replayButton.gameObject.SetActive(false);
    }
}
