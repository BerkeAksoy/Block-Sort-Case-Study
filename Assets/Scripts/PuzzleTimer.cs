using UnityEngine;
using System;
using DG.Tweening;

public class PuzzleTimer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _timerText;
    [SerializeField] private float _warningStart = 30f; // Default to 30 seconds

    // Accumulator for measuring 1-second intervals
    private float secondAccumulator = 0f;
    private float currentTime;
    private bool timerRunning;

    public event Action OnTimerEnd;

    private void OnEnable()
    {
        TouchManager.Instance.OnFirstTouch += StartTimer;
    }

    private void OnDisable()
    {
        TouchManager.Instance.OnFirstTouch -= StartTimer;
    }

    public void InitTimer()
    {
        currentTime = LevelManager.Instance.GetLevelTime();
        UpdateTimerUI();
    }

    public void StartTimer()
    {
        currentTime = LevelManager.Instance.GetLevelTime();
        timerRunning = true;
        secondAccumulator = 0f;
        _timerText.transform.localScale = Vector3.one;
        UpdateTimerUI();
    }

    void Update()
    {
        if (!timerRunning) return;

        // Accumulate delta time
        secondAccumulator += Time.deltaTime;

        // Only decrement timer when at least 1 second has passed
        if (secondAccumulator >= 1f)
        {
            // Determine how many whole seconds have elapsed (e.g., if there was a lag spike)
            int decrementCount = Mathf.FloorToInt(secondAccumulator);
            secondAccumulator -= decrementCount;

            currentTime -= decrementCount;
            if (currentTime < 0) currentTime = 0;

            UpdateTimerUI();

            if (currentTime <= 0)
            {
                timerRunning = false;
                TimerEnd();
            }
        }
    }

    private void UpdateTimerUI()
    {
        int totalSeconds = Mathf.CeilToInt(currentTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        // Format as M:SS, e.g. "4:30"
        _timerText.text = $"{minutes}:{seconds:D2}";

        // warningStart if <= 10 seconds remain
        if (totalSeconds <= _warningStart && totalSeconds > 0)
        {
            _timerText.transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 1, 1);
            _timerText.color = Color.red;
        }
        else
        {
            _timerText.color = Color.white;
        }
    }

    private void TimerEnd()
    {
        Debug.Log("Timer ended.");
        OnTimerEnd?.Invoke();
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void ContinueTimer()
    {
        timerRunning = true;
    }

    public void DisplayWinText()
    {
        StopTimer();
        _timerText.text = "You Win";
    }
}
