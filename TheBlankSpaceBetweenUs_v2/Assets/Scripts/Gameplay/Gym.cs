using UnityEngine;
using Yarn.Unity;

public class Gym : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    public YarnProject[] yarnProjects;
    public bool allowExit;
    public GameObject playerObj;
    public GameObject FaustDiaImage;
    public void Awake()
    {
        // Initialize the dialogue runner and player references
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        if (dialogueRunner == null || !dialogueRunner)
        {
            Debug.LogError("Gym: No DialogueRunner found in scene.", this);
            return;
        }
        if (yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
        {
            Debug.LogError("Gym: Assign Gym0 YarnProject on Gym scene manager.", this);
            return;
        }
        if (dialogueRunner.IsDialogueRunning)
            dialogueRunner.Stop();
        dialogueRunner.SetProject(yarnProjects[0]);
        playerObj = GameObject.Find("PlayerObj");
        if (FaustDiaImage == null)
            FaustDiaImage = GameObject.Find("FaustDiaImage");
        allowExit = false;
    }

    private void OnEnable()
    {
        Player.OnExitButton += HandleExitButton;
    }

    private void OnDisable()
    {
        Player.OnExitButton -= HandleExitButton;
    }

    private void HandleExitButton()
    {
        // Handle player exit input and route scene change appropriately
        if (allowExit)
        {
            if (ContinuousData.instance.CDtimeIndex < 3)
            {
                ContinuousData.instance.SceneChangeDetected("CampusGrounds", new Vector3(-19.8f, 65f, 0f));
            }
            else
            {
                ContinuousData.instance.UpdateNextScene("CampusGrounds");
                ContinuousData.instance.SceneLoad(new Vector3(-19.8f, 65f, 0f));
            }
        }
    }

    void Start()
    {
        // Register command handlers and start appropriate Faust dialogues
        dialogueRunner.AddCommandHandler("allowExit", AllowExit);
        dialogueRunner.AddCommandHandler("startEventGame", StartEventGame);
        if (ContinuousData.instance.CDdayIndex == 0)
        {
            dialogueRunner.StartDialogue("Faust0");
        }
        if (ContinuousData.instance.CDdayIndex == 1)
        {
            dialogueRunner.StartDialogue("Faust1");
        }
        if (ContinuousData.instance.CDdayIndex == 2)
        {
            if (ContinuousData.instance.FaustRP > 0 && ContinuousData.instance.playerClub == "Wrestling")
            {
                dialogueRunner.StartDialogue("Faust2Positive");
            }

            else
            {
                dialogueRunner.StartDialogue("Faust2Negative");
            }
        }
        if (ContinuousData.instance.CDdayIndex == 3)
        {
            dialogueRunner.StartDialogue("Faust3");
        }
        if (ContinuousData.instance.CDdayIndex == 4 && ContinuousData.instance.EndingIndex < 6)
        {
            DetermineFaustEnd();
        }
        else if (ContinuousData.instance.CDdayIndex == 4 && ContinuousData.instance.EndingIndex == 7)
        {
            dialogueRunner.StartDialogue("PlayerWinsEvent");
        }
        else if (ContinuousData.instance.CDdayIndex == 4 && ContinuousData.instance.EndingIndex == 8)
        {
            dialogueRunner.StartDialogue("PlayerLosesEvent");
        }
    }

    public void FixedUpdate()
    {
        // Update Faust dialogue image visibility each fixed update
        if (FaustDiaImage == null)
            FaustDiaImage = GameObject.Find("FaustDiaImage");
        if (FaustDiaImage != null && ContinuousData.instance != null)
            FaustDiaImage.SetActive(ContinuousData.instance.FaustDiaImageState);
    }

    public void AllowExit()
    {
        // Stop any running dialogue and allow the player to exit
        dialogueRunner.Stop();
        allowExit = true;

    }

    public void LoadPlayerWinAgainstFaust()
    {
        dialogueRunner.StartDialogue("PlayerWins");
    }

    public void LoadPlayerLoseAgainstFaust()
    {
        dialogueRunner.StartDialogue("PlayerLoses");
    }

    public void LoadEventWinAgainstFaust()
    {
        dialogueRunner.StartDialogue("PlayerWinsEvent");
    }

    public void LoadEventLoseAgainstFaust()
    {
        dialogueRunner.StartDialogue("PlayerLosesEvent");
    }

    public void DetermineFaustEnd()
    {
        if (ContinuousData.instance.playerClub == "Wrestling")
        {
            dialogueRunner.StartDialogue("FaustEventStart");
        }
    }

    public void StartEventGame()
    {
        ContinuousData.instance.LoadScene("Wrestling");
    }




}
