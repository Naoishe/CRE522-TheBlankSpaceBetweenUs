using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Timers;
using TMPro;

public class TimeUI : MonoBehaviour
{
    
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;
    public Animator sundialAnimator;
    public TimeManager timeManager;
    private int timeIndex;

    private void OnEnable()
    {
        TimeManager.OnTimeFrameChanged += UpdateTime;
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

