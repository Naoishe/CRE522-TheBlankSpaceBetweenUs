using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

public class TimeManager : MonoBehaviour
{
    public static Action OnTimeFrameChanged;
    public static Action OnDayChanged;

    public static string[] TimeFrame = { "Morning", "Midday", "Early Evening", "Late Evening", "Night" };
    public static int Day; 

    public static int TimeFrameIndex;
    private bool allowUpdates;
    private GameObject CD;
    public GameObject TimeGUI;

    void Start()
    {
        TimeGUI.SetActive(true);
        
    }

    private void OnEnable()
    {
        OnTimeFrameChanged += UpdateTimeFrame;
        Day1Control.PreSceneChange += UpdateAndStoreTime;
        Day1Control.NewSceneLoaded += NewSceneResets; 
    }

    private void OnDisable()
    {
        OnTimeFrameChanged -= UpdateTimeFrame;
    }

    void Update()
    {
        CD = GameObject.Find("ContinuousDataObj");
        if (!allowUpdates)
        {
            TimeFrameIndex = ContinuousData.instance.CDtimeIndex;
            Day = ContinuousData.instance.CDdayIndex;

        }
    }

    private void UpdateTimeFrame()
    {
        if (TimeFrameIndex == 4)
        {
            TimeFrameIndex = 0;
            Day++;
            OnDayChanged?.Invoke();
            //Debug.Log("Current day:" + Day + " Current TimeframeIndex: " + TimeFrameIndex);
        }
        else
        {
            TimeFrameIndex++;
        }
        
    }

    private void NewSceneResets()
    {
        allowUpdates= true;
    }

    public void UpdateAndStoreTime()
    {
        OnTimeFrameChanged?.Invoke();
        allowUpdates = false;
        ContinuousData.instance.UpdateSavedTime(TimeFrameIndex, Day);
    }
}
