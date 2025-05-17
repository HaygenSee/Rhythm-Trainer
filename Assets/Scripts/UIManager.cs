using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    AudioManager audioManager;
    public bool gamePaused;
    public GameObject pauseScreen;
    public GameObject pauseButton;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        gamePaused = false;
    }
    public void OnRestartPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void loadEasyLevel()
    {
        SceneManager.LoadScene("easy");
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("menu");
    }

    public void resumeGame()
    {
        pauseScreen.SetActive(false);
        pauseButton.SetActive(true);
        gamePaused = false;
        audioManager.ResumeSong();
    }

    public void pauseGame()
    {
        audioManager.pauseSong();
        gamePaused = true;
        pauseScreen.SetActive(true);
        pauseButton.SetActive(false);
    }
}
