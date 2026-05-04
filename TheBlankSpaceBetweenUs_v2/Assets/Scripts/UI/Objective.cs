using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Objective 
{
    //Base Class for Objective objects 

    //Set at Generation Vars
    public string objectiveTitle;
    public string objectiveSummary;
    public int segmentCount;
    public string[] descriptions;

    //Changable Vars
    public bool completed;
    public string notificationType;
    public int currentIndex;
    public string currentDescription;


    public Objective(string insertObjectiveTitle, string insertObjectiveSummary)
    {
        objectiveTitle = insertObjectiveTitle;
        objectiveSummary = insertObjectiveSummary;
        completed = false;
        notificationType = "Update";
        currentIndex = 0;
    }

    public void UpdateNotificationType( string typeUpdate)
    {
       notificationType = typeUpdate;
    }

    public void UpdateNotificationType( bool completedUpdate)
    {
        completed = completedUpdate;
    }

    public void UpdateCurrentIndex()
    {
        currentIndex++;
        currentDescription = descriptions[currentIndex];
    }

}

