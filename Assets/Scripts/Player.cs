using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    private float greatWindow = 0.25f;
    private float mehWindow = 0.35f;
    public int perfectHits, greatHits, mehHits, misses;
    public int earlyNotes = 0;
    public int lateNotes = 0;
    public int missedNotes = 0;
    private HashSet<float> clappedBeats = new HashSet<float>();

    void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        spriteManager = GameObject.FindGameObjectWithTag("SpriteManager").GetComponent<SpriteManager>();
    }
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        perfectHits = 0;
        greatHits = 0;
        mehHits = 0;
        misses = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(keyToPressA) || Input.GetKeyDown(keyToPressB)) {  
            if (!muteClap) { audioManager.playFX(audioManager.clap); }
            SR.sprite = pressedImage;
        }

        if (Input.GetKeyUp(keyToPressA) || Input.GetKeyUp(keyToPressB)) {
            SR.sprite = normalImage;
        }
    }

    public void accuracyScoring(float clapTime, Bar currentBar) {
        List<float> timings = currentBar.getPatternTimings(false);
        clapTime = clapTime + offset - 4;
        foreach (float beat in timings) {

            if (clappedBeats.Contains(beat)) continue;

            if (clapTime >= beat) {
                float diff = Mathf.Abs(clapTime - beat);
                if (diff <= perfectWindow) {
                    Debug.Log($"Perfect hit! ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    lateNotes += 1;
                    perfectHits += 1;
                    spawnEffect(spriteManager.perfectHit);
                    return;
                }
                else if (diff <= greatWindow) {
                    Debug.Log($"Late Great hit! ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    lateNotes += 1;
                    greatHits += 1;
                    spawnEffect(spriteManager.greatHit);
                    return;
                }
                else if (diff <= mehWindow){
                    Debug.Log($"Late Meh hit ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    lateNotes += 1;
                    mehHits += 1;
                    spawnEffect(spriteManager.mehHit);
                    return;
                }
                else {
                    clappedBeats.Add(beat);
                    lateNotes += 1;
                    misses += 1;
                    spawnEffect(spriteManager.missHit);
                    return;
                }
  
            } else {
                float diff = Mathf.Abs(beat - clapTime);
                if (diff <= perfectWindow) {
                    Debug.Log($"Perfect hit! ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    earlyNotes += 1;
                    perfectHits += 1;
                    spawnEffect(spriteManager.perfectHit);
                    return;
                }
                else if (diff <= greatWindow) {
                    Debug.Log($"Early Great hit! ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    earlyNotes += 1;
                    greatHits += 1;
                    spawnEffect(spriteManager.greatHit);
                    return;
                }
                else if (diff <= mehWindow){
                    Debug.Log($"Early Meh hit ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    earlyNotes += 1;
                    mehHits += 1;
                    spawnEffect(spriteManager.mehHit);
                    return;
                }
                else {
                    Debug.Log("Miss - Way too Early");
                    clappedBeats.Add(beat);
                    earlyNotes += 1;
                    misses += 1;
                    spawnEffect(spriteManager.missHit);
                    return;
                }
            }
        }
    }

    public void noTapMissCheck(float currentBeat, Bar currentBar) {
        List<float> timings = currentBar.getPatternTimings(false);
        currentBeat -= 4;
        foreach (float beat in timings){
            if (clappedBeats.Contains(beat)) continue;

            if (currentBeat - beat > 0.4) {
                clappedBeats.Add(beat);
                spawnEffect(spriteManager.missHit);
                missedNotes += 1;
            }
        }
    }

    public void playerNextBar() {
        clappedBeats.Clear();
    }

    private void spawnEffect(GameObject timingEffect) {
        Vector2 randomOffset = Random.insideUnitCircle * 1.2f;

        Vector3 spawnPosition = enemyTransform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        GameObject hitText = Instantiate(timingEffect, spawnPosition, Quaternion.identity);
        spriteManager.pulseOnAppear(hitText);
        Destroy(hitText, 0.45f); 
    }
}

