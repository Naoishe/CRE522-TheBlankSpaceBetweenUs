using UnityEngine;
using Yarn.Unity;
public class Cafe : MonoBehaviour
{
    DialogueRunner dialogueRunner;
    public YarnProject[] yarnProjects;
    public bool allowExit;
    public GameObject playerObj;
    public void Awake()
    {
        // Initialize dialogue runner and player reference
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        if (dialogueRunner == null || !dialogueRunner)
        {
            Debug.LogError("Cafe: No DialogueRunner found in scene.", this);
            return;
        }
        if (yarnProjects == null || yarnProjects.Length == 0 || yarnProjects[0] == null)
        {
            Debug.LogError("Cafe: Assign Cafe0 YarnProject on Cafe scene manager.", this);
            return;
        }
        if (dialogueRunner.IsDialogueRunning)
            dialogueRunner.Stop();
        dialogueRunner.SetProject(yarnProjects[0]);
        playerObj = GameObject.Find("PlayerObj");
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
        if (dialogueRunner == null || !dialogueRunner)
            return;
        var cd = ContinuousData.instance;
        if (cd == null)
        {
            Debug.LogError("Cafe: No ContinuousData instance found.", this);
            return;
        }

        // Register exit command and start the day's Salem dialogue where applicable
        dialogueRunner.AddCommandHandler("allowExit", AllowExit);
        string startNode = null;
        if (cd.CDdayIndex == 0) startNode = "SalemDay0Start";
        else if (cd.CDdayIndex == 1) startNode = "SalemDay1Start";
        else if (cd.CDdayIndex == 2) startNode = "Salem2Start";
        else if (cd.CDdayIndex == 3) startNode = "Salem3Start";
        else if (cd.CDdayIndex == 4) startNode = "Salem4";
        else if (cd.CDdayIndex == 5) startNode = "Salem5";
        else if (cd.CDdayIndex == 6 && cd.SalemRP > 0 && cd.EndingIndex == -1) startNode = "Salem6Cafe";

        if (!string.IsNullOrEmpty(startNode))
            dialogueRunner.StartDialogue(startNode);
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
