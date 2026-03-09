using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Image whiteLerp;
    public Image blackLerp;
    public GameObject newNameObject;
    public GameObject confirmButtons;
    public TextMeshProUGUI askingText;
    public string submittedName;
    public InputField inputField;
    public float minOpacity=-1.0f;
    public float maxOpacity=1.0f;

    static float whiteT = 0.0f;
    static float blackT = 0.0f;

    public static Action startButtonPressed;



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
        WhiteLerpScreen();
    }

    public void WhiteLerpScreen()
    {
        whiteLerp.color = new Color(whiteLerp.color.r, whiteLerp.color.g, whiteLerp.color.b, Mathf.Lerp(minOpacity, maxOpacity, whiteT));
        whiteT += 0.5f * Time.deltaTime;

        if (whiteT >= 1.0f)
        {
            RequestName();
        }
    }

    public void BlackLerpScreen()
    {
        blackLerp.color = new Color(blackLerp.color.r, blackLerp.color.g, blackLerp.color.b, Mathf.Lerp(minOpacity, maxOpacity, blackT));
        blackT += 0.5f * Time.deltaTime;

        if (blackT >= 1.0f)
        {
            SceneManager.LoadScene("PlayerHouse");
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
        BlackLerpScreen();

    }


}
