using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TickerControl : MonoBehaviour
{
    
    public Animator tickerAnim;
    public Collider2D tickerTipCollider;
    public Collider2D safeZoneCollider;

    public void Awake()
    {
        tickerAnim = GetComponent<Animator>();
        tickerAnim.SetFloat("tickerSpeedMultiplier", 1);
        tickerAnim=GetComponent<Animator>();
    }

    public void OnEnable()
    {
        Player.OnMinigameInput += InputDetected;
    }

    public void OnDisable()
    {
        Player.OnMinigameInput -= InputDetected;
    }

    private void InputDetected()
    {
        tickerAnim.SetFloat("tickerSpeedMultiplier", 0);
        testCollision();
    }

    private void testCollision()
    {
       if(Physics2D.IsTouching(safeZoneCollider, tickerTipCollider))
        {
            Debug.Log("Success!");
        }
        else
        {
            Debug.Log("Missed.");
        }
    }

}
