using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bar
{
    public string id;
    public string pattern;

    public int getNoteCount() {
        int noteCount = 0;
        string[] notes = pattern.Split(" ");
        foreach (string note in notes)
        {
            if (note.Contains("X"))
            {
                noteCount += 1;
            }
            else if (note.Contains("D"))
            {
                noteCount += 2;
            }
            else if (note.Contains("Q"))
            {
                noteCount += 4;
            }
            else if (note.Contains("T"))
            {
                noteCount += 3;
            }
        }
        return noteCount;
    }

    public List<float> getPatternTimings(bool includeRests, bool includeDuplicates) {
        List<float> timings = new List<float>();
        float currentBeat = 1.0f;

        string[] notes = pattern.Split(" ");

        foreach (string note in notes)
        {
            float duration = calculateDuration(note);
            bool isRest = note.Contains("R");
            bool isNote = note.Contains("X");
            // for double/ quaduple 8th or 16th notes and triple 8th notes
            bool isDoubleNote = note.Contains("D");
            bool isQuadNote = note.Contains("Q");
            bool isTriplet = note.Contains("T");

            if (isNote)
            {
                timings.Add(currentBeat);
                currentBeat += duration;
            }

            else if (isRest)
            {
                if (includeRests)
                {
                    timings.Add(currentBeat);
                    currentBeat += duration;
                }
                else
                {
                    currentBeat += duration;
                }

            }

            if (isDoubleNote)
            {
                timings.Add(currentBeat);
                if (includeDuplicates)
                {
                    currentBeat += duration;
                    timings.Add(currentBeat);
                    currentBeat += duration;
                }
                else
                {
                    float doubleDuration = duration * 2;
                    currentBeat += doubleDuration;
                }
            }
            if (isQuadNote)
            {
                timings.Add(currentBeat);
                if (includeDuplicates)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        currentBeat += duration;
                        timings.Add(currentBeat);
                    }
                    currentBeat += duration;
                }
                else
                {
                    float quadDuration = duration * 4;
                    currentBeat += quadDuration;
                }
            }

            if (isTriplet)
            {
                timings.Add(currentBeat);
                if (includeDuplicates)
                {
                    float startingBeat = currentBeat;
                    float endingBeat = currentBeat + 1f;
                    for (int i = 0; i < 2; i++)
                    {
                        currentBeat += 1f / 3;
                        timings.Add(currentBeat);
                    }
                    currentBeat = endingBeat;

                }
                else
                {
                    currentBeat += duration;
                }
            }
        }
        if (checkBeatOverflow(currentBeat) && !includeDuplicates) {
            Debug.LogError($"Note overflow! (Last beat duration ends at {currentBeat})");
            return null;
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
    
    private bool checkBeatOverflow(float finalDuration) {
        if (finalDuration > 5) {
            return true;
        }
        return false;
    }   
}



