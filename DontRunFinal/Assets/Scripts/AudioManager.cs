using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("-------------------Audio Source-------------------")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource sfxSource;

    [Header("-------------------Audio Clips-------------------")]
    public AudioClip musicClips;
    public AudioClip sfxClips;

    [Header("-------------------Canvas Panel------------------")]
    public GameObject LoginPanel;
    public GameObject LobbyPanel;
    public void Update()
    {
        if (LoginPanel.activeSelf || LobbyPanel.activeSelf)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.clip = musicClips;
                musicSource.loop = true;
                musicSource.Play();
            } 
        } else
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }
    }

    public void PlaySFX(AudioClip audioClip)
    {
        sfxSource.PlayOneShot(audioClip);
    }
}
