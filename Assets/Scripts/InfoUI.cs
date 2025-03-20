using UnityEngine;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _levelText;
    [SerializeField] private TMPro.TextMeshProUGUI _stageText;

    private LevelManager _levelManager;

    private void OnEnable()
    {
        LevelManager.Instance.OnStageLoad += UpdateInfo;
    }

    private void OnDisable()
    {
        _levelManager.OnStageLoad -= UpdateInfo;
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
}
