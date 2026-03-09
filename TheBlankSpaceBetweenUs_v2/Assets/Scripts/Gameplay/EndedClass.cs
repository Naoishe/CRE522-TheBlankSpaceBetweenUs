using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;
using UnityEngine.UI;

public class EndedClass : MonoBehaviour
{
    public Image blackLerp;
    static float blackT = 0.0f;
    private float minOpacity = -1.0f;
    private float maxOpacity = 1.0f;
    public void BlackLerpScreen()
    {
        blackLerp.color = new Color(blackLerp.color.r, blackLerp.color.g, blackLerp.color.b, Mathf.Lerp(minOpacity, maxOpacity, blackT));
        blackT += 0.5f * Time.deltaTime;

        if (blackT >= 1.0f)
        {
            SceneManager.LoadScene("CampusGrounds");
        }
    }
}
