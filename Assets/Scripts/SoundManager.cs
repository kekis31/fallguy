using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;      // Instance of the Sound Manager

    void Awake()
    {
        // Set the singleton instance
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        ClearEmptySources();
    }

    /// <summary>
    /// Checks for all the audio sources in the GameObject and clears them.
    /// </summary>
    void ClearEmptySources()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Delete all sources that are not playing
        foreach (AudioSource s in sources)
        {
            if (s.time == s.clip.length)
            {
                Destroy(s);
            }
        }
    }

    /// <summary>
    /// Creates an audio source and plays the clip with the specified volume, pitch and loop
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    /// <param name="loop"></param>
    public void PlaySound(string clip, float volume, float pitch, bool loop, bool music)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();

        string path = "SFX/";
        if (music) { path = "Music/"; }

        AudioClip c = Resources.Load(path + clip) as AudioClip;
        source.clip = c;
        if (music)
        {
            source.volume = volume; // * PlayerPrefs.GetFloat("MusicVolume");
        }
        else
        {
            if (clip.Contains("dialogue"))
                source.volume = volume * PlayerPrefs.GetFloat("DialogueVolume");
            else
                source.volume = volume; //* PlayerPrefs.GetFloat("Volume");

        }
        source.pitch = pitch;
        source.loop = loop;
        source.Play();

        print("Playing sound (" + clip + ")");
    }

    public void PlaySound(string clip) => PlaySound(clip, 1, 1, false, false);

    /// <summary>
    /// Stops all sounds with the specified clip playing.
    /// </summary>
    /// <param name="clip"></param>
    public void StopSound(string clip)
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Delete all sources that are not playing
        foreach (AudioSource s in sources)
        {
            if (s.clip.name == clip)
            {
                Destroy(s);
            }
        }
    }

    public void StopSoundsContaining(string clip)
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Delete all sources that are not playing
        foreach (AudioSource s in sources)
        {
            if (s.clip.name.Contains(clip))
            {
                Destroy(s);
            }
        }
    }

    /// <summary>
    /// Stops every sound clip playing.
    /// </summary>
    public void StopAllSounds()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Delete all sources that are not playing
        foreach (AudioSource s in sources)
        {
            Destroy(s);
        }
    }

    /// <summary>
    /// Set the global volume of the sounds playing in percentage values (0 = 0%, 0.5 = 50%, ect).
    /// </summary>
    public void SetGlobalVolume(float volume)
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Set the volume of all sounds playing
        foreach (AudioSource s in sources)
        {
            s.volume *= volume;
        }
    }

    public void SetVolume(string clip, float volume)
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Set the volume for the clip
        foreach (AudioSource s in sources)
        {
            if (s.clip.name == clip)
            {
                s.volume = volume;
            }
        }
    }

    public AudioClip GetSound(string clip)
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return null;
        }

        // Return the first source with the same clip
        foreach (AudioSource s in sources)
        {
            if (s.clip.name == clip)
            {
                return s.clip;
            }
        }

        return null;
    }

    public void PauseAllSounds()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Pause all sources
        foreach (AudioSource s in sources)
        {
            s.Pause();
        }
    }

    public void UnpauseAllSounds()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        // Return if no audio sources
        if (sources.Length == 0)
        {
            return;
        }

        // Unpause all sources
        foreach (AudioSource s in sources)
        {
            s.UnPause();
        }
    }
}