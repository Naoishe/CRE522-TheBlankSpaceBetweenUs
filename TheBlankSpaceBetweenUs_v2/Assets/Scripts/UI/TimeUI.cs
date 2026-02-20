using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Timers;
using TMPro;

public class TimeUI : MonoBehaviour
{
    
    public TextMeshProUGUI timeText;
    public Animator sundialAnimator;
    private float staticTimeFeedingVar;
    //public TimeManager timeManager;
    private int timeIndex;
    private int day;
    private string timeframe;
    private Timer staticTimer;

    private void OnEnable()
    {
        TimeManager.OnTimeFrameChanged += SundialStatic;
    }
    private void OnDisable()
    {
        TimeManager.OnTimeFrameChanged -= SundialStatic;
    }

    public void Update()
    {
        timeIndex = TimeManager.TimeFrameIndex;  
        day= TimeManager.Day;
        timeframe = TimeManager.TimeFrame[timeIndex];
        UpdateTime();
        UpdateSundial();

    }
    
    private void UpdateTime()
    {
        timeText.text = $"Day {day}: {TimeManager.TimeFrame[timeIndex]}";
        //Debug.Log("TimeUpdated");
        //Debug.Log("timeIndex = " + timeIndex);
        //Debug.Log("Day = " + day);
    }

    private void UpdateSundial()
    {
        if (timeframe == "Morning")
        {
            sundialAnimator.SetTrigger("Morning");
        }
        if (timeframe == "Midday")
        {
            sundialAnimator.SetTrigger("Midday");
        }
        if (timeframe == "Early Evening")
        {
            sundialAnimator.SetTrigger("EarlyEvening");
        }
        if (timeframe == "Late Evening")
        {
            sundialAnimator.SetTrigger("LateEvening");
        }
        if (timeframe == "Night")
        {
            sundialAnimator.SetTrigger("Night");
        }
    }

    private void SundialStatic()
    {
        sundialAnimator.SetTrigger("Static");
        staticTimer = new Timer(2500); 
        staticTimer.Start();
        //staticTimer.Elapsed += StopStatic;

        
    }

    private void StopStatic(ElapsedEventArgs e)
    {
        Console.WriteLine("Time Returned"+e.SignalTime);
        sundialAnimator.SetTrigger("StaticReset");
    }

    
 

}

