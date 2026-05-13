using System;
using UnityEngine;

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
        // Keep a single persistent TimeManager (scenes often add another on UI prefabs).
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if (TimeGUI == null)
            TimeGUI = gameObject;
        TimeGUI.SetActive(true);
    }

    private void OnEnable()
    {
        // Subscribe to time and scene events
        OnTimeFrameChanged += UpdateTimeFrame;
        ContinuousData.PreSceneChange += UpdateAndStoreTime;
        ContinuousData.NewSceneLoaded += NewSceneResets;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        OnTimeFrameChanged -= UpdateTimeFrame;
        ContinuousData.PreSceneChange -= UpdateAndStoreTime;
        ContinuousData.NewSceneLoaded -= NewSceneResets;
    }

    void Update()
    {
        // Cache ContinuousData object and synchronize time indices when allowed
        CD = GameObject.Find("ContinuousDataObj");
        if (allowUpdates)
        {
            TimeFrameIndex = ContinuousData.instance.CDtimeIndex;
            Day = ContinuousData.instance.CDdayIndex;
        }
    }

    private void UpdateTimeFrame()
    {
        // Advance the time frame index
        TimeFrameIndex++;

    }

    private void NewSceneResets()
    {
        // Allow time synchronization after a new scene loads
        allowUpdates = true;
    }

    public void UpdateAndStoreTime()
    {
        // Advance time, prevent immediate sync, and save to ContinuousData
        OnTimeFrameChanged?.Invoke();
        allowUpdates = false;
        ContinuousData.instance.UpdateSavedTime(TimeFrameIndex, Day);
    }

    public void ResetTimeForNewDay()
    {
        // Reset time for a new day and notify listeners
        TimeFrameIndex = 0;
        Day++;
        OnDayChanged?.Invoke();

    }
}
