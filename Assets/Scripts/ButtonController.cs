using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer SR;
    public Sprite normalImage;
    public Sprite pressedImage;
    public AudioSource _clap;
    public AudioSource musicAudioSource;
    public bool muteClap;
    public KeyCode keyToPressA;
    public KeyCode keyToPressB;
    public float offset;
    private float samplesPerBeat;
    public BeatManager beatManager;
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        if (muteClap) {
            _clap.mute = true;
        }
        float sampleRate = musicAudioSource.clip.frequency;
        samplesPerBeat = sampleRate * (60f / beatManager.bpm); 

    }

    void Update()
    {
        if (Input.GetKeyDown(keyToPressA) || Input.GetKeyDown(keyToPressB)) {
            _clap.Play();
            SR.sprite = pressedImage;
            // float timeInSeconds = musicAudioSource.timeSamples / (float)musicAudioSource.clip.frequency;
            // Debug.Log("Clap at song time sample: " + (timeInSeconds + offset));
            float beatInSong = musicAudioSource.timeSamples / samplesPerBeat;
            Debug.Log("Clap at song's beat number: " + beatInSong);
            // function to check timing i guess
        }

        if (Input.GetKeyUp(keyToPressA) || Input.GetKeyUp(keyToPressB)) {
            SR.sprite = normalImage;
        }
    }
}
