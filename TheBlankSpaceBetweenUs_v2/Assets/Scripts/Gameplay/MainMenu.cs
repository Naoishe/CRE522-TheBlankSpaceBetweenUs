using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Image screen1;
    public Image screen2;
    public GameObject newNameObject;
    public GameObject confirmButtons;
    public GameObject submitButton;
    public GameObject nameRequest;
    public TextMeshProUGUI askingText;
    public string submittedName;
    public GameObject inputFieldObject;
    public InputField inputField;
    public float minOpacity = 0f;
    public float maxOpacity = 255f;

    static float lerpT = 0.0f;

    public static Action startButtonPressed;
    public static Action newGameTriggered;

    private bool buttonDisable;



    /// <summary>
    /// /DEBUG Variables
    /// </summary>

    private bool DEBUGVAR_block;

    void Start()
    {
        // Initialize menu UI state
        newNameObject.SetActive(false);
        confirmButtons.SetActive(false);
        submitButton.SetActive(false);
        buttonDisable = false;
    }
    

    public void StartNewGame()
    {
        // Trigger start of a new game and show the name request UI
        if (!buttonDisable)
        {
            startButtonPressed?.Invoke();
            LerpScreen();
            RequestName();
        }
    }

    public void LerpScreen()
    {
        screen1.color = new Color(Mathf.Lerp(minOpacity, maxOpacity, lerpT), Mathf.Lerp(minOpacity, maxOpacity, lerpT), Mathf.Lerp(minOpacity, maxOpacity, lerpT), 1);
        screen2.color = new Color(Mathf.Lerp(minOpacity, maxOpacity, lerpT), Mathf.Lerp(minOpacity, maxOpacity, lerpT), Mathf.Lerp(minOpacity, maxOpacity, lerpT), 1);
        lerpT -= 0.5f * Time.deltaTime;



    }

    public void RequestName()
    {
        // Show UI elements to request a player name
        nameRequest.SetActive(true);
        buttonDisable = true;
        inputFieldObject.SetActive(true);
        submitButton.SetActive(false);
        newNameObject.SetActive(true);
        confirmButtons.SetActive(true);
        askingText.text = "What is your name?";
    }

    public void SubmitName()
    {
        // Store submitted name and update UI for confirmation
        submittedName = inputField.text;
        inputFieldObject.SetActive(false);
        newNameObject.SetActive(false);
        confirmButtons.SetActive(false);
        askingText.text = submittedName + "...Is that right?...";
        submitButton.SetActive(true);



    }

    public void ConfirmName()
    {
        // Confirm name and notify ContinuousData, then proceed
        submitButton.SetActive(false);
        askingText.text = "Alright...I'll remember that.";
        ContinuousData.instance.UpdatePlayerName(submittedName);
        StartCoroutine(TimeForWords());


    }

    IEnumerator TimeForWords()
    {
        yield return new WaitForSeconds(5);
        newGameTriggered?.Invoke();
        nameRequest.SetActive(true);

    }


}
