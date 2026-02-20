using UnityEngine;
using UnityEngine.Audio;

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

    public void Update()
    {
        //if game started
        //music.volume = 0.2f;
    }
}
