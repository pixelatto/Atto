using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BindService]
public class SimpleSoundProvider : ISoundService
{

    GameObject musicContainer;
    AudioSource musicAudioSource;

    public SimpleSoundProvider()
    {
        musicContainer = new GameObject("Music Container");
        musicAudioSource = musicContainer.AddComponent<AudioSource>();
    }

    public void ChangeMusic(AudioClip clip, PlaybackOptions options = null)
    {
        musicAudioSource.clip = clip;
        if (options != null)
        {
            musicAudioSource.volume = options.volume != -1 ? options.volume : 1f;
            musicAudioSource.pitch = options.pitch != -1 ? options.pitch : 1f;
        }
    }

    public void FadeInMusic(float fadeTime)
    {
        musicAudioSource.volume = 1;
    }

    public void FadeOutMusic(float fadeTime)
    {
        musicAudioSource.volume = 0;
    }

    public void PlaySound(AudioClip clip, PlaybackOptions options = null)
    {
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        var audioSourceObject = new GameObject("Sound Effect: " + clip.name);
        AudioSource audioSource = audioSourceObject.AddComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.volume = options.volume != -1 ? options.volume : 1f;
            audioSource.pitch = options.pitch != -1 ? options.pitch : 1f;
        }
        GameObject.Destroy(audioSourceObject, clip.length);
    }
}
