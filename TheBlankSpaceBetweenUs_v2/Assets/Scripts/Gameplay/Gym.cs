using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class Gym : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    public YarnProject[] yarnProjects;
    public bool allowExit;
    public GameObject playerObj;
    public GameObject FaustDiaImage;
    private bool _dialogueStartScheduled;
    private bool _dialogueStarted;
    private int _runnerCommandsRegisteredFor = int.MinValue;

    public void Awake()
    {
        // Initialize scene refs; dialogue setup runs after scene refresh to avoid runner teardown races.
        playerObj = GameObject.Find("PlayerObj");
        if (FaustDiaImage == null)
            FaustDiaImage = GameObject.Find("FaustDiaImage");
        allowExit = false;
    }

    private void OnEnable()
    {
        Player.OnExitButton += HandleExitButton;
        ContinuousData.AfterYarnSceneRefresh += OnAfterYarnSceneRefresh;
    }

    private void OnDisable()
    {
        Player.OnExitButton -= HandleExitButton;
        ContinuousData.AfterYarnSceneRefresh -= OnAfterYarnSceneRefresh;
    }

    private void HandleExitButton()
    {
        // Handle player exit input and route scene change appropriately
        var cd = ContinuousData.instance;
        if (cd == null)
            return;
        if (allowExit)
        {
            if (cd.CDtimeIndex < 3)
            {
                cd.SceneChangeDetected("CampusGrounds", new Vector3(-19.8f, 65f, 0f));
            }
            else
            {
                cd.UpdateNextScene("CampusGrounds");
                cd.SceneLoad(new Vector3(-19.8f, 65f, 0f));
            }
        }
    }

    void Start()
    {
        ScheduleDialogueStartIfNeeded();
    }

    private void OnAfterYarnSceneRefresh(Scene scene)
    {
        if (!isActiveAndEnabled)
            return;
        if (!string.Equals(scene.name, "Gym", System.StringComparison.Ordinal))
            return;
        ScheduleDialogueStartIfNeeded();
    }

    private void ScheduleDialogueStartIfNeeded()
    {
        if (_dialogueStarted || _dialogueStartScheduled)
            return;
        _dialogueStartScheduled = true;
        StartCoroutine(StartGymDialogueWhenReady());
    }

    private System.Collections.IEnumerator StartGymDialogueWhenReady()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            var dr = ResolveDialogueRunner();
            if (dr == null || !dr)
            {
                yield return null;
                continue;
            }

            if (!BindGymProject(dr))
            {
                yield return null;
                continue;
            }

            RegisterGymCommandsIfNeeded(dr);

            string startNode = GetGymStartNode();
            if (string.IsNullOrEmpty(startNode))
            {
                _dialogueStartScheduled = false;
                yield break;
            }

            _dialogueStarted = true;
            dr.StartDialogue(startNode);
            yield break;
        }

        _dialogueStartScheduled = false;
        Debug.LogWarning("Gym: Failed to start dialogue after retries.", this);
    }

    private DialogueRunner ResolveDialogueRunner()
    {
        ContinuousData.EnsureDialogueRunnerExistsInScene();
        var cd = ContinuousData.instance;
        if (cd != null)
        {
            cd.EnsureDialogueRunnerAndStorage();
            cd.RegisterYarnCommandHandlersIfNeeded();
            if (cd.diaRunner != null && cd.diaRunner)
            {
                dialogueRunner = cd.diaRunner;
                return dialogueRunner;
            }
        }

        dialogueRunner = ContinuousData.FindPreferredDialogueRunner();
        if (dialogueRunner == null || !dialogueRunner)
            dialogueRunner = FindObjectOfType<DialogueRunner>(true);
        return dialogueRunner;
    }

    private bool BindGymProject(DialogueRunner dr)
    {
        if (yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
        {
            Debug.LogError("Gym: Assign Gym0 YarnProject on Gym scene manager.", this);
            return false;
        }

        if (dr.YarnProject == yarnProjects[0])
            return true;

        if (dr.IsDialogueRunning)
        {
            dr.Stop();
            return false;
        }

        dr.SetProject(yarnProjects[0]);
        return true;
    }

    private void RegisterGymCommandsIfNeeded(DialogueRunner dr)
    {
        if (dr == null || !dr)
            return;
        int runnerId = dr.GetInstanceID();
        if (_runnerCommandsRegisteredFor == runnerId)
            return;
        dr.AddCommandHandler("allowExit", AllowExit);
        dr.AddCommandHandler("startEventGame", StartEventGame);
        _runnerCommandsRegisteredFor = runnerId;
    }

    private string GetGymStartNode()
    {
        var cd = ContinuousData.instance;
        if (cd == null)
            return null;

        if (cd.CDdayIndex == 0) return "Faust0";
        if (cd.CDdayIndex == 1) return "Faust1";
        if (cd.CDdayIndex == 2)
        {
            if (cd.FaustRP > 0 && cd.playerClub == "Wrestling")
                return "Faust2Positive";
            return "Faust2Negative";
        }
        if (cd.CDdayIndex == 3) return "Faust3";
        if (cd.CDdayIndex == 4 && cd.EndingIndex < 6 && cd.playerClub == "Wrestling")
            return "FaustEventStart";
        if (cd.CDdayIndex == 4 && cd.EndingIndex == 7) return "PlayerWinsEvent";
        if (cd.CDdayIndex == 4 && cd.EndingIndex == 8) return "PlayerLosesEvent";
        return null;
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
        if (dialogueRunner != null && dialogueRunner)
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
        if (ContinuousData.instance != null)
            ContinuousData.instance.LoadScene("Wrestling");
    }




}
