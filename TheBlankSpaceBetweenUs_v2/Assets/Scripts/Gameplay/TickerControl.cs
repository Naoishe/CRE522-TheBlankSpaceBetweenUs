using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Yarn.Unity;
using System.Linq.Expressions;

public class TickerControl : MonoBehaviour
{
    public Action OnHit;
    public Action OnMiss;
    public Animator tickerAnim;
    public Collider2D tickerTipCollider;
    public Collider2D safeZoneCollider;

    public GameObject player;

    public GameObject fullObject;
    public GameObject MinigameManager;

    private bool isSubscribed;
    private bool gameStarted;


    public void Awake()
    {
        tickerAnim.SetFloat("tickerSpeedMultiplier", 1);
        player = GameObject.Find("PlayerObj");
        

    }

    public void OnEnable()
    {
        if (!isSubscribed)
        {
            Player.OnMinigameInput += InputDetected;
            isSubscribed = true;
        }

        //Club Dependent Code, changes reference depending on scene's specific script

        try
        {
            if (ContinuousData.instance.currentSceneName == "Wrestling")
            {
                WrestlingUI.WrestlingUpdate += UpdateGameBool;
            }
        }
        catch
        {
            Debug.LogWarning("TickerControl: OnEnable: Failed to subscribe to Action");
        }
    }




    public void OnDisable()
    {
        if (isSubscribed)
        {
            try
            {
                Player.OnMinigameInput -= InputDetected;
            }
            catch
            {
                Debug.LogWarning("TickerControl: OnDisable: Failed to subscribe to Action");
            }
            isSubscribed = false;
        }
        try
        {
            if (ContinuousData.instance.currentSceneName == "Wrestling")
            {
                WrestlingUI.WrestlingUpdate -= UpdateGameBool;
            }
        }
        catch
        {
            Debug.LogWarning("TickerControl: OnDisable: Failed to unsubscribe from Action");
        }

    }

    private void InputDetected()
    {
        tickerAnim.SetFloat("tickerSpeedMultiplier", 0);
        testCollision();
    }

    private void testCollision()
    {
        if (gameStarted)
        {
            if (Physics2D.IsTouching(safeZoneCollider, tickerTipCollider))
            {

                OnHit?.Invoke();
            }
            else
            {

                OnMiss?.Invoke();
            }
        }
        
    }

    private void UpdateGameBool()
    {
        if (gameStarted)
        {
            gameStarted = false;
        }
        else
        {
            gameStarted = true;
        }
            
    }



    

}
