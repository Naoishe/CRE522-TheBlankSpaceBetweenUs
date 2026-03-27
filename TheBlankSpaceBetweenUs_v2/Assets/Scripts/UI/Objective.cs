using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Objective 
{
    public string objectiveTitle;
    public string objectiveDescription;

    //Changable Vars

    public bool completed;
    public int segmentCount;
    public string notificationType;
    public string[] descriptions; //All segment descriptions of the Objective that can appear on an update are stored in here


    public Objective()
    {

    }
}

