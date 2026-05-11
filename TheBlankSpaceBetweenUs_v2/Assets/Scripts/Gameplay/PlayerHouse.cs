using System.Collections;
using UnityEngine;
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
        // Initialize DialogueRunner and assign the relevant Yarn project
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.SetProject(yarnProjects[0]);

    }

    void OnEnable()
    {
        // Setup scene-specific player placement based on time and subscribe to day changes
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

        // Monitor colliders to trigger scene transitions or local movement
        if (Physics2D.IsTouching(leavingCollider, playerCollider))
        {
            ContinuousData.instance.SceneChangeDetected("Midday", ContinuousData.instance.campusGrounds_BridgeSpawn);
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
        // Show screen cover and start the 'leaving for class' dialogue
        screenCover.SetActive(true);
        dialogueRunner.StartDialogue("LeavingForClass");
    }

    void MorningLoad()
    {
        // Position player for morning and start the appropriate morning sequence
        player.transform.position = new Vector3(-6.7f, -0.2f, 0f);
        TurnOffPlayer();
        breakfastDone = false;
        if (ContinuousData.instance.CDdayIndex == 0) //
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
        // Play the normal morning sequence for subsequent days
        yield return new WaitForSeconds(2f);
        dialogueRunner.StartDialogue("MorningNorm");
        yield return new WaitForSeconds(10f);
        bed.SetTrigger("WakePlayer");
        yield return new WaitForSeconds(8f);
        TurnOnPlayer();
    }

    void NightLoad()
    {
        // Position player for nighttime
        player.transform.position = new Vector3(-2.4f, -28.5f, 0f);
        breakfastDone = true;

    }

    public void UpdateBreakfastStatus(bool boolState)
    {
        // Update the breakfast completion flag
        breakfastDone = boolState;
    }

    private void TurnOffPlayer()
    {
        // Make the player invisible via animator
        player.GetComponent<Animator>().SetBool("Invisible", true);
    }

    private void TurnOnPlayer()
    {
        // Make the player visible via animator
        player.GetComponent<Animator>().SetBool("Invisible", false);
    }


}
