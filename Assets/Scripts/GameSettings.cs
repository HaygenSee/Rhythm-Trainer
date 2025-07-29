using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public Toggle toggleQues;
    [Header("Highscores")]
    [SerializeField]
    private Text tutorialHighscore, easyHighscore, defoEasyHighscore;
    public AudioSource testSFX;
    public AudioClip clapTestSound;

    void Start()
    {
        if (!PlayerPrefs.HasKey("audioQues"))
            PlayerPrefs.SetInt("audioQues", 1);

        bool toggleState = PlayerPrefs.GetInt("audioQues") == 1;
        toggleQues.isOn = toggleState;

        toggleQues.onValueChanged.RemoveAllListeners();
        toggleQues.onValueChanged.AddListener(OnAudioCueToggleChanged);

        if (!PlayerPrefs.HasKey("easyHighscore"))
            PlayerPrefs.SetInt("easyHighscore", 0);

        if (!PlayerPrefs.HasKey("definitelyEasyHighscore"))
            PlayerPrefs.SetInt("definitelyEasyHighscore", 0);

        if (!PlayerPrefs.HasKey("tutorialHighscore"))
            PlayerPrefs.SetInt("tutorialHighscore", 0);

        easyHighscore.text = "Highscore: " + getEasyHighScore().ToString();
        defoEasyHighscore.text = "Highscore: " + getDefinitelyEasyHighScore().ToString();
        tutorialHighscore.text = "Highscore: " + getTutorialHighScore().ToString();
    }


    public void OnAudioCueToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt("audioQues", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private int getEasyHighScore()
    {
        return PlayerPrefs.GetInt("easyHighscore");
    }
    private int getDefinitelyEasyHighScore()
    {
        return PlayerPrefs.GetInt("definitelyEasyHighscore");
    }

    private int getTutorialHighScore()
    {
        return PlayerPrefs.GetInt("tutorialHighscore");
    }
    
    public void clapTest()
    {
        testSFX.PlayOneShot(clapTestSound);
    }


}
