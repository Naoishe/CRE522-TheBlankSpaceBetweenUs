using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectivesManager : MonoBehaviour
{
    public static ObjectivesManager instance;



    //Hierarchy Vars

    public GameObject objectivesIcon;

    public GameObject objectiveUpdatedNotif;
    public GameObject objectiveFailedNotif;
    public GameObject objectiveCompletedNotif;

    private GameObject objectiveTextControlled;
    public GameObject objectiveUpdatedText;
    public GameObject objectiveFailedText;
    public GameObject objectiveCompletedText;

    public TextMeshProUGUI objectiveUpdatedDesc;
    public TextMeshProUGUI objectiveFailedDesc;
    public TextMeshProUGUI objectiveCompletedDesc;

    public GameObject ObjectivesScreen;
    public GameObject ObjectivesScreenTitles;
    public TextMeshProUGUI printedTitle;
    public TextMeshProUGUI printedSummary;
    public TextMeshProUGUI printedTask;
    public bool ObjScrActive;

    //hierarchy vars for active gameplay methods

    public GameObject notifImage;
    public GameObject notifTitle;
    public TextMeshProUGUI displayedDescription;

    //Animator Vars
    public Animator animator;

    //Other Vars
    private string notificationType;


    //Objective Objects
    public Objective Objective;
    public List<Objective> activeObjectives = new List<Objective>();
    Objective objective_0;
    Objective objective_1;
    Objective objective_2;
    Objective objective_3;

    private bool disable1;
    private bool disable2;
    private bool disable3;

    private int i = 0;


    private void Awake()
    {
        // Initialize singleton, UI state and base objectives
        instance = this;
        DontDestroyOnLoad(gameObject);
        ObjScrActive = false;

        objectiveUpdatedText.SetActive(false);
        objectiveFailedText.SetActive(false);
        objectiveCompletedText.SetActive(false);
        objectiveUpdatedDesc.gameObject.SetActive(false);
        objectiveFailedDesc.gameObject.SetActive(false);
        objectiveCompletedDesc.gameObject.SetActive(false);

        GenerateObjectiveZero();
        GenerateObjectiveOne();
        GenerateObjectiveTwo();
        GenerateObjectiveTutorial();

        disable1 = false;
        disable2 = false;
        disable3 = false;

    }

    

    private void Start()
    {
        // Add the initial objective for the current day and show notification
        if (ContinuousData.instance.CDdayIndex == 0)
        {
            activeObjectives.Add(objective_3);
            AssignDisplayVariables(objective_3);
            OutputNotification(objective_3);
        }
        else
        {
            activeObjectives.Add(objective_2);
            AssignDisplayVariables(objective_2);
            OutputNotification(objective_2);
        }

        objectiveUpdatedText.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PushUpdateNotification(objective_0);
        }

        /*if (disable1!)
        {
            if (objective_0.completed == true)
            {
                PushCompletedNotification(objective_1);
            }
        }
        if (disable2!)
        {
            if (objective_1.completed)
            {
                PushCompletedNotification(objective_2);
            }
        }
        if (disable3!)
        {
            if (objective_2.completed)
            {
                PushCompletedNotification(objective_0);
            }
        }*/



    }

    public void PushUpdateNotification(Objective objective)
    {
        // Show an update notification for the given objective
        objectiveUpdatedText.SetActive(true);
        SetNotificationType(objective, new string("Update"));
        SetObjectiveActive(objective);
        OutputNotification(objective);
    }
    public void PushCompletedNotification(Objective objective)
    {
        // Show a completed notification for the given objective
        objectiveCompletedText.SetActive(true);
        SetNotificationType(objective, new string("Completed"));
        SetObjectiveActive(objective);
        OutputNotification(objective);
    }

    public void OutputNotification(Objective objective)
    {
        // Play the notification animation and start its lifecycle coroutine
        notifImage.SetActive(true);
        animator = notifImage.GetComponent<Animator>();
        animator.SetTrigger("Form");
        StartCoroutine(NotificationHold(objective));
    }

    public void SetObjectiveActive(Objective objective)
    {
        // Add objective to active list and prepare display variables
        activeObjectives.Add(objective);
        AssignDisplayVariables(objective);
    }

    public void DisplayActiveObjectives()
    {
        if (ObjScrActive)
        {
            ObjScrActive = false;
            ObjectivesScreen.GetComponent<Animator>().SetTrigger("dissolve");
            StartCoroutine(HeaderHold(false));
            //ObjectivesScreen.SetActive(false);

        }
        else
        {
            ObjScrActive = true;
            ObjectivesScreen.SetActive(true);
            ObjectivesScreen.GetComponent<Animator>().SetTrigger("form");
            StartCoroutine(HeaderHold(true));
        }




    }

    public void PrintToScreen()
    {
        for (int i = 0; i < activeObjectives.Count; i++)
        {
            //
        }
    }


    public void AssignDisplayVariables(Objective fetchedObjective)
    {
        // Configure UI elements based on the objective's notification type
        notificationType = fetchedObjective.notificationType;
        //Decide which notification appearance type is used to show the objective with on screen [Update,Completed,Failed]
        if (notificationType == "Update")
        {
            objectiveUpdatedDesc.gameObject.SetActive(true);
            notifImage = objectiveUpdatedNotif; //reads from objective's notification type and assigns the correct image to the notification image var
            displayedDescription = objectiveUpdatedDesc;
            objectiveTextControlled = objectiveUpdatedText;
        }
        else
        {
            if (notificationType == "Completed")
            {
                objectiveCompletedDesc.gameObject.SetActive(true);
                notifImage = objectiveCompletedNotif;
                displayedDescription = objectiveCompletedDesc;
                objectiveTextControlled = objectiveCompletedText;
            }
            else
            {
                if (notificationType == "Failed")
                {
                    objectiveFailedDesc.gameObject.SetActive(true);
                    notifImage = objectiveFailedNotif;
                    displayedDescription = objectiveFailedDesc;
                    objectiveTextControlled = objectiveFailedText;
                }
                else
                {
                    Debug.Log("ERROR: Objective Notification Type Undetermined - cs.Objective void FetchNotificationType");
                }
            }
        }
    }

    private void SetNotificationType(Objective objective, string UpdateCompletedFailed)
    {
        objective.notificationType = UpdateCompletedFailed;
    }

    private IEnumerator NotificationHold(Objective objective)
    {
        // Show notification text and hide it after a delay
        yield return new WaitForSeconds(0.25f);
        notifTitle.GetComponent<TextMeshProUGUI>().text = objective.objectiveTitle;
        displayedDescription.text = objective.currentDescription;
        notifTitle.SetActive(true);
        objectiveTextControlled.SetActive(true);
        yield return new WaitForSeconds(6f);
        CloseNotification(objective);
        yield return new WaitForSeconds(0.3f);
        objectiveUpdatedText.SetActive(false);
        objectiveFailedText.SetActive(false);
        objectiveCompletedText.SetActive(false);
        displayedDescription.text = "";
    }
    private IEnumerator HeaderHold(bool state)
    {
        // Hold the objectives header visible briefly when opening/closing
        if (state == true)
        {
            yield return new WaitForSeconds(0.25f);
            ObjectivesScreenTitles.SetActive(true);
        }
        else
        {
            ObjectivesScreenTitles.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            ObjectivesScreen.SetActive(false);


        }

    }

    public void AssignObjectiveByTitle(string objectiveTitle, Objective objRef)
    {
        foreach (Objective obj in activeObjectives)
        {
            if (obj.objectiveTitle == objectiveTitle)
            {
                objRef = obj;
                break;
            }
        }
        if (objRef == null)
        {
            Debug.Log("ERROR: No Matching Objective Found");
        }

    }
    public void IncObjectiveIndex(Objective objective)
    {
        objective.UpdateCurrentIndex();

    }



    public void CloseNotification(Objective objective)
    {
        animator = notifImage.GetComponent<Animator>();
        animator.SetTrigger("Dissolve");
        objectiveTextControlled.SetActive(false);
        notifTitle.SetActive(false);
    }

    //Objective Generation
    public void GenerateObjectiveZero()
    {

        //Objective 0: Pull Up Your Boötes
        objective_0 = new Objective(new string("Pull Up Your Boötes"), new string("Start your assignment and complete your day."));
        objective_0.descriptions = new string[6];
        objective_0.descriptions[0] = "Find Candidates for your Essay 0/3";
        objective_0.descriptions[1] = "Find Candidates for your Essay 1/3";
        objective_0.descriptions[2] = "Find Candidates for your Essay 2/3";
        objective_0.descriptions[3] = "Cross the River to Return Home";
        objective_0.descriptions[4] = "Work on your Essay";
        objective_0.descriptions[5] = "Go to Sleep";
        objective_0.segmentCount = 6;
        objective_0.currentDescription = objective_0.descriptions[objective_0.currentIndex];

    }

    public void GenerateObjectiveOne()
    {
        //Objective 1: Come and Have a Go
        objective_1 = new Objective(new string("Come and Have a Go"), new string("Improve an Attribute through a University Club"));
        objective_1.descriptions = new string[3];
        objective_1.descriptions[0] = "Join a Club (Theatre, Gym, Debate)";
        objective_1.descriptions[1] = "Attend Club Practice";
        objective_1.segmentCount = 2;
        objective_1.currentDescription = objective_1.descriptions[objective_1.currentIndex];

    }

    public void GenerateObjectiveTwo()
    {

        objective_2 = new Objective(new string("Doing the Rounds"), new string(""));
        objective_2.descriptions = new string[3];
        objective_2.descriptions[0] = "Collect Question Answers from Candidates (0/3)";
        objective_2.descriptions[1] = "Collect Question Answers from Candidates (1/3)";
        objective_2.descriptions[2] = "Collect Question Answers from Candidates (2/3)";
        objective_2.segmentCount = 3;
        objective_2.currentDescription = objective_2.descriptions[objective_2.currentIndex];
    }

    public void GenerateObjectiveTutorial()
    {

        objective_3 = new Objective(new string("Tutorial"), new string("Complete your nightly routine!"));
        objective_3.descriptions = new string[3];
        objective_3.descriptions[0] = "Use WASD to walk around";
        objective_3.descriptions[1] = "Press E to interact (kitchen)";
        objective_3.descriptions[2] = "Note: Use your mouse to navigate any dialogue!";
        objective_3.segmentCount = 3;
        objective_3.currentDescription = objective_3.descriptions[objective_3.currentIndex];


    }



}
