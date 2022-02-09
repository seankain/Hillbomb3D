using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGruntEmitter : MonoBehaviour
{

    public AudioSource GruntAudioSource;
    public List<AudioClipLocation> clipLocations;
    
    public void PlayRandom()
    {
        var index = UnityEngine.Random.Range(0, clipLocations.Count-1);
        var clipLocation = clipLocations[index];
        //GruntAudioSource.time = clipLocation.Start;
        //GruntAudioSource.Play();
        StartCoroutine(PlaySubclip(clipLocation));
    }

    private IEnumerator PlaySubclip(AudioClipLocation location)
    {
        GruntAudioSource.time = location.Start;
        GruntAudioSource.Play();
        var elapsed = 0f;
        yield return new WaitForSeconds(location.Duration);
        //while(elapsed < location.Duration)
        //{
        //    yield return null;
        //}
        GruntAudioSource.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class AudioClipLocation
{
    public float Start = 0;
    public float Duration = 0;
}
