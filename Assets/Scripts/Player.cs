using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private ScoreClass _scoreClass;
    AudioManager audioManager;
    private SpriteRenderer SR;
    public Sprite normalImage;
    public Sprite pressedImage;
    public bool muteClap;
    public KeyCode keyToPressA;
    public KeyCode keyToPressB;
    public float offset;
    private float perfectWindow = 0.1f;
    private float greatWindow = 0.2f;
    private float mehWindow = 0.3f;
    public int earlyNotes = 0;
    public int lateNotes = 0;
    public int missedNotes = 0;
    private HashSet<float> clappedBeats = new HashSet<float>();

    void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        ScoreClass finalScore = new ScoreClass();
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
        List<float> timings = currentBar.getPatternTimings();
        clapTime = clapTime + offset - 4;
        foreach (float beat in timings) {

            if (clappedBeats.Contains(beat)) continue;

            if (clapTime >= beat) {
                float diff = Mathf.Abs(clapTime - beat);
                if (diff <= perfectWindow) {
                    Debug.Log($"Late Perfect hit! ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    lateNotes += 1;
                    return;
                }
                else if (diff <= greatWindow) {
                    Debug.Log($"Late Great hit! ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    lateNotes += 1;
                    return;
                }
                else if (diff <= mehWindow){
                    Debug.Log($"Late Meh hit ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    lateNotes += 1;
                    return;
                }
                else {
                    Debug.Log("Miss - Way too late");
                    clappedBeats.Add(beat);
                    lateNotes += 1;
                    return;
                }
  
            } else {
                float diff = Mathf.Abs(beat - clapTime);
                if (diff <= perfectWindow) {
                    Debug.Log($"Early Perfect hit! ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    earlyNotes += 1;
                    return;
                }
                else if (diff <= greatWindow) {
                    Debug.Log($"Early Great hit! ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    earlyNotes += 1;
                    return;
                }
                else if (diff <= mehWindow){
                    Debug.Log($"Early Meh hit ({clapTime:F3} vs {beat:F3})");
                    clappedBeats.Add(beat);
                    earlyNotes += 1;
                    return;
                }
                else {
                    Debug.Log("Miss - Way too Early");
                    clappedBeats.Add(beat);
                    earlyNotes += 1;
                    return;
                }
            }
        }
    }

    public void noTapMissCheck(float currentBeat, Bar currentBar) {
        List<float> timings = currentBar.getPatternTimings();
        currentBeat -= 4;
        foreach (float beat in timings){
            if (clappedBeats.Contains(beat)) continue;

            if (currentBeat - beat > 0.4) {
                Debug.Log("bro you need to tap you missed");
                clappedBeats.Add(beat);
                missedNotes += 1;
            }
        }
    }

    public void playerNextBar() {
        clappedBeats.Clear();
    }

    private class ScoreClass {
        public int totalNotes;
        public int perfects;
        public int greats;
        public int mehs;
        public int misses;
    }
}
