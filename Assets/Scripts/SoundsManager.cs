using System.Collections;
using UnityEngine;

public enum Sounds
{
    BackgroundMusic,
    ButtonClick,
    WallMove
}

public class SoundsManager : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip wallMove;

    private AudioSource audioSource;

    private static SoundsManager instance;

    public static SoundsManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);

        audioSource.Play();
    }

    public void PlaySounds(Sounds sounds)
    {
        switch(sounds)
        {
            case Sounds.BackgroundMusic:
                audioSource.PlayOneShot(backgroundMusic);
                break;
            case Sounds.ButtonClick:
                audioSource.PlayOneShot(buttonClick);
                break;
            case Sounds.WallMove:
                audioSource.PlayOneShot(wallMove);
                break;

        }
    }
}
