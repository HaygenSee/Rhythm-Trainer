using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    AudioManager audioManager;
    ChartReader chartReader;
    ButtonController buttonController;
    public bool _playingSong = false;
    private float countdownSamplesPerBeat;
    private float songSamplesPerBeat;
    private List<Bar> fullChart;
    private int currentBarIndex = 0;
    private int chartBarIndex = 0;
    private float timeSignature = 4f; // 4/4
    public Enemy enemyObject;
    public Text _readyText;

    void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        buttonController = GameObject.FindGameObjectWithTag("ButtonController").GetComponent<ButtonController>();
        chartReader = GameObject.FindGameObjectWithTag("ChartReader").GetComponent<ChartReader>();
    }

    void Start()
    {
        Invoke("startCountdown", 2.0f);
        Invoke("byeReady", 2.0f);

        countdownSamplesPerBeat = audioManager.FindSamplesPerBeat(audioManager.countdownSource);
        songSamplesPerBeat = audioManager.FindSamplesPerBeat(audioManager.musicSource);

        fullChart = chartReader.randomiseBarOrder(false);

        Debug.Log($"Bars in chart: {chartReader.barCount}");
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playingSong && audioManager.countdownFinished()) {
            _playingSong = true;
            audioManager.playSong();
        }

        if (_playingSong && Input.GetKeyDown(KeyCode.Escape)) { // instead of esc key lose condition
            _playingSong = false;
            audioManager.stopSong();
        }

        if (_playingSong) {
            float beatInLoop = audioManager.loopBeat();
            
            Debug.Log("Beating: " + beatInLoop);
            int newBarIndex = Mathf.FloorToInt(audioManager.currentBeatInSong / timeSignature);

            if (Input.GetKeyDown(buttonController.keyToPressA) || Input.GetKeyDown(buttonController.keyToPressB)) {  
                buttonController.findClapTiming(audioManager.musicSource);
            }

            if (beatInLoop >= 1f && beatInLoop <= 4.99f) {
                enemyObject.enemyTurn = true;
            } else {
                enemyObject.enemyTurn = false;
            }

            if (newBarIndex != currentBarIndex) {
                currentBarIndex = newBarIndex;
                if (enemyObject.enemyTurn == true) {
                    chartBarIndex += 1;
                }
            }

            if (enemyObject.enemyTurn && chartBarIndex <= chartReader.barCount - 1) {   
                enemyObject.clapToPattern(fullChart[chartBarIndex], beatInLoop);
                Debug.Log("get clapping teto");
            }
        }

    }

    void startCountdown() {
        audioManager.countdownSource.Play(); 
    }

    void byeReady() {
        _readyText.enabled = false;
    }

}
