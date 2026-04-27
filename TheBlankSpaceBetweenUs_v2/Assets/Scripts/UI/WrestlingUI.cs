using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrestlingUI : MonoBehaviour
{
    public GameObject ActiveRoundSpotlight;
    public GameObject Victory;
    public GameObject YouLose;
    public GameObject Countdown;
    public GameObject HitTxt;
    public GameObject MissTxt;

    public AudioSource Music;
    public AudioSource HitSFX;
    public AudioSource MissSFX;
    public AudioSource VictorySFX;
    public AudioSource YouLoseSFX;

    public TickerControl tickerControl;

    public int HitsLanded;
    public float tickerSpeed;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        
    }

    public void OnDisable()
    {
        
    }

    private void Awake()
    {
        ActiveRoundSpotlight.SetActive(false);
        Victory.SetActive(false);
        YouLose.SetActive(false);
        Countdown.SetActive(false);
        HitTxt.SetActive(false);
        MissTxt.SetActive(false);
    }



    void Update()
    {
        
    }

    public void TickerHit()
    {
        HitTxt.GetComponent<Animator>().SetTrigger("HitLanded");
        HitSFX.Play();
    }

    public void TickerMiss()
    {
        MissTxt.GetComponent<Animator>().SetTrigger("MissLanded");
        MissSFX.Play();
    }
}
