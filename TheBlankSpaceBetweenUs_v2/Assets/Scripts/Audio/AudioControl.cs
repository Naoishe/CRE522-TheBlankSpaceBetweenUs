using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public AudioSource music;
    public static AudioControl instance;

    public void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        music.Play();
        music.volume = 0.2f;
    }

    
}
