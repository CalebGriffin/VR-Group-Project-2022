using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

//Code from Brackys tutorial
public class AudioManager : MonoBehaviour
{
    [System.Serializable] 
    public class Sound
    {
        //Variables for each sound
        public string name;
        public AudioClip clip;
        [Range(0, 1)]
        public float volume;
        [Range(0.1f, 3)]
        public float pitch;

        public bool loop;

        [HideInInspector] public AudioSource source;
    }
    public Sound[] sounds;
    public static AudioManager instance;
    

    private void Awake()
    {
        //Create a singleton
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        //DontDestroyOnLoad(gameObject);


        foreach (Sound s in sounds)
        {
            //Initialize the variables for each clip
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            //s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void PlaySounds(string soundName)
    {
        //Find the correct sound
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning($"Sound not found: {soundName}");
            return;
        }
        s.source.Play();
    }
}
