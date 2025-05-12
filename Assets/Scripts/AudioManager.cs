using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXSource;
    [SerializeField] public AudioSource countdownSource;

    [Header("Audio Clips")]
    public AudioClip bgm;
    public float songBpm = 120f;
    public AudioClip clap;
    public AudioClip countdown;
    public AudioClip EnemyClap;

    [Header("Pulse Trigger")]
    [SerializeField] private UnityEvent _trigger;

    private float samplesPerBeat;
    private float countdownSamplesPerBeat;
    private int lastBeat = -1;
    public float currentBeatInSong;

    void Start() {
        samplesPerBeat = FindSamplesPerBeat(musicSource);
        countdownSamplesPerBeat = FindSamplesPerBeat(countdownSource);
    }
    void Update() {
        // pulsate
        trackBeat(musicSource, samplesPerBeat);
        currentBeatInSong = musicSource.timeSamples / samplesPerBeat;
    }

    public void playSong() {
        musicSource.Play();
    }

    public void stopSong() {
        musicSource.Stop();
    }

    public void playFX(AudioClip clip) {
        SFXSource.PlayOneShot(clip);
    }

    // return samplesPerBeat of audio anywhere
    public float FindSamplesPerBeat(AudioSource _audioSource) {
        float sampleRate = _audioSource.clip.frequency;
        return sampleRate * (60f / songBpm);
    }

    // track beat of audio for gameobjects to pulse
    private int trackBeat(AudioSource _audioSource, float _samplesPerBeat) {
        int currentBeat = Mathf.FloorToInt(_audioSource.timeSamples / _samplesPerBeat);
        // Debug.Log("Current Beat = " + ((_audioSource.timeSamples / _samplesPerBeat) + 1)); 
        if (currentBeat != lastBeat) {
            lastBeat = currentBeat;
            _trigger.Invoke();
        }
        return currentBeat;
    }

    public bool countdownFinished() {
        int currentBeat = trackBeat(countdownSource, countdownSamplesPerBeat);
        return currentBeat >= 4;
    }

    public float loopBeat() {
        float beatInLoop = (currentBeatInSong % 8f) + 1;
        return beatInLoop;
    }

}

