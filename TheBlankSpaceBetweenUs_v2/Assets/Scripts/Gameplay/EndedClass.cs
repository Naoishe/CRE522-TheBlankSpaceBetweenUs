using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;
using static Unity.Collections.AllocatorManager;

public class EndedClass : MonoBehaviour
{
    private InMemoryVariableStorage variableStorage;
    private bool endingclass;

    private void Start()
    {
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        endingclass = false;
    }

    private void Update()
    {


        if (endingclass)
        {
            SceneManager.LoadScene("CampusGrounds");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            endingclass = true;
        }
    }
}
