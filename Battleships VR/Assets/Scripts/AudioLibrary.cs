using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLibrary : MonoBehaviour
{
    [System.Serializable]
    public class AudioStats
    {
        public string name;
        public AudioClip clip;
        [Range(0, 1)]
        public float volume;
        [Range(0.1f, 3f)]
        public float pitch;
        [HideInInspector] public AudioSource source;
    }
    public static AudioLibrary instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public AudioStats[] sounds;


    public IEnumerator GenerateSound(string name, GameObject obj, bool is3D, float delay = 0f, float minDistance = 100f, bool manualStop = false)
    {
        if (obj.GetComponent<AudioSource>() == null)
            obj.AddComponent<AudioSource>();


        AudioStats stats = Array.Find(sounds, sound => sound.name == name);
        if(stats == null)
        {
            Debug.Log("No suitable sound found");
            yield return null;
        }
        AudioSource source = obj.GetComponent<AudioSource>();
        source.playOnAwake = false;

        //Set the sound to 3D
        if (is3D)
        {
            source.spatialBlend = 1f;
            source.minDistance = minDistance;
            source.maxDistance = 1000f;
        }
        source.clip = stats.clip;
        source.volume = stats.volume;
        Debug.Log("Sound called");
        //Delay is an optional parameter so other classes can use this method without the delay
        yield return new WaitForSeconds(delay);
        source.Play();
        if(manualStop == false)
            StartCoroutine(StopPlayingSound(source, source.clip.length));
    }

    private IEnumerator StopPlayingSound(AudioSource source, float timeToStop)
    {
        yield return new WaitForSeconds(timeToStop);
        Debug.Log("Stopping source");
        source.Stop();
    }
}
