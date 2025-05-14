using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartReader : MonoBehaviour
{
    public TextAsset jsonFile;
    private List<Bar> barList;
    public int barCount;
    public float bpm { get; private set; }

    public List<Bar> randomiseBarOrder(bool shuffle) {
        var bars = LoadBars();
        barCount = bars.Count;
        if (shuffle) {
            for (int i = 0; i < bars.Count; i++) {
                var temp = bars[i];
                int randInt = Random.Range(i, bars.Count);
                bars[i] = bars[randInt];
                bars[randInt] = temp;
            }   
        }
        
        return bars;
    }


    // read and organise bars from json
    public List<Bar> LoadBars() {
        BarList wrapper = JsonUtility.FromJson<BarList>(jsonFile.text);
        bpm = wrapper.bpm;
        return wrapper.bars;
    }
}

    [System.Serializable]
    public class BarList {
        public float bpm;
        public List<Bar> bars;

    }
