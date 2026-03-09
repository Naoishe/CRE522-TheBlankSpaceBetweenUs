using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BetaTemps : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ContinuousData.instance = null;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
