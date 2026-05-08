using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Yarn;
using Yarn.Unity;

public class PlayerHouse : MonoBehaviour
{
    public Collider2D playerCollider;
    public Collider2D leavingCollider;
    public Collider2D toDownStairs;
    public Collider2D toUpStairs;
    public GameObject player;
    public GameObject screenCover;
    public Animator bed;

    public bool breakfastDone;

    public YarnProject[] yarnProjects;
    public DialogueRunner dialogueRunner;
    void Awake()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.SetProject(yarnProjects[0]);

    }

    void OnEnable()
    {
        if (ContinuousData.instance.CDtimeIndex <= 3)
        {
            MorningLoad();
        }
        else
        {
            NightLoad();
        }

        TimeManager.OnDayChanged += MorningLoad;

    }

    private void OnDisable()
    {
        TimeManager.OnDayChanged -= MorningLoad;
    }
    void Update()
    {

        if (Physics2D.IsTouching(leavingCollider, playerCollider))
        {
            ContinuousData.instance.SceneChangeDetected("Midday",ContinuousData.instance.campusGrounds_BridgeSpawn);
            LeavingForClass();
        }
        if (Physics2D.IsTouching(toDownStairs, playerCollider))
        {
            player.transform.position = new Vector3(8.1f, -18.9f, 0f);
        }
        if (Physics2D.IsTouching(toUpStairs, playerCollider))
        {
            player.transform.position = new Vector3(-12.3f, -1.9f, 0f);
        }

    }

    void LeavingForClass()
    {
        screenCover.SetActive(true); 
        dialogueRunner.StartDialogue("LeavingForClass");
    }

    void MorningLoad()
    {
        player.transform.position = new Vector3(-6.7f, -0.2f, 0f);
        TurnOffPlayer();
        breakfastDone = false;
        if (ContinuousData.instance.CDdayIndex == 0 ) //
        {
            ContinuousData.instance.newGame = false;
            StartCoroutine(Morning0());
        }
         else
        {
            StartCoroutine(MorningNorm());
        }
    }

    public IEnumerator Morning0()
    {
        yield return new WaitForSeconds(2f);
        dialogueRunner.StartDialogue("IntroDialogue");
        yield return new WaitForSeconds(10f);
        bed.SetTrigger("WakePlayer");
        yield return new WaitForSeconds(10f);
        TurnOnPlayer();

    }

    public IEnumerator MorningNorm()
    {
        yield return new WaitForSeconds(2f);
        dialogueRunner.StartDialogue("MorningNorm");
        yield return new WaitForSeconds(10f);
        bed.SetTrigger("WakePlayer");
        yield return new WaitForSeconds(8f);
        TurnOnPlayer();
    }

    void NightLoad() 
    {
        player.transform.position = new Vector3(-2.4f, -28.5f, 0f);
        breakfastDone = true;

    }

    public void UpdateBreakfastStatus(bool boolState)
    {
        breakfastDone = boolState;
    }

    private void TurnOffPlayer()
    {
        player.GetComponent<Animator>().SetBool("Invisible", true);
    }

    private void TurnOnPlayer()
    {
        player.GetComponent<Animator>().SetBool("Invisible", false);
    }


}
