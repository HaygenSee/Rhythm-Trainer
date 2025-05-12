using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    ChartReader chartReader;
    public bool enemyTurn;
    private float timingWindow = 0.1f; // Acceptable error range in beats
    public int Health;
    void Awake() {
        chartReader = GameObject.FindGameObjectWithTag("ChartReader").GetComponent<ChartReader>();
        enemyTurn = true;
    }


    // bar pattern the enemy will clap to
    public void clapToPattern(Bar currentBar, float currentBeat) {
        string pattern = currentBar.pattern;
        List<float> tempBar = currentBar.getPatternTimings();

        Debug.Log($"Current beat: {currentBeat}, current pattern: {pattern}, translated pattern:  {string.Join(", ", tempBar)}");
    }
}
