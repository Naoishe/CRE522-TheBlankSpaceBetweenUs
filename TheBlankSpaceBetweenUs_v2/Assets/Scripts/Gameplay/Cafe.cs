using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;
public class Cafe : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    public YarnProject[] yarnProjects;
    public bool allowExit;
    public GameObject playerObj;
    private bool _dialogueStartScheduled;
    private bool _dialogueStarted;
    private int _runnerCommandsRegisteredFor = int.MinValue;

    public void Awake()
    {
        // Initialize player reference only; dialogue binding happens after scene refresh to avoid runner teardown races.
        playerObj = GameObject.Find("PlayerObj");
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
        // Exit the cafe if allowed, routing based on time of day
        var cd = ContinuousData.instance;
        if (cd == null)
            return;
        if (allowExit)
        {
            if (cd.CDtimeIndex < 3)
            {
                cd.SceneChangeDetected("CampusGrounds", new Vector3(9.5f, 41.5f, 0f));
            }
            else
            {
                cd.UpdateNextScene("CampusGrounds");
                cd.SceneLoad(new Vector3(9.5f, 41.5f, 0f));
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
        if (!string.Equals(scene.name, "Cafe", System.StringComparison.Ordinal))
            return;
        ScheduleDialogueStartIfNeeded();
    }

    private void ScheduleDialogueStartIfNeeded()
    {
        if (_dialogueStarted || _dialogueStartScheduled)
            return;
        _dialogueStartScheduled = true;
        StartCoroutine(StartCafeDialogueWhenReady());
    }

    private System.Collections.IEnumerator StartCafeDialogueWhenReady()
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            var dr = ResolveDialogueRunner();
            if (dr == null || !dr)
            {
                yield return null;
                continue;
            }

            if (!BindCafeProject(dr))
            {
                yield return null;
                continue;
            }

            RegisterCafeCommandsIfNeeded(dr);

            string startNode = GetCafeStartNode();
            if (string.IsNullOrEmpty(startNode))
            {
                // Some days intentionally have no cafe dialogue.
                _dialogueStartScheduled = false;
                yield break;
            }

            _dialogueStarted = true;
            dr.StartDialogue(startNode);
            yield break;
        }

        _dialogueStartScheduled = false;
        Debug.LogWarning("Cafe: Failed to start dialogue after retries.", this);
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

    private bool BindCafeProject(DialogueRunner dr)
    {
        if (yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
        {
            Debug.LogError("Cafe: Assign Cafe0 YarnProject on Cafe scene manager.", this);
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

    private void RegisterCafeCommandsIfNeeded(DialogueRunner dr)
    {
        if (dr == null || !dr)
            return;
        int runnerId = dr.GetInstanceID();
        if (_runnerCommandsRegisteredFor == runnerId)
            return;
        dr.AddCommandHandler("allowExit", AllowExit);
        _runnerCommandsRegisteredFor = runnerId;
    }

    private string GetCafeStartNode()
    {
        var cd = ContinuousData.instance;
        if (cd == null)
            return null;
        if (cd.CDdayIndex == 0) return "SalemDay0Start";
        if (cd.CDdayIndex == 1) return "SalemDay1Start";
        if (cd.CDdayIndex == 2) return "Salem2Start";
        if (cd.CDdayIndex == 3) return "Salem3Start";
        if (cd.CDdayIndex == 4) return "Salem4";
        if (cd.CDdayIndex == 5) return "Salem5";
        if (cd.CDdayIndex == 6 && cd.SalemRP > 0 && cd.EndingIndex == -1) return "Salem6Cafe";
        return null;
    }
    
    public void AllowExit()
    {
        // Stop dialogue and trigger scene change to campus grounds
        if (dialogueRunner != null && dialogueRunner)
            dialogueRunner.Stop();
        if (ContinuousData.instance != null)
            ContinuousData.instance.SceneChangeDetected("CampusGrounds", new Vector3(9.5f, 41.5f, 0f));
    }
}
