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
    public static Action WrestlingEnded;

    public GameObject activeRoundSpotlight;
    public GameObject victory;
    public GameObject youLose;
    public GameObject countdown;
    public GameObject hitTxt;
    public GameObject missTxt;
    public GameObject fighter;
    public GameObject player;
    public GameObject fullObject;

    public AudioSource music;
    public AudioSource hitSFX;
    public AudioSource missSFX;
    public AudioSource victorySFX;
    public AudioSource youLoseSFX;

    public Animator audience;

    public int HitsLanded;
    public float tickerSpeed;

    public Slider fighterSlider;
    public Slider playerSlider;

    private bool proceed = false;

    void Start()
    {
        countdown.GetComponent<Animator>().SetTrigger("StartCount");
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
        activeRoundSpotlight.SetActive(false);
        victory.SetActive(false);
        youLose.SetActive(false);
        hitTxt.SetActive(false);
        missTxt.SetActive(false);
        fullObject.transform.position = new Vector3(0, -7, 0);
    }



    void Update()
    {
        if (fighterSlider.value <= 0)
        {
            Victory();
        }
        if (playerSlider.value <= 0)
        {
            YouLose();
        }
    }

    public void Victory()
    {
        music.Stop();
        victorySFX.Play();
        victory.SetActive(true);
        StartCoroutine(DelaySceneChange());
        audience.SetTrigger("Cheer");
    }

    private IEnumerator DelaySceneChange()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("CampusGrounds"); //Change to Gym Scene
    }

    public void YouLose()
    {
        music.Stop();
        youLoseSFX.Play();
        youLose.SetActive(true);
        StartCoroutine(DelaySceneChange());
        audience.SetTrigger("Still");
    }

    public void TickerHit()
    {
        hitTxt.SetActive(true);
        hitTxt.GetComponent<Animator>().SetTrigger("HitLanded");
        player.GetComponent<Animator>().SetTrigger("Attack");
        hitSFX.Play();
        fighterSlider.value -= 0.34f;

        FighterTurn();
    }

    public void TickerMiss()
    {
        missTxt.SetActive(true);
        missTxt.GetComponent<Animator>().SetTrigger("MissLanded");
        missSFX.Play();

        FighterTurn();

        
    }

    private void FighterTurn()
    {
        fullObject.transform.position = new Vector3(0, -7, 0);
        WrestlingUpdate?.Invoke();
        fighter.GetComponent<Animator>().SetTrigger("Attack");
        StartCoroutine(FighterAttackFX());



    }

    public IEnumerator FighterAttackFX()
    {
        yield return new WaitForSeconds(0.5f);
        var randomNum = new System.Random();
        int randomInt = randomNum.Next(1, 4);
        if (randomInt == 3) //Randomises a 1 in three chance to miss
        {

            missTxt.GetComponent<Animator>().SetTrigger("MissLanded");
            missSFX.Play();
        }
        else
        {
            hitTxt.GetComponent<Animator>().SetTrigger("HitLanded");
            hitSFX.Play();
            playerSlider.value -= 0.2f;
        }


    }

    public void PrepareFighter()
    {
        StartCoroutine(DelayTurn());

        if (proceed)
        {
            FighterTurn();
        }
            
    }

    private IEnumerator DelayTurn()
    {
        yield return new WaitForSeconds(3f);
        proceed = true;
    }

    public void ResetTicker()
    {
        StartCoroutine(ActivateTicker());
    }

    public IEnumerator ActivateTicker()
    {
        yield return new WaitForSeconds(2f);
        activeRoundSpotlight.SetActive(true);
        yield return new WaitForSeconds(1f);
        fullObject.transform.position= new Vector3(0, -3, 0);
        WrestlingUpdate?.Invoke();
    }

    public void Deactivates()
    {
        WrestlingEnded?.Invoke();
        hitTxt.SetActive(false);
        missTxt.SetActive(false);
        fullObject.transform.position = new Vector3(0, -7, 0);
        

    }
}
