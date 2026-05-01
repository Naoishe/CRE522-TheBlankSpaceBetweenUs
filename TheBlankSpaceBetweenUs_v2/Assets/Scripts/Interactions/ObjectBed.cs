using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBed : InteractableObject
{
    private bool nightTime;
    public Animator playeranim;

    public new void OnEnable()
    {
        Player.OnInteractionEnabled += InteractionActivated;
        NightCheck();
        if (!nightTime)
        {
            this.GetComponent<Animator>().SetTrigger("MorningStarted");
            StartCoroutine(AppearPlayer());
        }

    }
    public new void OnDisable()
    {
        Player.OnInteractionEnabled -= InteractionActivated;
    }
    public override void Interaction()
    {
        //dialogueRunner.StartDialogue("");
        playeranim.SetTrigger("Invisible");
        this.GetComponent<Animator>().SetTrigger("PlayerSleep");
    }

    public override void EndSpecifics()
    {
        NightCheck();
        if (!nightTime)
        {
            this.GetComponent<Animator>().SetTrigger("MorningStarted");
            StartCoroutine(AppearPlayer());
        }
    }

    private IEnumerator AppearPlayer()
    {
        yield return new WaitForSeconds(8f);
        playeranim.SetTrigger("Visible");
    }

    public void NightCheck()
    {
        if (ContinuousData.instance.CDtimeIndex > 3)
        {
            nightTime = true;
        }
        else
        {
            nightTime = false;
        }
    }
}
