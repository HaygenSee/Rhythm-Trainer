using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    AudioManager audioManager;
    SpriteManager spriteManager;
    public Transform enemyTransform;
    private SpriteRenderer SR;
    public Sprite normalImage;
    public Sprite pressedImage;
    public bool muteClap;
    public KeyCode keyToPressA;
    public KeyCode keyToPressB;
    public float offset;
    private float perfectWindow = 0.1f;
    private float greatWindow = 0.24f;
    public int perfectHits, greatHits, misses;
    public int earlyNotes = 0;
    public int lateNotes = 0;
    public int missedNotes = 0;
    private HashSet<float> clappedBeats = new HashSet<float>();

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        spriteManager = GameObject.FindGameObjectWithTag("SpriteManager").GetComponent<SpriteManager>();
    }
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        perfectHits = 0;
        greatHits = 0;
        misses = 0;
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


    public bool accuracyScoring(float clapTime, Bar currentBar)
    {
        List<float> timings = currentBar.getPatternTimings(false, true);
        clapTime = clapTime + offset - 4;
        foreach (float beat in timings)
        {
            if (clappedBeats.Contains(beat)) continue;

            if (clapTime >= beat)
            {
                float diff = Mathf.Abs(clapTime - beat);
                if (diff <= perfectWindow)
                {
                    clappedBeats.Add(beat);
                    lateNotes += 1;
                    perfectHits += 1;
                    spawnEffect(spriteManager.perfectHit);
                    return true;
                }
                else if (diff <= greatWindow)
                {
                    clappedBeats.Add(beat);
                    lateNotes += 1;
                    greatHits += 1;
                    spawnEffect(spriteManager.greatHit);
                    showEarlyLate(spriteManager.tellLate);
                    return true;
                }
                else
                {
                    clappedBeats.Add(beat);
                    lateNotes += 1;
                    misses += 1;
                    spawnEffect(spriteManager.missHit);
                    showEarlyLate(spriteManager.tellLate);
                    takeDamage();
                    return false;
                }

            }
            else
            {
                float diff = Mathf.Abs(beat - clapTime);
                if (diff <= perfectWindow)
                {
                    clappedBeats.Add(beat);
                    earlyNotes += 1;
                    perfectHits += 1;
                    spawnEffect(spriteManager.perfectHit);
                    return true;
                }
                else if (diff <= greatWindow)
                {
                    clappedBeats.Add(beat);
                    earlyNotes += 1;
                    greatHits += 1;
                    spawnEffect(spriteManager.greatHit);
                    showEarlyLate(spriteManager.tellEarly);
                    return true;
                }
                else
                {
                    clappedBeats.Add(beat);
                    earlyNotes += 1;
                    misses += 1;
                    spawnEffect(spriteManager.missHit);
                    showEarlyLate(spriteManager.tellEarly);
                    takeDamage();
                    return false;
                }
            }
        }
        return false;
    }

    public void noTapMissCheck(float currentBeat, Bar currentBar)
    {
        List<float> timings = currentBar.getPatternTimings(false, true);
        currentBeat -= 4;
        foreach (float beat in timings)
        {
            if (clappedBeats.Contains(beat)) continue;

            if (currentBeat - beat > 0.23)
            {
                clappedBeats.Add(beat);
                spawnEffect(spriteManager.missHit);
                missedNotes += 1;
                misses += 1;
                takeDamage();
            }
        }
    }

    public void playerNextBar()
    {
        clappedBeats.Clear();
    }

    private void spawnEffect(GameObject timingEffect)
    {
        Vector2 randomOffset = Random.insideUnitCircle * 1.2f;

        Vector3 spawnPosition = enemyTransform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        GameObject hitText = Instantiate(timingEffect, spawnPosition, Quaternion.identity);
        spriteManager.pulseOnAppear(hitText);
        Destroy(hitText, 0.45f);
    }

    private void showEarlyLate(GameObject timingObject)
    {
        Vector3 spawnPosition;

        if (timingObject.name == "early")
        {
            spawnPosition = transform.position + new Vector3(-1f, 1f, 0f);
        }
        else
        {
            spawnPosition = transform.position + new Vector3(1f, 1f, 0f);
        }

        GameObject offsetText = Instantiate(timingObject, spawnPosition, Quaternion.identity);
        spriteManager.pulseOnAppear(offsetText);
        Destroy(offsetText, 0.45f);
    }

    public void takeDamage()
    {
        StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        SR.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        SR.color = Color.white;
    }
}

