using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Objective: MonoBehaviour 
{
    public string objectiveTitle;
    public string objectiveDescription;
    public int objectiveSegmentCount;
    public int segmentIndex;
    public bool completed;

    public string[] descriptions;

    //hierarchy variables

    public GameObject notifImage;
    public GameObject notifTitle;
    public TextMeshProUGUI description;

}
