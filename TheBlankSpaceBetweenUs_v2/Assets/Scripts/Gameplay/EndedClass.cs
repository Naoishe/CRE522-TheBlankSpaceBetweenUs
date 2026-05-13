using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

[DefaultExecutionOrder(50)]
public class EndedClass : MonoBehaviour
{
    private bool endingclass;
    private bool _yarnProjectApplied;
    private bool _startedClassDialogue;
    private bool _classDialogueCoroutineScheduled;
    public YarnProject[] yarnProjects;
    public DialogueRunner dialogueRunner;

    private void Awake()
    {
        ConfigureClassYarnProject();
    }

    private void Start()
    {
        endingclass = false;
        ConfigureClassYarnProject();
        StartCoroutine(ConfigureClassYarnAfterFrame());
    }

    private IEnumerator ConfigureClassYarnAfterFrame()
    {
        yield return null;
        ConfigureClassYarnProject();
        yield return null;
        ConfigureClassYarnProject();
        ScheduleClassDialogueRoutineIfNeeded();
    }

    private void OnEnable()
    {
        ContinuousData.AfterYarnSceneRefresh += OnAfterYarnSceneRefresh;
    }

    private void OnDisable()
    {
        ContinuousData.AfterYarnSceneRefresh -= OnAfterYarnSceneRefresh;
    }

    private void OnAfterYarnSceneRefresh(Scene scene)
    {
        if (!isActiveAndEnabled)
            return;
        if (!string.Equals(scene.name, "Midday", System.StringComparison.Ordinal))
            return;

        ConfigureClassYarnProject();
        ScheduleClassDialogueRoutineIfNeeded();
    }

    private void ScheduleClassDialogueRoutineIfNeeded()
    {
        if (_classDialogueCoroutineScheduled || _startedClassDialogue)
            return;
        if (!_yarnProjectApplied || yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
            return;
        _classDialogueCoroutineScheduled = true;
        StartCoroutine(StartClassDialogueWhenReady());
    }

    private void ConfigureClassYarnProject()
    {
        if (yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
        {
            Debug.LogError("EndedClass: Assign at least one YarnProject on GameManager.", this);
            return;
        }

        ContinuousData.EnsureDialogueRunnerExistsInScene();
        DialogueRunner resolved = null;
        if (ContinuousData.instance != null)
        {
            ContinuousData.instance.RegisterYarnCommandHandlersIfNeeded();
            resolved = ContinuousData.instance.diaRunner;
        }
        if (resolved == null || !resolved)
            resolved = ContinuousData.FindPreferredDialogueRunner();

        // Same as PlayerHouse: scene-load cleanup can destroy the scene Dialogue System while DDOL survives.
        if (_yarnProjectApplied && (dialogueRunner == null || !dialogueRunner || dialogueRunner != resolved))
        {
            _yarnProjectApplied = false;
            _startedClassDialogue = false;
            _classDialogueCoroutineScheduled = false;
        }

        if (resolved == null || !resolved)
        {
            Debug.LogError(
                "EndedClass: No DialogueRunner found. Add Assets/Prefabs/Dialogue System Variant.prefab to this scene (or enter from a scene that already runs Yarn).",
                this);
            return;
        }

        dialogueRunner = resolved;
        if (!_yarnProjectApplied)
        {
            if (dialogueRunner.IsDialogueRunning)
                return;
            dialogueRunner.SetProject(yarnProjects[0]);
            _yarnProjectApplied = true;
        }
    }

    private IEnumerator StartClassDialogueWhenReady()
    {
        // ContinuousData.OnSceneLoadedRefreshYarnRoutine: yield null, then DestroyDuplicateDialogueKitRootsInScene.
        // If we StartDialogue on the same frame as that destroy, LinePresenter TMP is torn down mid-typewriter (TMP NRE).
        yield return null;
        yield return null;
        yield return null;

        if (_startedClassDialogue)
            yield break;
        if (yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
            yield break;

        for (int attempt = 0; attempt < 8; attempt++)
        {
            ContinuousData.EnsureDialogueRunnerExistsInScene();
            if (ContinuousData.instance != null)
            {
                ContinuousData.instance.EnsureDialogueRunnerAndStorage();
                ContinuousData.instance.RegisterYarnCommandHandlersIfNeeded();
            }

            DialogueRunner dr = ContinuousData.instance != null ? ContinuousData.instance.diaRunner : null;
            if (dr == null || !dr)
                dr = ContinuousData.FindPreferredDialogueRunner();
            if (dr == null || !dr)
            {
                yield return null;
                continue;
            }

            dialogueRunner = dr;

            if (dr.IsDialogueRunning)
            {
                dr.Stop();
                for (int wait = 0; wait < 6 && dr.IsDialogueRunning; wait++)
                    yield return null;
            }

            if (dr.YarnProject != yarnProjects[0])
            {
                if (dr.IsDialogueRunning)
                {
                    yield return null;
                    continue;
                }
                dr.SetProject(yarnProjects[0]);
            }

            string node = (ContinuousData.instance != null && ContinuousData.instance.CDdayIndex > 0)
                ? "ClassOther"
                : "Class0";

            bool nodeExists = dr.YarnProject != null
                && dr.YarnProject.NodeNames != null
                && System.Array.IndexOf(dr.YarnProject.NodeNames, node) >= 0;
            if (!nodeExists)
            {
                Debug.LogError($"EndedClass: Node '{node}' not found in YarnProject '{(dr.YarnProject != null ? dr.YarnProject.name : "<null>")}'.", this);
                yield break;
            }

            _startedClassDialogue = true;
            dr.StartDialogue(node);
            yield break;
        }

        Debug.LogWarning("EndedClass: Failed to start class dialogue after retries.", this);
    }

    private void Update()
    {
        if (ContinuousData.instance == null)
            return;

        endingclass = ContinuousData.instance.MonitorBool("$EndClass");

        if (endingclass)
        {
            SceneManager.LoadScene("CampusGrounds");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            endingclass = true;
        }
    }
}
