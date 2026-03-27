using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using TMPro.EditorUtilities;

public class ObjectivesManager : Objective, IMonoBehaviour
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

    //Segment Vars
    public int segmentIndex;

    //hierarchy vars for active gameplay methods

    public GameObject notifImage;
    public GameObject notifTitle;
    public TextMeshProUGUI displayedDescription;

    //Animator Vars
    public Animator animator;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    { 

    }

    private void OnDisable()
    {
        
    }

    public void Update()
    {
       
    }

    public void OutputNotification(Objective objective)
    {
        FetchNotificationType();
        notifImage.SetActive(true);
        animator=notifImage.GetComponent<Animator>();
        animator.SetTrigger("Form");
        StartCoroutine(NotificationHold());
    }

    public void FetchNotificationType()
    {
        //Decide which notification appearance type is used to show the objective with on screen [Update,Completed,Failed]
        if (notificationType == "Update")
        {
            notifImage = objectiveUpdatedNotif;
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
                    Debug.Log("ERROR: Objective Notification Type Undetermined - cs.Objective line 48");
                }
            }
        }
    }

    private IEnumerator NotificationHold()
    {
        yield return new WaitForSeconds(0.5f);
        notifTitle.SetActive(true);
        //displayedDescription=

    }

    private void GenerateObjectives()
    {
        //Objective 0: Pull Up Your Boötes
        Objective objective_0 = new Objective();
    }



}
