using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
public class Cafe : MonoBehaviour
{
    DialogueRunner dialogueRunner;

    public bool allowExit;

    public GameObject playerObj;
    public void Awake()
    {
        dialogueRunner = GetComponent<DialogueRunner>();
        playerObj= GameObject.Find("PlayerObj");
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
        if(allowExit)
        {
            ContinuousData.instance.SceneChangeDetected("CampusGrounds", new Vector3(9.5f,41.5f,0f));
        }
    }

    void Start()
    {
        if (ContinuousData.instance.CDdayIndex==0)
        {
            dialogueRunner.StartDialogue("SalemDay0Start");
        }
        if (ContinuousData.instance.CDdayIndex == 1)
        {
            dialogueRunner.StartDialogue("SalemDay1Start");
        }
         if (ContinuousData.instance.CDdayIndex == 2)
        {
            dialogueRunner.StartDialogue("SalemDay2Start");
        }
    }
     void Update()
    {

    }

    [YarnCommand("allowExit")]
    public void AllowExit()
    {
        dialogueRunner.Stop();
        allowExit = true;
    }
}
