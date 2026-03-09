using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;
using static Unity.Collections.AllocatorManager;

public class EndedClass : MonoBehaviour
{
    //public Image blackLerp;
    static float blackT = 0.0f;
    private float minOpacity = -1.0f;
    private float maxOpacity = 1.0f;
    private InMemoryVariableStorage variableStorage;
    private bool endingclass;

    private void Start()
    {
        variableStorage = FindObjectOfType<InMemoryVariableStorage>();
        endingclass = false;
    }

    private void Update()
    {

        //ContinuousData.instance.variableStorage.TryGetValue("$EndClass", out endingclass);

        if (endingclass)
        {
            SceneManager.LoadScene("CampusGrounds");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            endingclass = true;
        }
    }
    //public void BlackLerpScreen()
    //{
    //    blackLerp.color = new Color(blackLerp.color.r, blackLerp.color.g, blackLerp.color.b, Mathf.Lerp(minOpacity, maxOpacity, blackT));
    //    blackT += 0.5f * Time.deltaTime;

    //    if (blackT >= 1.0f)
    //    {
    //        SceneManager.LoadScene("CampusGrounds");
    //    }
    //}
}
