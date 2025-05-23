using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    [SerializeField] float _pulseSize = 1.5f;
    [SerializeField] float _returnSpeed = 0.15f;
    [Header("Hit Game Objects")]
    public GameObject perfectHit; public GameObject greatHit; public GameObject missHit;
    public GameObject tellLate; public GameObject tellEarly;

    [Header("Note Symbols Game Objects")]
    public GameObject crotchetGO; public GameObject quaverGO; public GameObject semiquaverGO;
    public GameObject doubleQuaverGO; public GameObject doubleSemiquaverGO;
    public GameObject quadQuaverGO; public GameObject quadSemiquaverGO; 
    public GameObject crotchetRestGO; public GameObject quaverRestGO;
    public GameObject semiquaverRestGO; public GameObject dotGO;

    [Header("Spotlight Objects")]
    public GameObject tetoLight; public GameObject playerLight;
    public GameObject hintText;
    public GameObject musicVolumeBar, SFXVolumeBar;

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
        List<float> timings = currentBar.getPatternTimings(true, false);
        string[] notations = currentBar.pattern.Split(" ");
        int count = notations.Length;
        // float spacing = 4f / count;
        // float startX = -2f + spacing / 2f; 

        for (int i = 0; i < count; i++)
        {
            string note = notations[i];
            GameObject symbolToSpawn = getNotePrefab(note);
            float xPos = timings[i] - 3f;
            // float xPos = startX + i * spacing;
            Vector3 spawnPos = new Vector3(xPos, 2.85f, 0f);

            GameObject instance = Instantiate(symbolToSpawn, spawnPos, Quaternion.identity);
            if (note.Contains('.'))
            {
                GameObject instanceDot = Instantiate(dotGO, spawnPos, Quaternion.identity);
                spawnedSymbols.Add(instanceDot);
            }
            spawnedSymbols.Add(instance);
        }
        return spawnedSymbols;
    }

    public void togglePlayerLight(bool onOrOff)
    {
        playerLight.SetActive(onOrOff);
    }

    public void toggleTetoLight(bool onOrOff)
    {
        tetoLight.SetActive(onOrOff);
    }

    public void toggleHint(bool onoff)
    {
        hintText.SetActive(onoff);
    }

    // get gameobject from notation;
    private GameObject getNotePrefab(string notation)
    {
        notation = notation.Replace(".", string.Empty);
        switch (notation)
        {
            case "X": return crotchetGO;
            case "X/2": return quaverGO;
            case "X/4": return semiquaverGO;
            case "D/2": return doubleQuaverGO;
            case "D/4": return doubleSemiquaverGO;
            case "Q/2": return quadQuaverGO;
            case "Q/4": return quadSemiquaverGO;

            case "R": return crotchetRestGO;
            case "R/2": return quaverRestGO;
            case "R/4": return semiquaverRestGO;

            default: return null;
        }
    }
    
}
