using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioClip music;

    public void PlayMusic(AudioClip clip) {
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip, float pitchVariance) {
        soundSource.pitch = 1 + (Random.value * 2 - 1) * pitchVariance;
        if (clip == null) return;
        soundSource.PlayOneShot(clip);
    }


    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            PlayMusic(music);
        }
    }
}
