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

    public GameObject objectiveUpdatedTitle;
    public GameObject objectiveFailedTitle;
    public GameObject objectiveCompletedTitle;

    public TextMeshProUGUI objectiveUpdatedDesc;
    public TextMeshProUGUI objectiveFailedDesc;
    public TextMeshProUGUI objectiveCompletedDesc;

    //hierarchy vars for active gameplay methods

    public GameObject notifImage;
    public GameObject notifTitle;
    public TextMeshProUGUI displayedDescription;

    //Animator Vars
    public Animator animator;

    //Other Vars
    private string notificationType;

    //Objective Objects
    public List<Objective> activeObjectives = new List<Objective>();
    Objective objective_0;
    Objective objective_1;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        GenerateObjectives();
    }

    private void OnEnable()
    { 

    }

    private void OnDisable()
    {
        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OutputNotification(objective_0);
        }
    }

    public void OutputNotification(Objective objective)
    {
        FetchNotificationType(objective);
        notifImage.SetActive(true);
        animator=notifImage.GetComponent<Animator>();
        animator.SetTrigger("Form");
        StartCoroutine(NotificationHold(objective));
    }

    public void FetchNotificationType(Objective fetchedObjective)
    {
        notificationType = fetchedObjective.notificationType;
        //Decide which notification appearance type is used to show the objective with on screen [Update,Completed,Failed]
        if (notificationType == "Update")
        {
            notifImage = objectiveUpdatedNotif; //reads from objective's notification type and assigns the correct image to the notification image var
            notifTitle = objectiveUpdatedTitle;
            displayedDescription = objectiveUpdatedDesc;
        }
        else
        {
            if (notificationType == "Completed")
            {
                notifImage = objectiveCompletedNotif;
                notifTitle = objectiveCompletedTitle;
                displayedDescription = objectiveCompletedDesc;
            }
            else
            {
                if (notificationType == "Failed")
                {
                    notifImage = objectiveFailedNotif;
                    notifTitle = objectiveFailedTitle;
                    displayedDescription = objectiveFailedDesc;
                }
                else
                {
                    Debug.Log("ERROR: Objective Notification Type Undetermined - cs.Objective void FetchNotificationType");
                }
            }
        }
    }

    private IEnumerator NotificationHold(Objective objective)
    {
        yield return new WaitForSeconds(0.2f);
        notifTitle.SetActive(true);
        displayedDescription.text = objective.currentDescription;
        yield return new WaitForSeconds(5f);
        CloseNotification(objective);
        yield return new WaitForSeconds(0.2f);
        notifTitle.SetActive(false);

    }

    public void CloseNotification(Objective objective)
    {
        animator = notifImage.GetComponent<Animator>();
        animator.SetTrigger("Dissolve");
    }

    //Objective Generation
    public void GenerateObjectives()
    {
        /*
        objective_ = new Objective(new string(""), new string(""));
        objective_.descriptions = new string[7];
        objective_.descriptions[0] = "";
        objective_.descriptions[1] = "";
        objective_.descriptions[2] = "";
        objective_.segmentCount = ;
        */
        
        //Objective 0: Pull Up Your Boötes
        objective_0 = new Objective(new string("Pull Up Your Boötes"), new string("Start your assignment and complete your day."));
        objective_0.descriptions = new string[7];
        objective_0.descriptions[0] = "Find Candidates for your Essay 0/3";
        objective_0.descriptions[1] = "Find Candidates for your Essay 1/3";
        objective_0.descriptions[2] = "Find Candidates for your Essay 2/3";
        objective_0.descriptions[5] = "Cross the River to Return Home";
        objective_0.descriptions[6] = "Work on your Essay";
        objective_0.descriptions[7] = "Go to Sleep";
        objective_0.segmentCount = 8;
        objective_0.currentDescription = objective_0.descriptions[objective_0.currentIndex];

        //Objective 1: Come and Have a Go
        objective_1 = new Objective(new string("Come and Have a Go"), new string("Improve an Attribute through a University Club"));
        objective_1.descriptions = new string[2];
        objective_1.descriptions[0] = "Join a Club (Theatre, Gym, Debate)";
        objective_1.descriptions[1] = "Attend Club Practice";
        objective_1.descriptions[2] = "View Attributes in 'Profile'";
        objective_1.segmentCount = 3;
        objective_1.currentDescription = objective_1.descriptions[objective_1.currentIndex];

    }



}
