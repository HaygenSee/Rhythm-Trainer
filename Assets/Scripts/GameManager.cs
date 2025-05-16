using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    AudioManager audioManager;
    ChartReader chartReader;
    SpriteManager spriteManager;
    Player _Player;
    public bool _playingSong = false;
    public bool countdownDone = false;
    private bool firstBarSpawned = false;
    private List<GameObject> notesSpawned;
    private float songSamplesPerBeat;
    private List<Bar> fullChart;
    private int currentBarIndex = 0; private int chartBarIndex = 0; private float timeSignature = 4f; 
    private int totalNotes = 0; private int maxScore = 0;
    public Enemy enemyObject;
    public TMP_Text _readyText;
    public GameObject resultsPage;
    [Header("Results Text")]
    public TMP_Text perfectsText; public TMP_Text greatsText; public TMP_Text mehsText; public TMP_Text missesText;
    public TMP_Text totalScoreText; public TMP_Text percentageText;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        chartReader = GameObject.FindGameObjectWithTag("ChartReader").GetComponent<ChartReader>();
        spriteManager = GameObject.FindGameObjectWithTag("SpriteManager").GetComponent<SpriteManager>();
        
    }

    void Start()
    {
        fullChart = chartReader.randomiseBarOrder(false);
        audioManager.songBpm = chartReader.bpm;
        enemyObject.enemyTurn = false;
        StartCoroutine(CountdownThenStartSong());
        Invoke("byeReady", 2.0f);

        audioManager.samplesPerBeat = audioManager.FindSamplesPerBeat(audioManager.musicSource);

        songSamplesPerBeat = audioManager.FindSamplesPerBeat(audioManager.musicSource);

        StartCoroutine(CountdownThenStartSong());

        Debug.Log($"Bars in chart: {chartReader.barCount}");

        foreach (Bar bar in fullChart)
        {
            totalNotes += bar.getNoteCount();
        }
        maxScore = totalNotes * 300;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_playingSong && Input.GetKeyDown(KeyCode.Escape)) { // instead of esc key lose condition
            _playingSong = false;
            audioManager.stopSong();
        }

        if (_playingSong) {

            if (!firstBarSpawned)
            {
                notesSpawned = spriteManager.DrawBar(fullChart[0]);
                firstBarSpawned = true;
            }

            float beatInLoop = audioManager.loopBeat();
            
            int newBarIndex = Mathf.FloorToInt(audioManager.currentBeatInSong / timeSignature);

            // taking turns between player and enemy
            if (beatInLoop >= 1f && beatInLoop <= 4.90f)
            {
                enemyObject.enemyTurn = true;
            }
            else
            {
                enemyObject.enemyTurn = false;
            }

            // next bar
            if (newBarIndex != currentBarIndex)
            {
                currentBarIndex = newBarIndex;

                if (enemyObject.enemyTurn) {   
                    foreach (GameObject note in notesSpawned) {
                        Destroy(note);
                    }
                    chartBarIndex += 1;
                    enemyObject.enemyNextBar();
                    _Player.playerNextBar();
                    if (chartBarIndex < chartReader.barCount) { notesSpawned = spriteManager.DrawBar(fullChart[chartBarIndex]); }
                }
            }

            // still playing song
            if (chartBarIndex <= chartReader.barCount - 1) {
                if (enemyObject.enemyTurn)
                {
                    if (enemyObject.clapToPattern(fullChart[chartBarIndex], beatInLoop))
                    {
                        enemyObject.Clap();
                    }
                }
                else
                {
                    _Player.noTapMissCheck(beatInLoop, fullChart[chartBarIndex]);
                }

                if (Input.GetKeyDown(_Player.keyToPressA) || Input.GetKeyDown(_Player.keyToPressB)) {
                    if (enemyObject.enemyTurn) {
                        Debug.Log("Not your turn yet! - lose health" );
                    } else {
                        _Player.accuracyScoring(beatInLoop, fullChart[chartBarIndex]);
                    }
                }
            } else {
                // fade audio when out of bars
                IEnumerator fadeSound = audioManager.FadeOut(audioManager.musicSource, 0.3f);
                StartCoroutine(fadeSound);
                StopCoroutine(fadeSound);
            }
        }
        if (countdownDone && !_playingSong && !resultsPage.activeInHierarchy) {
            // calculate and show results screen
            calculateResults(_Player.perfectHits, _Player.greatHits, _Player.mehHits, _Player.misses, maxScore);
            resultsPage.SetActive(true);
        }


    }

    private IEnumerator CountdownThenStartSong() {
        yield return new WaitForSeconds(2f); // delay before countdown (optional)

        yield return StartCoroutine(audioManager.PlayDynamicCountdown()); // play 4-beat countdown

        countdownDone = true;
        _playingSong = true;
        audioManager.playSong();
    }      

    void byeReady() {
        _readyText.enabled = false;
    }

    private void calculateResults(int perfects, int greats, int mehs, int misses, int totalMaxScore) {
        int finalScore = perfects * 300 + 
                            greats * 100 + 
                            mehs * 50;

        float percentage = ((float)finalScore / totalMaxScore) * 100;
        float rounded = (float)Math.Round((double)percentage, 2);
        string formatted = rounded.ToString("F2");

        perfectsText.text = perfects.ToString();
        greatsText.text = greats.ToString();
        mehsText.text = mehs.ToString();
        missesText.text = misses.ToString();
        totalScoreText.text = finalScore.ToString();
        percentageText.text = formatted + "%";
            
    }

}
