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
    public TextMeshProUGUI askingText;
    public string submittedName;
    public InputField inputField;
    public float minOpacity=-1.0f;
    public float maxOpacity=1.0f;

    static float lerpT = 0.0f;

    public static Action startButtonPressed;
    public static Action newGameTriggered;



    /// <summary>
    /// /DEBUG Variables
    /// </summary>

    private bool DEBUGVAR_block;

    void Start()
    {
     
    }
    void Update()
    {
        

    }

    public void StartNewGame()
    {
        startButtonPressed?.Invoke();
        LerpScreen();
    }

    public void LerpScreen()
    {
        screen.color = new Color(screen.color.r, screen.color.g, screen.color.b, Mathf.Lerp(minOpacity, maxOpacity, lerpT));
        lerpT += 0.5f * Time.deltaTime;
        Debug.Log("Activated");

        if (lerpT >= 1.0f)
        {
            RequestName();
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
        StartCoroutine(TimeForWords());


    }

    IEnumerator TimeForWords()
    {
        yield return new WaitForSeconds(5);
        newGameTriggered?.Invoke();

    }


}
