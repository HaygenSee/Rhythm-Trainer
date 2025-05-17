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
    public float songBpm;
    public AudioClip clap;
    public AudioClip CD_3to1;
    public AudioClip CD_Go;
    public AudioClip EnemyClap;

    [Header("Pulse Trigger")]
    [SerializeField] private UnityEvent _trigger;

    public float samplesPerBeat;
    private int lastBeat = -1;
    public float currentBeatInSong;
    public GameObject gameManagerObject;

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

    public void pauseSong()
    {
        musicSource.Pause();
    }

    public void ResumeSong()
    {
        musicSource.UnPause();
    }

    public void playFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    // return samplesPerBeat of audio anywhere
    public float FindSamplesPerBeat(AudioSource _audioSource) {
        float sampleRate = _audioSource.clip.frequency;
        return sampleRate * (60f / songBpm);
    }

    // track beat of audio for gameobjects to pulse
    private int trackBeat(AudioSource _audioSource, float _samplesPerBeat) {
        int currentBeat = Mathf.FloorToInt(_audioSource.timeSamples / _samplesPerBeat) + 1;
        if (currentBeat != lastBeat) {
            lastBeat = currentBeat;
            _trigger.Invoke();
        }
        return currentBeat;
    }

    public IEnumerator PlayDynamicCountdown(int beatCount = 4) {
        float interval = 60f / songBpm;
        for (int i = 0; i < beatCount; i++) {
            if (i != 3) { countdownSource.PlayOneShot(CD_3to1); }
            else { countdownSource.PlayOneShot(CD_Go); }
            yield return new WaitForSeconds(interval);
        }
    }

    public float loopBeat() {
        float beatInLoop = (currentBeatInSong % 8f) + 1;
        return beatInLoop;
    }

    public IEnumerator FadeOut(AudioSource audioSource, float fadeTime) {
        GameManager managerScript = gameManagerObject.GetComponent<GameManager>();

        float startVolume = audioSource.volume;

        while (startVolume > 0.001f) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.Stop();
        managerScript._playingSong = false;
        audioSource.volume = startVolume;
    }

}

