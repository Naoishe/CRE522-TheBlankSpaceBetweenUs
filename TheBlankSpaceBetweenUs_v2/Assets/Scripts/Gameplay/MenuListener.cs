using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;
using TMPro;
using System;
using UnityEngine.UI;

public class MenuListener : MonoBehaviour
{
    public AudioSource MainMusic;
    public AudioSource SoundEffect;
    //public Image screen;
    public float minOpacity = -1.0f;
    public float maxOpacity = 1.0f;
    public GameObject MenuScript;

    static float lerpT = 0.0f;
    static float volumeLerp = 0.0f;
    void Start()
    {
        MainMusic.Play();
    }
    private void OnEnable()
    {
        MainMenu.startButtonPressed += PlaySoundEffect;
        MainMenu.startButtonPressed += LerpMusic;
    }

    private void OnDisable()
    {
        MainMenu.startButtonPressed -= PlaySoundEffect;
        MainMenu.startButtonPressed -= LerpMusic;
    }
    void Update()
    {
        
    }

    public void LerpMusic()
    {
        /*MainMusic.volume = Mathf.Lerp(1f, 0f, volumeLerp);
        volumeLerp += 0.5f * Time.deltaTime;
        if (MainMusic.volume == 0f)
        {
            LoadGame();
        }*/
        LoadGame();
    }

    public void PlaySoundEffect()
    {
        SoundEffect.Play();
    }

    public void LoadGame()
    {

        SceneManager.LoadScene("PlayerHouse");
    }
}
