using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioSource _countdown;
    public AudioSource _mainSongSource; 
    public bool _playingSong = false;
    public BeatManager beatManager;
    private float countdownSamplesPerBeat;

    public Text _readyText;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("startCountdown", 2.0f);
        Invoke("byeReady", 2.0f);
        float countdownSampleRate = _countdown.clip.frequency;
        countdownSamplesPerBeat = countdownSampleRate * (60f / beatManager.bpm); 

    }

    // Update is called once per frame
    void Update()
    {
        if (!_playingSong && countdownFinished()) {
            _playingSong = true;
            _mainSongSource.Play();
        }

        if (_playingSong && Input.GetKeyDown(KeyCode.Escape)) {
            _playingSong = false;
            _mainSongSource.Stop();
        }
    }

    void startCountdown() {
        _countdown.Play(); 
    }

    void byeReady() {
        _readyText.enabled = false;
    }

    bool countdownFinished() {
        int currentBeat = Mathf.FloorToInt(_countdown.timeSamples / countdownSamplesPerBeat);
        return currentBeat >= 4;
    }
}
