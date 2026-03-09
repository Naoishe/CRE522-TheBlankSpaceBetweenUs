using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Image screen;
    public GameObject newNameObject;
    public GameObject confirmButtons;
    public GameObject scoredOut;
    public TextMeshProUGUI askingText;
    public string submittedName;
    public InputField inputField;
    public float minOpacity=0f;
    public float maxOpacity=255f;

    static float lerpT = 0.0f;

    public static Action startButtonPressed;
    public static Action newGameTriggered;



    /// <summary>
    /// /DEBUG Variables
    /// </summary>

    private bool DEBUGVAR_block;

    void Start()
    {
        scoredOut.SetActive(true);
        newNameObject.SetActive(false);
        confirmButtons.SetActive(false);
    }
    void Update()
    {
        

    }

    public void StartNewGame()
    {
        startButtonPressed?.Invoke();
        scoredOut.SetActive(false);
        LerpScreen();
    }

    public void LerpScreen()
    {
        screen.color = new Color(Mathf.Lerp(minOpacity, maxOpacity, lerpT), Mathf.Lerp(minOpacity, maxOpacity, lerpT), Mathf.Lerp(minOpacity, maxOpacity, lerpT),1);
        lerpT -= 0.5f * Time.deltaTime;

        if (lerpT == 0f)
        {
            //RequestName();
        }

    }

    public void RequestName()
    {
        newNameObject.SetActive(true);
        confirmButtons.SetActive(false);
        askingText.text = "What is your name?";
    }

    public void SubmitName()
    {
        submittedName = inputField.text;
        newNameObject.SetActive(false);
        askingText.text = submittedName + "...Is that right?...";
        confirmButtons.SetActive(true);


    }

    public void ConfirmName()
    {
        confirmButtons.SetActive(false);
        askingText.text = "Alright...I'll remember that.";
        ContinuousData.instance.UpdatePlayerName(submittedName);
        StartCoroutine(TimeForWords());


    }

    IEnumerator TimeForWords()
    {
        yield return new WaitForSeconds(5);
        newGameTriggered?.Invoke();

    }


}
