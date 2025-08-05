using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    AudioManager audioManager;
    GameManager gameManager;
    public bool gamePaused = false;
    public GameObject pauseScreen;
    public GameObject pauseButton;
    public GameObject settingsUI;
    

    void Awake()
    {
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");

        if (audioObject != null)
            audioManager = audioObject.GetComponent<AudioManager>();
        if (gameManagerObject != null)
            gameManager = gameManagerObject.GetComponent<GameManager>();
    }

    public void OnRestartPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void loadEasyLevel()
    {
        SceneManager.LoadScene("easy");
    }

    public void loadDefinitelyEasyLevel()
    {
        SceneManager.LoadScene("Consistency Test");
    }
    public void loadIntermediateLevel()
    {
        SceneManager.LoadScene("Intermediate");
    }

    public void loadTutorialLevel()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("menu");
    }

    public void resumeGame()
    {
        pauseScreen.SetActive(false);
        pauseButton.SetActive(true);
        audioManager.ResumeSong();
    }

    public void pauseGame()
    {
        audioManager.pauseSong();
        gamePaused = true;
        pauseScreen.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void openSettings()
    {
        settingsUI.SetActive(true);
    }

    public void closeSettings()
    {
        settingsUI.SetActive(false);
    }

    
    public void quitGame()
    {
        Application.Quit();
    }
}
