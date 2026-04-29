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

    private bool isSubscribed=false;
    private bool gameStarted = false;

    //Serialized Inspector variables
    [SerializeField] private bool WrestlingSceneActive;
    [SerializeField] private bool DebateSceneActive;
    [SerializeField] private bool TheatreSceneActive;


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

        if(WrestlingSceneActive)
        {
            WrestlingUI.WrestlingUpdate += UpdateGameBool;
            WrestlingUI.WrestlingEnded += GameEnded;
        }
         if(DebateSceneActive)
        {
            //DebateUI.DebateUpdate += UpdateGameBool;
        }
         if(TheatreSceneActive)
        {
            //TheatreUI.TheatreUpdate += UpdateGameBool;
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
        if (WrestlingSceneActive)
        {
            WrestlingUI.WrestlingUpdate -= UpdateGameBool;
            WrestlingUI.WrestlingEnded -= GameEnded;
        }
        if (DebateSceneActive)
        {
            //DebateUI.DebateUpdate -= UpdateGameBool;
        }
        if (TheatreSceneActive)
        {
            //TheatreUI.TheatreUpdate -= UpdateGameBool;
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
        else
        {
           Debug.Log("Game Not Started");
        }
        
    }

    private void UpdateGameBool()
    {
        if (gameStarted)
        {
            gameStarted = false;
        }
        
        if (!gameStarted)
        {
            gameStarted = true;
        }
            
    }

    private void GameEnded()
    {
        gameStarted = false;
    }



    

}
