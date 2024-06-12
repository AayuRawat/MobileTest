using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public GameObject laserPrefab;
    public Transform[] laserPositions; // Positions around the arena for lasers
    public float minLaserActiveTime = 3f; // Minimum time lasers stay active
    public float maxLaserActiveTime = 6f; // Maximum time lasers stay active
    public float delayBetweenWaves = 2f; // Delay between waves
    public float waveDuration = 5f; // Duration for each wave
    public int totalWaves = 5;

    private int currentWave = 1;

    void Start()
    {
        // Start the laser activation coroutine
        StartCoroutine(SpawnLasers());
    }

    IEnumerator SpawnLasers()
    {
        while (currentWave <= totalWaves)
        {
            // Start a new wave
            yield return StartCoroutine(ActivateLaserWave());

            // Wait for the interval before starting the next wave
            yield return new WaitForSeconds(delayBetweenWaves);

            currentWave++;
        }

        // Show win panel after all waves are completed
        UIManager.Instance.ShowWinPanel();
    }

    IEnumerator ActivateLaserWave()
    {
        List<GameObject> spawnedLasers = new List<GameObject>();

        foreach (Transform position in laserPositions)
        {
            StartCoroutine(ActivateLaserAtRandomTime(position, spawnedLasers));
        }

        // Wait for the wave duration
        yield return new WaitForSeconds(waveDuration);

        // Deactivate all lasers after the wave duration
        foreach (GameObject laser in spawnedLasers)
        {
            if (laser != null)
            {
                Destroy(laser);
            }
        }
    }

    IEnumerator ActivateLaserAtRandomTime(Transform position, List<GameObject> spawnedLasers)
    {
        // Wait for a random time before activating the laser
        float randomDelay = Random.Range(0, waveDuration - minLaserActiveTime);
        yield return new WaitForSeconds(randomDelay);

        // Instantiate and activate the laser
        GameObject laser = Instantiate(laserPrefab, position.position, position.rotation);
        laser.SetActive(true);
        spawnedLasers.Add(laser);

        // Wait for a random active time within the specified range
        float activeTime = Random.Range(minLaserActiveTime, maxLaserActiveTime);
        yield return new WaitForSeconds(activeTime);

        // Deactivate the laser after it has been active
        if (laser != null)
        {
            Destroy(laser);
        }
    }
}
