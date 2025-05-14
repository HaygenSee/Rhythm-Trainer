using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    AudioManager audioManager;
    ChartReader chartReader;
    Player _Player;
    public bool _playingSong = false;
    private float songSamplesPerBeat;
    private List<Bar> fullChart;
    private int currentBarIndex = 0;
    private int chartBarIndex = 0;
    private float timeSignature = 4f; 
    private int totalNotes = 0;
    public Enemy enemyObject;
    public Text _readyText;

    void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        chartReader = GameObject.FindGameObjectWithTag("ChartReader").GetComponent<ChartReader>();
    }

    void Start()
    {
        fullChart = chartReader.randomiseBarOrder(false);
        audioManager.songBpm = chartReader.bpm;
        StartCoroutine(CountdownThenStartSong());
        Invoke("byeReady", 2.0f);

        audioManager.samplesPerBeat = audioManager.FindSamplesPerBeat(audioManager.musicSource);

        songSamplesPerBeat = audioManager.FindSamplesPerBeat(audioManager.musicSource);

        StartCoroutine(CountdownThenStartSong());

        Debug.Log($"Bars in chart: {chartReader.barCount}");
        
        foreach (Bar bar in fullChart) {
            totalNotes += bar.getNoteCount();
        }

        Debug.Log($"Total number of notes: {totalNotes}");
    }

    // Update is called once per frame
    void Update()
    {
        if (_playingSong && Input.GetKeyDown(KeyCode.Escape)) { // instead of esc key lose condition
            _playingSong = false;
            audioManager.stopSong();
        }

        if (_playingSong) {
            float beatInLoop = audioManager.loopBeat();
            
            int newBarIndex = Mathf.FloorToInt(audioManager.currentBeatInSong / timeSignature);

            if (beatInLoop >= 1f && beatInLoop <= 4.90f) {
                enemyObject.enemyTurn = true;
            } else {
                enemyObject.enemyTurn = false;
            }

            if (newBarIndex != currentBarIndex) {
                currentBarIndex = newBarIndex;
                if (enemyObject.enemyTurn == true) {
                    chartBarIndex += 1;
                    enemyObject.enemyNextBar();
                    _Player.playerNextBar();
                }
            }

            // within bars
            if (chartBarIndex <= chartReader.barCount - 1) {
                if (enemyObject.enemyTurn) { 
                    if (enemyObject.clapToPattern(fullChart[chartBarIndex], beatInLoop)) {
                        enemyObject.Clap();
                    }

                } else { 
                    _Player.noTapMissCheck(beatInLoop, fullChart[chartBarIndex]); 
                }

                if (Input.GetKeyDown(_Player.keyToPressA) || Input.GetKeyDown(_Player.keyToPressB)) {
                    if (enemyObject.enemyTurn) {
                        Debug.Log("Not your turn yet! - lose health" );
                    } else {
                        _Player.accuracyScoring(beatInLoop, fullChart[chartBarIndex]);
                    }
                }
            } else { Debug.Log("Game End"); }
        }

    }

    private IEnumerator CountdownThenStartSong() {
        yield return new WaitForSeconds(2f); // delay before countdown (optional)

        yield return StartCoroutine(audioManager.PlayDynamicCountdown()); // play 4-beat countdown

        _playingSong = true;
        audioManager.playSong();
    }      

    void byeReady() {
        _readyText.enabled = false;
    }

}
