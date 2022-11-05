using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundClip
{
    public AudioClip Clip;
    [Range(0, 1)]
    public float Volume;
    public bool Loop;

    public void Play(AudioSource audioSource)
    {
        if (!Clip)
            return;

        audioSource.Stop();
        audioSource.clip = Clip;
        audioSource.volume = Volume;
        audioSource.loop = Loop;
        audioSource.Play();
    }
}
