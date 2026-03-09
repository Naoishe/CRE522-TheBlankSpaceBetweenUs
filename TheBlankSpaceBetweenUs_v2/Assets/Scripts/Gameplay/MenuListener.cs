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
    void Start()
    {
        MainMusic.Play();
    }
    private void OnEnable()
    {
        MainMenu.startButtonPressed += PlaySoundEffect;
    }

    private void OnDisable()
    {
        MainMenu.startButtonPressed += PlaySoundEffect;
        MainMenu.startButtonPressed += LoadGame;
    }
    void Update()
    {
        
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
