using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    [Header("Results Text")]
    public TMP_Text perfectsText; public TMP_Text greatsText; public TMP_Text missesText;
    public TMP_Text totalScoreText; public TMP_Text percentageText;
    public TMP_Text earlyText; public TMP_Text lateText;
    public GameObject highscoreText;
    public int highscore;

    private void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Easy")
        {
            highscore = PlayerPrefs.GetInt("easyHighscore");
        }
        else if (scene.name == "Consistency Test")
        {
            highscore = PlayerPrefs.GetInt("definitelyEasyHighscore");
        }
        else if (scene.name == "Tutorial") {
            highscore = PlayerPrefs.GetInt("tutorialHighscore");
        }
    }

    public bool calculateResults(int perfects, int greats, int misses, int totalMaxScore, int earlys, int lates, string levelScoreKey)
    {
        int finalScore = perfects * 300 +
                            greats * 100;

        float percentage = ((float)finalScore / totalMaxScore) * 100;
        float rounded = (float)Math.Round((double)percentage, 2);
        string formatted = rounded.ToString("F2");
        string earlysFormatted = earlys.ToString();
        string latesFormatted = lates.ToString();

        perfectsText.text = perfects.ToString();
        greatsText.text = greats.ToString();
        missesText.text = misses.ToString();
        totalScoreText.text = finalScore.ToString();
        percentageText.text = formatted + "%";
        earlyText.text = "Early: " + earlysFormatted;
        lateText.text = "Late: " + latesFormatted;

        if (finalScore > PlayerPrefs.GetInt(levelScoreKey))
        {
            PlayerPrefs.SetInt(levelScoreKey, finalScore);
            return true;
        }
        return false;
    } 
}
