using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISoundService
{

    void PlaySound(AudioClip clip, PlaybackOptions options = null);
    void ChangeMusic(AudioClip clip, PlaybackOptions options = null);
    void FadeOutMusic(float fadeTime);
    void FadeInMusic(float fadeTime);

}

public class PlaybackOptions
{
    public float volume = -1;
    public float pitch = -1;
}