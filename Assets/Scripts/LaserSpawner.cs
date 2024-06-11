using System.Collections;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public GameObject laserPrefab;
    public float spawnInterval = 2f;
    private int currentWave = 1;
    public int totalWaves = 5;

    void Start()
    {
        StartCoroutine(SpawnLasers());
    }

    IEnumerator SpawnLasers()
    {
        while (currentWave <= totalWaves)
        {
            // Start a new wave
            StartCoroutine(SpawnLaserWave());
            yield return new WaitForSeconds(spawnInterval * 10); // Adjust duration for each wave

            currentWave++;
        }

        // Show win panel after all waves are completed
        UIManager.Instance.ShowWinPanel();
    }

    IEnumerator SpawnLaserWave()
    {
        for (int i = 0; i < currentWave * 5; i++) // Increase laser count with each wave
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10)); // Adjust for arena size
            Instantiate(laserPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval / currentWave); // Increase speed by decreasing interval
        }
    }
}
