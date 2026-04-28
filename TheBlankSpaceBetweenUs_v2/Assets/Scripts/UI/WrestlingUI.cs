using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WrestlingUI : MonoBehaviour
{
    public static Action WrestlingUpdate;
    public GameObject ActiveRoundSpotlight;
    public GameObject Victory;
    public GameObject YouLose;
    public GameObject Countdown;
    public GameObject HitTxt;
    public GameObject MissTxt;
    public GameObject Fighter;


    public GameObject FullObject;

    public AudioSource Music;
    public AudioSource HitSFX;
    public AudioSource MissSFX;
    public AudioSource VictorySFX;
    public AudioSource YouLoseSFX;

    public int HitsLanded;
    public float tickerSpeed;

    public Slider fighterSlider;
    public Slider playerSlider;

    void Start()
    {
        Countdown.GetComponent<Animator>().SetTrigger("StartCount");
        StartCoroutine(ActivateTicker());
        fighterSlider.value = 1;
        playerSlider.value= 1;

    }

    private void OnEnable()
    {
        this.GetComponent<TickerControl>().OnHit += TickerHit;
        this.GetComponent<TickerControl>().OnMiss += TickerMiss;
    }

    public void OnDisable()
    {
        this.GetComponent<TickerControl>().OnHit -= TickerHit;
        this.GetComponent<TickerControl>().OnMiss -= TickerMiss;
    }

    private void Awake()
    {
        ActiveRoundSpotlight.SetActive(false);
        Victory.SetActive(false);
        YouLose.SetActive(false);
        HitTxt.SetActive(false);
        MissTxt.SetActive(false);
        FullObject.transform.position = new Vector3(0, -7, 0);
    }



    void Update()
    {
        if (fighterSlider.value <= 0)
        {
            Victory.SetActive(true);
        }
        if (playerSlider.value <= 0)
        {
            YouLose.SetActive(true);
        }
    }

    public void TickerHit()
    {
        HitTxt.SetActive(true);
        HitTxt.GetComponent<Animator>().SetTrigger("HitLanded");
        HitSFX.Play();
        fighterSlider.value -= 0.34f;

        FighterTurn();
    }

    public void TickerMiss()
    {
        MissTxt.SetActive(true);
        MissTxt.GetComponent<Animator>().SetTrigger("MissLanded");
        MissSFX.Play();

        FighterTurn();

        
    }

    private void FighterTurn()
    {
        FullObject.transform.position = new Vector3(0, -7, 0);
        WrestlingUpdate?.Invoke();
        Fighter.GetComponent<Animator>().SetTrigger("Attack");
        StartCoroutine(FighterAttackFX());



    }

    public IEnumerator FighterAttackFX()
    {
        yield return new WaitForSeconds(0.5f);
        var randomNum = new System.Random();
        int randomInt = randomNum.Next(1, 4);
        Debug.Log("Random Number: "+ randomInt);
        if (randomInt == 3) //Randomises a 1 in three chance to miss
        {

            MissTxt.GetComponent<Animator>().SetTrigger("MissLanded");
            MissSFX.Play();
        }
        else
        {
            HitTxt.GetComponent<Animator>().SetTrigger("HitLanded");
            HitSFX.Play();
            playerSlider.value -= 0.2f;
        }


    }

    public IEnumerator ActivateTicker()
    {
        yield return new WaitForSeconds(2f);
        ActiveRoundSpotlight.SetActive(true);
        yield return new WaitForSeconds(1f);
        FullObject.transform.position= new Vector3(0, -3, 0);
        WrestlingUpdate?.Invoke();
    }
}
