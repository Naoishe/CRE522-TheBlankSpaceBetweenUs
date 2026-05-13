using System.Collections;
using UnityEngine;
using Yarn.Unity;

[DefaultExecutionOrder(10)]
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

    private bool _yarnProjectApplied;

    private void ConfigureYarnForHouse()
    {
        ContinuousData.EnsureDialogueRunnerExistsInScene();
        DialogueRunner resolved = null;
        if (ContinuousData.instance != null)
        {
            ContinuousData.instance.RegisterYarnCommandHandlersIfNeeded();
            resolved = ContinuousData.instance.diaRunner;
        }
        if (resolved == null || !resolved)
            resolved = ContinuousData.FindPreferredDialogueRunner();

        // OnSceneLoadedRefreshYarn may destroy a scene-level duplicate kit; our cached runner then dies while the flag stays true.
        if (_yarnProjectApplied && (dialogueRunner == null || !dialogueRunner || dialogueRunner != resolved))
            _yarnProjectApplied = false;

        if (_yarnProjectApplied)
            return;

        if (resolved == null || !resolved)
        {
            Debug.LogError("PlayerHouse: DialogueRunner is still missing after EnsureDialogueRunnerExistsInScene. Assign dialogueRunner in Inspector or install Yarn Spinner.", this);
            return;
        }

        dialogueRunner = resolved;

        if (yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
        {
            Debug.LogError("PlayerHouse: yarnProjects must have at least one YarnProject assigned in the Inspector.", this);
            return;
        }

        dialogueRunner.SetProject(yarnProjects[0]);
        _yarnProjectApplied = true;
    }

    void Awake()
    {
        // Before OnEnable so morning dialogue coroutines always see a configured runner when possible.
        ConfigureYarnForHouse();
    }

    void Start()
    {
        // Retry after all Awakes (e.g. if DialogueRunner was created very late in the frame).
        ConfigureYarnForHouse();
        if (!_yarnProjectApplied)
            StartCoroutine(ConfigureYarnAfterDataInit());
    }

    private IEnumerator ConfigureYarnAfterDataInit()
    {
        yield return null;
        ConfigureYarnForHouse();
    }

    void OnEnable()
    {
        // Setup scene-specific player placement based on time and subscribe to day changes
        int timeIndex = ContinuousData.instance != null ? ContinuousData.instance.CDtimeIndex : 0;
        if (timeIndex <= 3)
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
            if (ContinuousData.instance != null)
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
        ConfigureYarnForHouse();
        if (dialogueRunner == null || !_yarnProjectApplied)
        {
            Debug.LogError("PlayerHouse: Cannot start LeavingForClass — DialogueRunner or Yarn project not configured.", this);
            return;
        }
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
        if (ContinuousData.instance == null || ContinuousData.instance.CDdayIndex == 0) //
        {
            if (ContinuousData.instance != null)
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
        ConfigureYarnForHouse();
        if (dialogueRunner == null || !_yarnProjectApplied)
            yield break;
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
        ConfigureYarnForHouse();
        if (dialogueRunner == null || !_yarnProjectApplied)
            yield break;
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
