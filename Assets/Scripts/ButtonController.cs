using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    AudioManager audioManager;
    private SpriteRenderer SR;
    public Sprite normalImage;
    public Sprite pressedImage;
    public bool muteClap;
    public KeyCode keyToPressA;
    public KeyCode keyToPressB;
    public float offset;
    private float samplesPerBeat;

    void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        samplesPerBeat = audioManager.FindSamplesPerBeat(audioManager.musicSource);
    }

    void Update()
    {
        if (Input.GetKeyDown(keyToPressA) || Input.GetKeyDown(keyToPressB)) {  
            if (!muteClap) { audioManager.playFX(audioManager.clap); }
            SR.sprite = pressedImage;
        }

        if (Input.GetKeyUp(keyToPressA) || Input.GetKeyUp(keyToPressB)) {
            SR.sprite = normalImage;
        }
    }

    public float findClapTiming(AudioSource source) {
        float beatInSong = source.timeSamples / samplesPerBeat;

        int barNumber = Mathf.FloorToInt(beatInSong / 4f) + 1; // Bar index (1-based)
        float beatInBar = (beatInSong % 4f) + 1f; // Beat within bar (1.0 to 4.999...)

        Debug.Log($"Clap at Bar {barNumber}, Timing in Bar: {beatInBar}");
        return beatInBar;
    }
}
