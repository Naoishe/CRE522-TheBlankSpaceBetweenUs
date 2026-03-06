using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    private string receivedPlayerName;
    private Image whiteLerp;
    private Image blackLerp;
    public Text text;
    public float minOpacity=-1.0f;
    public float maxOpacity=1.0f;
    static float whiteT = 0.0f;
    static float blackT = 0.0f;



    /// <summary>
    /// /DEBUG Variables
    /// </summary>

    private bool DEBUGVAR_block;

    void Start()
    {
        DEBUGVAR_block = false;


    }
    void Update()
    {
        if (!DEBUGVAR_block) 
        {
            WhiteLerpScreen();
        }

    }

    public void StartNewGame()
    {
        WhiteLerpScreen();
        //SceneManager.LoadScene("PlayerHouse");
    }

    public void WhiteLerpScreen()
    {
        // value = new Vector3(Mathf.Lerp(minimum, maximum, t), 0, 0);
        //whiteLerp.GetComponent<Image>().color.a = 0f;
        whiteLerp.color = new Color(whiteLerp.color.r, whiteLerp.color.g, whiteLerp.color.b, Mathf.Lerp(minOpacity, maxOpacity, whiteT));
        whiteT += 0.5f * Time.deltaTime;

        if (whiteT >= 1.0f)
        {
            DEBUGVAR_block = true;
        }
    }

    private void RegisterPlayerName()
    {

    }
}
