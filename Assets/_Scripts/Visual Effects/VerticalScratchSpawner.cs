using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalScratchSpawner : MonoBehaviour
{
    //Check if scratches should be spawned
    private bool _loop = true;

    //The last coroutine started
    private Coroutine _lastCoroutine;

    //The vertical scratch sprite that will be spawned
    public GameObject VerticalScratch;

    //The amount of time waited until a new scratch is spawned
    private float _timeUntilNextScratch = 0.5f;

    //The time range between scratches
    public float
        MinimumTime = 0f,
        MaximumTime = 0.25f;
    
    void OnEnable()
    {
        _lastCoroutine = StartCoroutine(SpawnScratches());
    }

    private void OnDisable()
    {
        if (_lastCoroutine != null)
        {
            StopCoroutine(_lastCoroutine);
        }
    }

    private IEnumerator SpawnScratches()
    {
        while (_loop)
        {
            yield return new WaitForSecondsRealtime(_timeUntilNextScratch);

            //Create a new scratch
            InstantiateNewScratch();

            //Randomize the time until the next scratch is generated
            RandomizeTime();
        }
    }

    private void InstantiateNewScratch()
    {
        Instantiate(VerticalScratch, new Vector2(GetRandomXPosition(), Screen.height), Quaternion.identity, transform);
    }

    private int GetRandomXPosition()
    {
        return Random.Range(0, Screen.width);
    }

    private void RandomizeTime()
    {
        _timeUntilNextScratch = Random.Range(MinimumTime, MaximumTime);
    }
}
