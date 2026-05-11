using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class EndedClass : MonoBehaviour
{

    private bool endingclass;
    public YarnProject[] yarnProjects;
    public DialogueRunner dialogueRunner;

    private void Awake()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        dialogueRunner.SetProject(yarnProjects[0]);
    }

    private void Start()
    {

        endingclass = false;
    }

    private void Update()
    {
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
