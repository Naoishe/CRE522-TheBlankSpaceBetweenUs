using System.Collections;
using UnityEngine;

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
        // Start main menu music
        MainMusic.Play();
    }
    private void OnEnable()
    {
        // Subscribe to main menu events
        MainMenu.startButtonPressed += PlaySoundEffect;
        MainMenu.newGameTriggered += BeginGame;
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        MainMenu.startButtonPressed -= PlaySoundEffect;
    }
    
    public void BeginGame()
    {
        // Start asynchronous game loading transition
        StartCoroutine(LoadGame());
    }

    public void PlaySoundEffect()
    {
        // Play UI click sound effect
        SoundEffect.Play();
    }



    IEnumerator LoadGame()
    {

        // Delay then move to the player's house scene
        yield return new WaitForSeconds(2);
        ContinuousData.instance.SceneChangeDetected("PlayerHouse", ContinuousData.instance.playerHouse_MorningSpawn);
    }


}
