using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using TMPro.EditorUtilities;

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
    Objective objective_4;
    Objective objective_5;
    Objective objective_6;
    Objective objective_7;
    Objective objective_8;
    Objective objective_9;
    Objective objective_10;
    Objective objective_11;

    private int i = 0;


    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        GenerateObjectives();
        ObjScrActive = false;
    }

    private void OnEnable()
    { 

    }

    private void OnDisable()
    {
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
                PushUpdateNotification(objective_0);
        }
        
    }

    public void PushUpdateNotification(Objective objective)
    {
        SetNotificationType(objective, new string("Update"));
        SetObjectiveActive(objective);
        OutputNotification(objective);
    }

    public void OutputNotification(Objective objective)
    {
        notifImage.SetActive(true);
        animator=notifImage.GetComponent<Animator>();
        animator.SetTrigger("Form");
        StartCoroutine(NotificationHold(objective));
    }

    public void SetObjectiveActive(Objective objective)
    {
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
        for (int i = 0;  i < activeObjectives.Count; i++)
        {
            //
        }
    }


    public void AssignDisplayVariables(Objective fetchedObjective)
    {
        notificationType = fetchedObjective.notificationType;
        //Decide which notification appearance type is used to show the objective with on screen [Update,Completed,Failed]
        if (notificationType == "Update")
        {
            notifImage = objectiveUpdatedNotif; //reads from objective's notification type and assigns the correct image to the notification image var
            displayedDescription = objectiveUpdatedDesc;
            objectiveTextControlled = objectiveUpdatedText;
        }
        else
        {
            if (notificationType == "Completed")
            {
                notifImage = objectiveCompletedNotif;
                displayedDescription = objectiveCompletedDesc;
                objectiveTextControlled = objectiveCompletedText;
            }
            else
            {
                if (notificationType == "Failed")
                {
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
        yield return new WaitForSeconds(0.25f);
        notifTitle.GetComponent<TextMeshProUGUI>().text = objective.objectiveTitle;
        displayedDescription.text = objective.currentDescription;
        notifTitle.SetActive(true);
        objectiveTextControlled.SetActive(true);
        yield return new WaitForSeconds(6f);
        CloseNotification(objective);
        

    }
    private IEnumerator HeaderHold(bool state)
    {
        if (state==true)
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



    public void CloseNotification(Objective objective)
    {
        animator = notifImage.GetComponent<Animator>();
        animator.SetTrigger("Dissolve");
        objectiveTextControlled.SetActive(false);
        notifTitle.SetActive(false);
    }

    //Objective Generation
    public void GenerateObjectives()
    {
       
        /*
        objective_ = new Objective(new string(""), new string(""));
        objective_.descriptions = new string[];
        objective_.descriptions[0] = "";
        objective_.descriptions[1] = "";
        objective_.descriptions[2] = "";
        objective_.segmentCount = ;
        */
        
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

        //Objective 1: Come and Have a Go
        objective_1 = new Objective(new string("Come and Have a Go"), new string("Improve an Attribute through a University Club"));
        objective_1.descriptions = new string[3];
        objective_1.descriptions[0] = "Join a Club (Theatre, Gym, Debate)";
        objective_1.descriptions[1] = "Attend Club Practice";
        objective_1.descriptions[2] = "View Attributes in 'Profile'";
        objective_1.segmentCount = 3;
        objective_1.currentDescription = objective_1.descriptions[objective_1.currentIndex];

        objective_2 = new Objective(new string("Doing the Rounds"), new string(""));
        objective_2.descriptions = new string[3];
        objective_2.descriptions[0] = "Collect Question Answers from Candidates (0/3)";
        objective_2.descriptions[1] = "Collect Question Answers from Candidates (1/3)";
        objective_2.descriptions[2] = "Collect Question Answers from Candidates (2/3)";
        objective_2.segmentCount = 3;

        objective_3 = new Objective(new string("Sun Down"), new string("Complete your nightly routine!"));
        objective_3.descriptions = new string[4];
        objective_3.descriptions[0] = "Make Dinner";
        objective_3.descriptions[1] = "Eat Dinner";
        objective_3.descriptions[2] = "Work On Essay";
        objective_3.descriptions[3] = "Go To Sleep";
        objective_3.segmentCount = 4;

        objective_4 = new Objective(new string("Sun Down"), new string("Complete your nightly routine."));
        objective_4.descriptions = new string[5];
        objective_4.descriptions[0] = "Make Dinner";
        objective_4.descriptions[1] = "Eat Dinner";
        objective_4.descriptions[2] = "Work On Essay";
        objective_4.descriptions[3] = "Lock The Window";
        objective_4.descriptions[4] = "Go To Sleep";
        objective_4.segmentCount = 5;

        objective_5 = new Objective(new string("Among the Stars"), new string("Get involved with club tournaments!"));
        objective_5.descriptions = new string[6];
        objective_5.descriptions[0] = "Enter a club tournament";
        objective_5.descriptions[1] = "Win A Club Tournament";
        objective_5.segmentCount = 2;

        objective_6 = new Objective(new string("Have some Taste"), new string("Help Salem with the Cafe"));
        objective_6.descriptions = new string[3];
        objective_6.descriptions[0] = "Order Something from the Cafe";
        objective_6.descriptions[1] = "Put Rubbish in the Bin";
        objective_6.descriptions[2] = "Wipe Tables";
        objective_6.segmentCount = 3;

        objective_7 = new Objective(new string("Lightspeed"), new string(""));
        objective_7.descriptions = new string[3];
        objective_7.descriptions[0] = "Collect Question Answers from Candidates 0/3";
        objective_7.descriptions[1] = "Collect Question Answers from Candidates 1/3";
        objective_7.descriptions[2] = "Collect Question Answers from Candidates 2/3";
        objective_7.segmentCount = 3;

        objective_8 = new Objective(new string("Honey, I'm Home..."), new string(""));
        objective_8.descriptions = new string[6];
        objective_8.descriptions[0] = "What does the note say..?";
        objective_8.descriptions[1] = "Check on the strange Sound";
        objective_8.descriptions[2] = "Save Salem";
        objective_8.segmentCount = 3;

        objective_9 = new Objective(new string("Gone Girl"), new string(""));
        objective_9.descriptions = new string[3];
        objective_9.descriptions[0] = "Ask Around Camous about Salem";
        objective_9.descriptions[1] = "Check the Cafe";
        objective_9.descriptions[2] = "Locate Salem";
        objective_9.segmentCount = 3;

        objective_10 = new Objective(new string("Unturned"), new string(""));
        objective_10.descriptions = new string[3];
        objective_10.descriptions[0] = "Find a Way inside the lecture halls";
        objective_10.descriptions[1] = "Locate the dean's office";
        objective_10.descriptions[2] = "Find Information on Niko";
        objective_10.segmentCount = 3;

        objective_11 = new Objective(new string("Racing Hearts"), new string(""));
        objective_11.descriptions = new string[3];
        objective_11.descriptions[0] = "Meet Faust by the Fountain";
        objective_11.descriptions[1] = "Attend the date";
        objective_11.descriptions[2] = "Steal phone?";
        objective_11.segmentCount = 3;

    }



}
