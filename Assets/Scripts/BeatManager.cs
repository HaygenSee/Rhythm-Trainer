using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public float bpm = 120f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private UnityEvent _trigger;
    private float samplesPerBeat;
    private float samplesPerBar;
    private int lastBeat = -1;

    void Start()
    {
        float sampleRate = _audioSource.clip.frequency;
        samplesPerBeat = sampleRate * (60f / bpm); 
    }

    void Update()
    {
        int currentBeat = Mathf.FloorToInt(_audioSource.timeSamples / samplesPerBeat);
        Debug.Log("Current Beat = " + currentBeat);
        if (currentBeat != lastBeat) {
            lastBeat = currentBeat;
            _trigger.Invoke();
        }
    }
}