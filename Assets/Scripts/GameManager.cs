using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    AudioManager audioManager;
    ChartReader chartReader;
    SpriteManager spriteManager;
    ScoreManager scoreManager;
    UIManager _UIManager;
    Player _Player;
    private bool firstBarSpawned = false;
    private List<GameObject> notesSpawned;
    private float songSamplesPerBeat;
    private List<Bar> fullChart;
    private int currentBarIndex = 0; private int chartBarIndex = 0; private float timeSignature = 4f;
    private int totalNotes = 0; private int maxScore = 0;
    public Enemy enemyObject;
    public TMP_Text _readyText;
    public GameObject resultsPage;
    private Turn currentTurn;
    private GameState currentGameState;
    private string levelScoreKey;
    [SerializeField]
    private bool _showLogs;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        chartReader = GameObject.FindGameObjectWithTag("ChartReader").GetComponent<ChartReader>();
        spriteManager = GameObject.FindGameObjectWithTag("SpriteManager").GetComponent<SpriteManager>();
        scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
        _UIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

    }

    void Start()
    {
        setLevelKey();
        fullChart = chartReader.randomiseBarOrder(true);
        audioManager.songBpm = chartReader.bpm;

        Invoke("byeReady", 2.0f);

        audioManager.samplesPerBeat = audioManager.FindSamplesPerBeat(audioManager.musicSource);

        songSamplesPerBeat = audioManager.FindSamplesPerBeat(audioManager.musicSource);

        StartCoroutine(CountdownThenStartSong());

        Log($"Bars in chart: {chartReader.barCount}");

        foreach (Bar bar in fullChart)
        {
            totalNotes += bar.getNoteCount();
        }
        maxScore = totalNotes * 300;

        Log($"Total notes: {totalNotes}");

    }

    public enum Turn
    {
        Player,
        Opponent
    }
    public enum GameState
    {
        Playing,
        Paused,
        Results,
        Countdown,

    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.Countdown)
        {
            if ((Input.anyKeyDown || Input.anyKeyDown) && !(Input.GetKeyDown(KeyCode.Escape)))
            {
                if (!_Player.muteClap) { audioManager.playFX(audioManager.clap); }
                _Player.Clap();
            }
        }
        else if (currentGameState == GameState.Playing)
        {
            _UIManager.pauseButton.SetActive(true);

            if ((Input.anyKeyDown || Input.anyKeyDown) && !(Input.GetKeyDown(KeyCode.Escape)))
            {
                if (!_Player.muteClap) { audioManager.playFX(audioManager.clap); }
                _Player.Clap();
            }

            float beatInLoop = audioManager.loopBeat();
            int newBarIndex = Mathf.FloorToInt(audioManager.currentBeatInSong / timeSignature);

            if (!firstBarSpawned)
            {
                audioManager.playFX(audioManager.bpmClap);
                notesSpawned = spriteManager.DrawBar(fullChart[0]);
                spriteManager.toggleTetoLight(true);
                firstBarSpawned = true;
            }

            // taking turns between player and enemy
            if (beatInLoop >= 1f && beatInLoop <= 4.90f)
            {
                currentTurn = Turn.Opponent;
            }
            else
            {
                currentTurn = Turn.Player;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _UIManager.pauseGame();
                currentGameState = GameState.Paused;
            }

            if (_UIManager.pauseScreen.activeSelf)
            {
                currentGameState = GameState.Paused;
            }

            // next bar
            if (newBarIndex != currentBarIndex)
            {
                currentBarIndex = newBarIndex;

                if (currentTurn == Turn.Opponent)
                {
                    foreach (GameObject note in notesSpawned)
                    {
                        Destroy(note);
                    }
                    chartBarIndex += 1;
                    enemyObject.enemyNextBar();
                    _Player.playerNextBar();

                    if (chartBarIndex < chartReader.barCount)
                    {
                        spriteManager.toggleTetoLight(true);
                        spriteManager.togglePlayerLight(false);
                        notesSpawned = spriteManager.DrawBar(fullChart[chartBarIndex]);
                    }
                    else
                    {
                        // jover
                        spriteManager.toggleTetoLight(false);
                        spriteManager.togglePlayerLight(false);
                    }
                }
            }

            // still playing song
            if (chartBarIndex <= chartReader.barCount - 1)
            {
                if (currentTurn == Turn.Opponent)
                {
                    if (PlayerPrefs.GetInt("audioQues", 1) == 1)
                    {
                        if (enemyObject.clapToPattern(fullChart[chartBarIndex], beatInLoop))
                        {
                            spriteManager.quePointer.SetActive(true);
                            enemyObject.setArrowLocation(spriteManager.quePointer, fullChart[chartBarIndex], beatInLoop);
                            enemyObject.Clap();
                        }
                    }
                    else
                    {
                        spriteManager.AudioQueHintOn();
                    }
                }
                else
                {
                    spriteManager.quePointer.SetActive(false);
                    spriteManager.toggleTetoLight(false);
                    spriteManager.togglePlayerLight(true);
                    _Player.noTapMissCheck(beatInLoop, fullChart[chartBarIndex]);
                }

                if ((Input.anyKeyDown || Input.anyKeyDown) && !(Input.GetKeyDown(KeyCode.Escape)) && currentGameState == GameState.Playing)
                {
                    if (currentTurn == Turn.Opponent)
                    {
                        Log("Not your turn yet! - lose health");
                    }
                    else
                    {
                        bool hit = _Player.accuracyScoring(beatInLoop, fullChart[chartBarIndex]);
                        if (hit)
                        {
                            enemyObject.takeDamage();
                        }
                    }
                }
            }
            else
            {
                IEnumerator fadeSound = FadeOut(audioManager.musicSource, 0.3f);
                StartCoroutine(fadeSound);
                StopCoroutine(fadeSound);
            }
        }
        else if (currentGameState == GameState.Paused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                currentGameState = GameState.Playing;
                _UIManager.resumeGame();
            }

            if (!_UIManager.pauseScreen.activeSelf)
            {
                currentGameState = GameState.Playing;
            }
        }
        else if (currentGameState == GameState.Results)
        {
            // calculate and show results screen
            spriteManager.musicVolumeBar.SetActive(false);
            spriteManager.SFXVolumeBar.SetActive(false);
            spriteManager.toggleHint(false);
            _UIManager.pauseButton.SetActive(false);
            if (scoreManager.calculateResults(_Player.perfectHits, _Player.greatHits, _Player.misses, maxScore, _Player.earlyNotes, _Player.lateNotes, levelScoreKey))
            {
                resultsPage.SetActive(true);
                scoreManager.highscoreText.SetActive(true);
            }
            else
            {
                resultsPage.SetActive(true);
            }
        }
    }

    private IEnumerator CountdownThenStartSong()
    {
        currentGameState = GameState.Countdown;
        yield return new WaitForSeconds(2f); // delay before countdown (optional)

        yield return StartCoroutine(audioManager.PlayDynamicCountdown()); // play 4-beat countdown

        audioManager.playSong();
        currentGameState = GameState.Playing;
    }

    private IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (startVolume > 0.001f)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.Stop();
        currentGameState = GameState.Results;
        audioSource.volume = startVolume;
    }


    void byeReady()
    {
        _readyText.enabled = false;
    }

    private void setLevelKey()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Easy")
        {
            levelScoreKey = "easyHighscore";
        }
        else if (scene.name == "Consistency Test")
        {
            levelScoreKey = "definitelyEasyHighscore";
        }
    }

    void Log(object message)
    {
        if (_showLogs)
        {
            Debug.Log(message);
        }
    }
}
