using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;
    public TimeManager timeManager;
    private int timeIndex;

    private void OnEnable()
    {
        TimeManager.OnTimeFrameChanged += UpdateTime;
        timeManager = TimeManager.instance;
    }
    private void OnDisable()
    {
        TimeManager.OnTimeFrameChanged -= UpdateTime;
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
        timeIndex = TimeManager.TimeFrameIndex;
        UpdateTime();
    }

    private void UpdateTime()
    {
        timeText.text = $" {TimeManager.TimeFrame[timeIndex]} ";
        dayText.text = $"Day + {TimeManager.Day}";
    }





}

