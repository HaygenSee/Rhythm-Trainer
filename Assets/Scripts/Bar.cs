using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bar
{
    public string id;
    public string pattern;

    public int getNoteCount() {
        int noteLength = 0;
        string[] notes = pattern.Split(" ");
        foreach (string note in notes){ 
            if (note.Contains("X")) {
                noteLength += 1;
            }
        }
        return noteLength;
    }

    public List<float> getPatternTimings() {
        List<float> timings = new List<float>();
        float currentBeat = 1.0f;

        string[] notes = pattern.Split(" ");

        foreach (string note in notes) {
            float duration = calculateDuration(note);
            bool isNote = note.Contains("X");

            if (isNote) {
                timings.Add(currentBeat);
            }

            currentBeat += duration;
        }

        return timings;
    }

    private float calculateDuration(string note) {
        float baseDuration = 1.0f;

        if (note.Contains("/")) {
            string[] parts = note.Split("/");
            if (float.TryParse(parts[1].TrimEnd('.'), out float denom)) {
                baseDuration = 1.0f / denom;
            }
        }

        if (char.IsDigit(note[0])) {
            if (int.TryParse(note.Substring(0, 1), out int mult)) {
                baseDuration *= mult;
            }
        }

        if (note.EndsWith(".")) {
            baseDuration *= 1.5f;
        }

        return baseDuration;
    }
}

