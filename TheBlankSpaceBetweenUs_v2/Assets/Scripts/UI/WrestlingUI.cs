using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

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
    public Animator fighterAnim;

    public int HitsLanded;
    public float tickerSpeed;

    public Slider fighterSlider;
    public Slider playerSlider;

    public DialogueRunner dialogueRunner;
    public YarnProject[] yarnProjects;


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
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.SetProject(yarnProjects[ContinuousData.instance.CDdayIndex]);
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
        audience.SetTrigger("Cheer");

        if(ContinuousData.instance.CDdayIndex == 4 && ContinuousData.instance.playerClub == "Wrestling")
        {
            StartCoroutine(OtherSceneChange());
        }
        else
        {
            StartCoroutine(DelaySceneChange());
        }

    }

    private IEnumerator DelaySceneChange()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("CampusGrounds"); //Change to Gym Scene for Faust Event
    }

    private IEnumerator OtherSceneChange()
    {
        yield return new WaitForSeconds(5f);
        ContinuousData.instance.EndingIndex = 7;
        SceneManager.LoadScene("Gym"); 
    }

    public void YouLose()
    {
        music.Stop();
        youLoseSFX.Play();
        youLose.SetActive(true);
        audience.SetTrigger("Still");
        if (ContinuousData.instance.CDdayIndex == 4 && ContinuousData.instance.playerClub == "Wrestling")
        {
            StartCoroutine(OtherOtherSceneChange());
        }
        else
        {
            StartCoroutine(DelaySceneChange());
        }
    }

    private IEnumerator OtherOtherSceneChange()
    {
        yield return new WaitForSeconds(5f);
        ContinuousData.instance.EndingIndex = 8;
        SceneManager.LoadScene("Gym");
    }

    public void TickerHit()
    {
        hitTxt.SetActive(true);
        hitTxt.GetComponent<Animator>().SetTrigger("HitLanded");
        player.GetComponent<Animator>().SetTrigger("Attack");
        hitSFX.Play();
        fighterSlider.value -= 0.34f;

        PrepareFighter();
    }

    public void TickerMiss()
    {
        missTxt.SetActive(true);
        missTxt.GetComponent<Animator>().SetTrigger("MissLanded");
        missSFX.Play();

        PrepareFighter();

        
    }

    private void FighterTurn()
    {
        fullObject.transform.position = new Vector3(0, -7, 0);
        WrestlingUpdate?.Invoke();
        
        fighter.GetComponent<Animator>().SetTrigger("Attack");
        StartCoroutine(WaitForAnimationToFinish(fighterAnim, "Attack"));

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

        ResetTicker();


    }

    public void PrepareFighter()
    {
        StartCoroutine(DelayTurn());
   
    }

    private IEnumerator DelayTurn()
    {
        yield return new WaitForSeconds(3f);
        FighterTurn();
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

    private IEnumerator WaitForAnimationToFinish(Animator anim, string stateName) //AI Generated method to debug 
    {
        if (anim == null) yield break;

        int layer = 0;
        float enterTimeout = 1.0f;
        float timer = 0f;

        // wait until the animator is in the Attack state (or timeout)
        while (!anim.GetCurrentAnimatorStateInfo(layer).IsName(stateName) && timer < enterTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // if didn't enter the state, stop
        if (!anim.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
            yield break;

        // wait until the state completes (normalizedTime >= 1)
        while (anim.GetCurrentAnimatorStateInfo(layer).IsName(stateName) &&
               anim.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1f)
        {
            yield return null;
        }

        // animation finished -> clear trigger so it won't re-enter accidentally
        anim.ResetTrigger("Attack");

        // optional: log or force a transition back to a known idle state if needed
        // Debug.Log("Fighter attack finished");
    }


    public void Deactivates()
    {
        WrestlingEnded?.Invoke();
        hitTxt.SetActive(false);
        missTxt.SetActive(false);
        fullObject.transform.position = new Vector3(0, -7, 0);
        

    }
}
