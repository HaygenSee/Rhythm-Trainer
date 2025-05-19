using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    AudioManager audioManager;
    ChartReader chartReader;
    public bool enemyTurn;
    public int Health;
    private HashSet<float> clappedBeats = new HashSet<float>();
    void Awake() {
        chartReader = GameObject.FindGameObjectWithTag("ChartReader").GetComponent<ChartReader>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }

    // bar pattern the enemy will clap to
    public bool clapToPattern(Bar currentBar, float currentBeat) {
        List<float> tempBar = currentBar.getPatternTimings(false, true);

        float timingWindow = 0.05f; // acceptable margin

        foreach (float beat in tempBar) {
            if (Mathf.Abs(currentBeat - beat) <= timingWindow && !clappedBeats.Contains(beat)) {
                audioManager.playFX(audioManager.EnemyClap);
                clappedBeats.Add(beat);
                return true;
            }
        }
        return false;   
    }   

    // reset clapped beats on each bar
    public void enemyNextBar() {
        clappedBeats.Clear();
    }

    public void Clap() {
        StopAllCoroutines();
        StartCoroutine(ClapRoutine());
    }
    private IEnumerator ClapRoutine() {
        _animator.SetBool("activateClap", true);
        yield return new WaitForSeconds(0.15f);
        _animator.SetBool("activateClap", false);
    }
}
