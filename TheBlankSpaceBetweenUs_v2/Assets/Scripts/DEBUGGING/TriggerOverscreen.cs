using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOverscreen : MonoBehaviour
{
    public GameObject gameObject;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            gameObject.SetActive(true);
        }
    }
}
