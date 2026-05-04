using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.AllocatorManager;
using TMPro;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;

public class MenuListener : MonoBehaviour
{
    public AudioSource MainMusic;
    public AudioSource SoundEffect;
    //public Image screen;
    public float minOpacity = -1.0f;
    public float maxOpacity = 1.0f;
    public GameObject MenuScript;

    void Start()
    {
        MainMusic.Play();
    }
    private void OnEnable()
    {
        MainMenu.startButtonPressed += PlaySoundEffect;
        MainMenu.newGameTriggered += BeginGame;
    }

    private void OnDisable()
    {
        MainMenu.startButtonPressed -= PlaySoundEffect;
    }
    void Update()
    {
        
    }
    public void BeginGame()
    {
        StartCoroutine(LoadGame());
    }

    public void PlaySoundEffect()
    {
        SoundEffect.Play();
    }

    

    IEnumerator LoadGame()
    {

        yield return new WaitForSeconds(2);
        ContinuousData.instance.SceneChangeDetected("PlayerHouse", ContinuousData.instance.playerHouse_MorningSpawn);
    }
    

}
