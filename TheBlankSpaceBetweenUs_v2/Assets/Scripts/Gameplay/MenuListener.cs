using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuListener : MonoBehaviour
{
    public AudioSource MainMusic;
    public AudioSource SoundEffect;
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
    }
    void Update()
    {
        
    }

    public void PlaySoundEffect()
    {
        SoundEffect.Play();
    }
}
