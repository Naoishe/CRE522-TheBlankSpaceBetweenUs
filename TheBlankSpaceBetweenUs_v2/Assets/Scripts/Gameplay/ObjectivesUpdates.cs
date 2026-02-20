using UnityEngine;
using System.Collections;
public class ObjectivesUpdates : MonoBehaviour
{
    public static ObjectivesUpdates instance;
    public GameObject objectives1;
    public GameObject objectives2;

    private GameObject currentObjective;

    public void Start()
    {
        instance = this;
        objectives1.SetActive(false);
        objectives2.SetActive(false);
    }

    public void OnEnable()
    {
        Day1Control.ObjUpdate1 += PresentUpdate1;
        Day1Control.ObjUpdate2 += PresentUpdate2;
    }
    public void OnDisable()
    {
        Day1Control.ObjUpdate1 -= PresentUpdate1;
        Day1Control.ObjUpdate2 -= PresentUpdate2;
    }
    public void PresentUpdate1()
    {
        objectives1.SetActive(true);
        currentObjective = objectives1;
        StartCoroutine(RemoveNotification());
    }
    public void PresentUpdate2()
    {
        objectives2.SetActive(true);
        currentObjective = objectives2;
        StartCoroutine(RemoveNotification());
    }

    private IEnumerator RemoveNotification()
    {
        yield return new WaitForSeconds(8);
        currentObjective.SetActive(false);
    }

}
