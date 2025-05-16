using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [SerializeField] float _pulseSize = 1.5f;
    [SerializeField] float _returnSpeed = 0.15f;
    [Header("Hit Game Objects")]
    public GameObject perfectHit; public GameObject greatHit; public GameObject mehHit; public GameObject missHit;

    [Header("Note Symbols Game Objects")]
    public GameObject crotchetGO; public GameObject quaverGO; public GameObject semiquaverGO;
    public GameObject doubleQuaverGO; public GameObject doubleSemiquaverGO;
    public GameObject crotchetRestGO; public GameObject quaverRestGO;
    public GameObject semiquaverRestGO; public GameObject dotGO;

    public void pulseOnAppear(GameObject _timingText)
    {
        StartCoroutine(Pulse(_timingText));
    }

    private IEnumerator Pulse(GameObject target)
    {
        Vector3 originalScale = target.transform.localScale;
        Vector3 startScale = originalScale * _pulseSize;

        target.transform.localScale = startScale;

        float elapsed = 0f;
        while (elapsed < _returnSpeed)
        {
            target.transform.localScale = Vector3.Lerp(startScale, originalScale, elapsed / _returnSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.transform.localScale = originalScale;
    }

    public List<GameObject> DrawBar(Bar currentBar)
    {
        List<GameObject> spawnedSymbols = new List<GameObject>();
        List<float> timings = currentBar.getPatternTimings(true);
        string[] notations = currentBar.pattern.Split(" ");
        float spaceDivision = 4f / timings.Count;
        float currentPosition = -2f;

        for (int i = 0; i < notations.Length; i++)
        {
            int count = notations.Length;
            string note = notations[i];

            string noteName = returnSymbolName(note);
            GameObject symbolToSpawn = getNotePrefab(noteName);

                float t = (count == 1) ? 0.5f : (float)i / (count - 1); 
            float xPos = Mathf.Lerp(-2f, 2f, t);
            Vector3 spawnPos = new Vector3(xPos, 2.85f, 0f);

            GameObject instance = Instantiate(symbolToSpawn, spawnPos, Quaternion.identity);
            if (note.Contains('.'))
            {
                GameObject instanceDot = Instantiate(dotGO, spawnPos, Quaternion.identity);
                spawnedSymbols.Add(instanceDot);
            }
            spawnedSymbols.Add(instance);
            currentPosition += spaceDivision;
        }
        return spawnedSymbols;
    }

    // get gameobject from name;
    private GameObject getNotePrefab(string noteName)
    {
        switch (noteName)
        {
            case "crotchet": return crotchetGO;
            case "quaver": return quaverGO;
            case "semiquaver": return semiquaverGO;

            case "crotchetrest": return crotchetRestGO;
            case "quaverrest": return quaverRestGO;
            case "semiquaverrest": return semiquaverRestGO;

            default: return null;
        }
    }
    
    // return note name from notation
    private string returnSymbolName(string notation)
    {
        notation = notation.Replace(".", string.Empty); 
        switch (notation)
        {
            case "X": return "crotchet";
            case "X/2": return "quaver";
            case "X/4": return "semiquaver";

            case "R": return "crotchetrest";
            case "R/2": return "quaverrest";
            case "R/4": return "semiquaverrest";

            default: return null;
        }
    }
    
}
