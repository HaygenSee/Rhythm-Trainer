using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    SpriteRenderer SR;
    AudioManager audioManager;
    ChartReader chartReader;
    public int Health;
    private HashSet<float> clappedBeats = new HashSet<float>();
    private HashSet<float> pointedBeats = new HashSet<float>();
    void Awake()
    {
        chartReader = GameObject.FindGameObjectWithTag("ChartReader").GetComponent<ChartReader>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        SR = GetComponent<SpriteRenderer>();

    }

    // bar pattern the enemy will clap to
    public bool clapToPattern(Bar currentBar, float currentBeat)
    {
        List<float> tempBar = currentBar.getPatternTimings(false, true);

        float timingWindow = 0.05f; // acceptable margin

        foreach (float beat in tempBar)
        {
            if (Mathf.Abs(currentBeat - beat) <= timingWindow && !clappedBeats.Contains(beat))
            {
                audioManager.playFX(audioManager.EnemyClap);
                clappedBeats.Add(beat);
                return true;
            }
        }
        return false;
    }

    public void setArrowLocation(Vector3 pointerStartSize, GameObject pointer, Bar currentBar, float currentBeat)
    {
        List<float> xAxisValues = currentBar.getPatternTimings(true, true);

        float timingWindow = 0.05f;

        foreach (float beat in xAxisValues)
        {
            if (Mathf.Abs(currentBeat - beat) <= timingWindow && !pointedBeats.Contains(beat))
            {
                pointedBeats.Add(beat);
                pointer.transform.position = new Vector3(beat - 3f, 2.3f, 0f);
                pointer.transform.localScale = pointerStartSize * 1.5f;
                return;
            }
        }
    }


    // reset clapped/ pointed beats on each bar
    public void enemyNextBar()
    {
        pointedBeats.Clear();
        clappedBeats.Clear();
    }

    public void Clap()
    {
        StartCoroutine(ClapRoutine());
    }
    private IEnumerator ClapRoutine()
    {
        _animator.SetBool("activateClap", true);
        yield return new WaitForSeconds(0.15f);
        _animator.SetBool("activateClap", false);
    }

    public void takeDamage()
    {
        StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        SR.color = Color.red;
        _animator.SetBool("takeDamage", true);
        yield return new WaitForSeconds(0.05f);
        _animator.SetBool("takeDamage", false);
        SR.color = Color.white;
    }
}
