using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    public static Action OnTimeFrameChanged;
    public static Action OnDayChanged;

    public static string[] TimeFrame = { "Morning", "Midday", "Early Evening", "Late Evening", "Night" };
    public static int Day; 

    public static int TimeFrameIndex;
    private bool allowUpdates;
    private GameObject CD;
    public GameObject TimeGUI;


    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        TimeGUI.SetActive(true);
        
    }

    private void OnEnable()
    {
        OnTimeFrameChanged += UpdateTimeFrame;
        ContinuousData.PreSceneChange += UpdateAndStoreTime;
        ContinuousData.NewSceneLoaded += NewSceneResets; 
    }

    private void OnDisable()
    {
        OnTimeFrameChanged -= UpdateTimeFrame;
        ContinuousData.PreSceneChange -= UpdateAndStoreTime;
        ContinuousData.NewSceneLoaded -= NewSceneResets;
    }

    void Update()
    {
        CD = GameObject.Find("ContinuousDataObj");
        if (allowUpdates)
        {
            TimeFrameIndex = ContinuousData.instance.CDtimeIndex;
            Day = ContinuousData.instance.CDdayIndex;
        }
    }

    private void UpdateTimeFrame()
    {
        TimeFrameIndex++;
         
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

    public void ResetTimeForNewDay()
    {
            TimeFrameIndex = 0;
            Day++;
            OnDayChanged?.Invoke();
            
    }
}
